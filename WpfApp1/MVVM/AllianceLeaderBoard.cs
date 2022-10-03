using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using STFC_EventLogger.AllianceClasses;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace STFC_EventLogger.MVVM
{
    public class AllianceLeaderBoard : INotifyPropertyChanged
    {
        private ObservableCollection<AllianceMember> members;
        private List<AllianceMember> membersInternal;
        private List<string> tmpFiles;
        private List<SSTypeAnalyzer> filesToScan;
        private List<OcrName> notRecognizedNames;
        private Visibility columnVisibility;

        #region #- Events -#

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        public Visibility ColumnVisibility
        {
            get { return columnVisibility; }
            set
            {
                columnVisibility = value;
                OnPropertyChanged();
            }
        }

        private readonly BackgroundWorker bgw_Scanner;

        public AllianceLeaderBoard()
        {
            membersInternal = new List<AllianceMember>();
            members = new ObservableCollection<AllianceMember>();
            columnVisibility = Visibility.Hidden;
            ScanEnabled = true;
            tmpFiles = new();
            filesToScan = new();
            notRecognizedNames = new();

            bgw_Scanner = new BackgroundWorker();
            bgw_Scanner.DoWork += Bgw_Scanner_DoWork;
            bgw_Scanner.RunWorkerCompleted += Bgw_Scanner_RunWorkerCompleted;
        }

        public void Clear()
        {
            Members.Clear();
            MembersInternal.Clear();
            NotRecognizedNames.Clear();
            TmpFiles.Clear();
            FilesToScan.Clear();

            AllianceScore = null;
        }

        internal List<AllianceMember> MembersInternal { get => membersInternal; set => membersInternal = value; }
        public ObservableCollection<AllianceMember> Members { get => members; set => members = value; }


        private ICommand? scanCommand;

        public ICommand ScanCommand
        {
            get
            {
                scanCommand ??= new RelayCommand(
                        p => ScanEnabled,
                        p => Scan());
                return scanCommand;
            }
        }

        public bool ScanEnabled { get; set; }

        private void Scan()
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                Filter = "Bilddateien|*.jpg;*.png;*.bmp;*.tiff"
            };

            if (ofd.ShowDialog() == false)
                return;

            Clear();

            foreach (var file in ofd.FileNames)
            {
                FilesToScan.Add(new SSTypeAnalyzer(file, ImageTypes.Positive));
            }

            ScanEnabled = false;
            bgw_Scanner.RunWorkerAsync();
        }

        private void Bgw_Scanner_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (V.us.UseInvertedImages)
            {
                F.InvertImages();
            }

            F.AnalyzeScreenshots();
            F.ScanMemberList();
            F.ScanEventList();
        }

        private void Bgw_Scanner_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            foreach (var item in MembersInternal)
            {
                item.MergeData();
                Members.Add(item);
            }

            ScanEnabled = true;

            Task.Run(() =>
            {
                foreach (var item in TmpFiles)
                {
                    if (File.Exists(item))
                    {
                        File.Delete(item);
                    }
                }
            });
        }


        private ICommand? finishCommand;

        public ICommand FinishCommand
        {
            get
            {
                finishCommand ??= new RelayCommand(
                        p => true,
                        p => Finish());
                return finishCommand;
            }
        }

        private void Finish()
        {
            AllianceScore = 0;
            foreach (var item in MembersInternal)
            {
                AllianceScore += item.BestScore.HasValue ? item.BestScore : 0;
            }

            int i = 1;
            foreach (var item in MembersInternal.OrderBy(_ => _.BestScore, new DescendingNullableUlongComparer()))
            {
                if (item.BestScore == null)
                    continue;

                MembersInternal[MembersInternal.IndexOf(item)].EventRanking = i++;
            }

            i = 1;
            foreach (var item in MembersInternal.OrderBy(_ => _.BestPower, new DescendingNullableUlongComparer()))
            {
                MembersInternal[MembersInternal.IndexOf(item)].PowerRanking = i++;
            }
        }

        private ulong? allianceScore;

        public ulong? AllianceScore
        {
            get => allianceScore;
            set
            {
                allianceScore = value;
                OnPropertyChanged();
            }
        }

        public List<string> TmpFiles { get => tmpFiles; set => tmpFiles = value; }
        public List<SSTypeAnalyzer> FilesToScan { get => filesToScan; set => filesToScan = value; }
        public List<OcrName> NotRecognizedNames { get => notRecognizedNames; set => notRecognizedNames = value; }
    }
}

