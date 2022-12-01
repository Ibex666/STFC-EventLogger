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
using System.Xml;
using System.Windows.Threading;
using System;
using STFC_EventLogger.Windows;
using System.Threading;
using Tesseract;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Documents;

namespace STFC_EventLogger.MVVM
{
    public class AllianceLeaderBoard : INotifyPropertyChanged
    {
        #region #- Private Fields -#

#if DEBUG
        private Stopwatch? stopwatch;
#endif
        private string? last_scanned_file;
        private readonly BackgroundWorker bgw_Scanner;
        private Visibility columnVisibility;
        private decimal? allianceScore;
        private decimal? powerAverage;
        private decimal? levelAverage;
        private bool isBusy;
        private string? panelMainMessage;
        private string? panelSubMessage;
        private string? elapsedTime;

        private ICommand? scanCommand;
        private ICommand? copyDataCommand;
        private ICommand? columnVisibilityCommand;
        private AllianceMember? selectedMember;


        #endregion

        #region #- Constructor -#


        public AllianceLeaderBoard()
        {
            UserConfigs = new();
            MembersInternal = new();
            AllianceListEntries = new();
            EventListEntries = new();
            Members = new();
            IsBusy = false;
            FilesToScan = new();
            NotRecognisedNames = new();

            columnVisibility = Visibility.Hidden;

            bgw_Scanner = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            bgw_Scanner.DoWork += Bgw_Scanner_DoWork;
            bgw_Scanner.RunWorkerCompleted += Bgw_Scanner_RunWorkerCompleted;
            bgw_Scanner.ProgressChanged += Bgw_Scanner_ProgressChanged;

            SelectedUserConfig = new();
        }

        #endregion

        #region #- Public Properties -#

        public UserConfig SelectedUserConfig { get; set; }
        public List<UserConfig> UserConfigs { get; set; }

        public ObservableCollection<AllianceMember> Members { get; set; }

        public AllianceMember? SelectedMember
        {
            get
            {
                return selectedMember;
            }
            set
            {
                selectedMember = value;
                OnPropertyChanged();
            }
        }

        internal List<AllianceListEntry> AllianceListEntries { get; set; }
        internal List<EventListEntry> EventListEntries { get; set; }


        internal List<AllianceMember> MembersInternal { get; set; }


        public List<SSTypeAnalyzer> FilesToScan { get; set; }
        public ObservableCollection<AllianceMember> NotRecognisedNames { get; set; }
        public Visibility ColumnVisibility
        {
            get { return columnVisibility; }
            set
            {
                columnVisibility = value;
                OnPropertyChanged();
            }
        }
        public decimal? AllianceScore
        {
            get => allianceScore;
            set
            {
                allianceScore = value;
                OnPropertyChanged();
            }
        }
        public decimal? PowerAverage
        {
            get => powerAverage;
            set
            {
                powerAverage = value;
                OnPropertyChanged();
            }
        }
        public decimal? LevelAverage
        {
            get => levelAverage;
            set
            {
                levelAverage = value;
                OnPropertyChanged();
            }
        }
        public bool IsBusy
        {
            get => isBusy; set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }
        public int MembersWithScore
        {
            get
            {
                return Members.Where(_ => _.BestScore.HasValue && _.BestScore.Value > 0).Count();
            }
        }
        public string ColumnVisibilityButtonText
        {
            get
            {

                switch (ColumnVisibility)
                {
                    case Visibility.Visible:
                        return "Show less columns";
                    default:
                        return "Show more columns";
                }
            }
        }

        public string? PanelMainMessage
        {
            get => panelMainMessage;
            set
            {
                panelMainMessage = value;
                OnPropertyChanged();
            }
        }
        public string? PanelSubMessage
        {
            get => panelSubMessage;
            set
            {
                panelSubMessage = value;
                OnPropertyChanged();
            }
        }
        public string? ElapsedTime
        {
            get => elapsedTime; set
            {
                elapsedTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand? ScanCommand
        {
            get
            {
                scanCommand ??= new RelayCommand(
                        p => !IsBusy,
                        p => Scan());
                return scanCommand;
            }
        }
        public ICommand? CopyDataCommand
        {
            get
            {
                copyDataCommand ??= new RelayCommand(
                        p => !IsBusy & Members.Count > 0,
                        p => CopyData());
                return copyDataCommand;
            }
        }
        public ICommand? ColumnVisibilityCommand
        {
            get
            {
                columnVisibilityCommand ??= new RelayCommand(
                        p => true,
                        p => ToggleColumnVisibility());
                return columnVisibilityCommand;
            }
        }



        #endregion

        #region #- Events -#

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region #- Instance Methods -#

        private void Clear()
        {
            Members.Clear();
            MembersInternal.Clear();
            AllianceListEntries.Clear();
            EventListEntries.Clear();
            FilesToScan.Clear();

            NotRecognisedNames = new();

            AllianceScore = null;
            LevelAverage = null;
            PowerAverage = null;

            OnPropertyChanged(nameof(MembersWithScore));
        }
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

            IsBusy = true;

#if DEBUG
            ElapsedTime = string.Empty;
            stopwatch = Stopwatch.StartNew();
#endif

            bgw_Scanner.RunWorkerAsync();
        }
        private void Finish()
        {
            AllianceScore = Members.Sum(_ => (decimal?)_.BestScore);

            int i = 1;
            foreach (var item in Members.OrderBy(_ => _.BestScore, new DescendingNullableUlongComparer()))
            {
                if (item.BestScore == null || item.BestScore == 0)
                    Members[Members.IndexOf(item)].EventRanking = null;
                else
                    Members[Members.IndexOf(item)].EventRanking = i++;
            }
            OnPropertyChanged(nameof(MembersWithScore));


            if (Members.Count > 0)
                LevelAverage = Members.Sum(_ => (decimal?)_.BestLevel) / Members.Count;

            if (Members.Count > 0)
                PowerAverage = Members.Sum(_ => (decimal?)_.BestPower) / Members.Count;

            i = 1;
            foreach (var item in Members.OrderBy(_ => _.BestPower, new DescendingNullableUlongComparer()))
            {
                if (item.BestPower == null || item.BestPower == 0)
                    Members[Members.IndexOf(item)].PowerRanking = null;
                else
                    Members[Members.IndexOf(item)].PowerRanking = i++;
            }

        }
        private void ToggleColumnVisibility()
        {
            switch (ColumnVisibility)
            {
                case Visibility.Visible:
                    ColumnVisibility = Visibility.Hidden;
                    break;
                default:
                    ColumnVisibility = Visibility.Visible;
                    break;
            }
            OnPropertyChanged(nameof(ColumnVisibilityButtonText));
        }
        private void Bgw_Scanner_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (SelectedUserConfig.UseInvertedImages)
            {
                bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("Invert Images", ScanWorkerProgressReportMessageTypes.MainMessage));
                InvertImages();
            }

            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("Analyze Screenshots", ScanWorkerProgressReportMessageTypes.MainMessage));
            AnalyzeScreenshots();

            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("Scan Screenshots", ScanWorkerProgressReportMessageTypes.MainMessage));
            ScanScreenshots();

            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("do some more stuff...", ScanWorkerProgressReportMessageTypes.MainMessage));
            DoSomeMoreStuff();

            if (NotRecognisedNames.Count > 0)
            {
#if DEBUG
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                }
#endif
                bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("handle not recognized names", ScanWorkerProgressReportMessageTypes.MainMessage));
                bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("", ScanWorkerProgressReportMessageTypes.SubMessage));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    HandleNotRecognizedNames();
                });
#if DEBUG
                if (stopwatch != null)
                {
                    stopwatch.Start();
                }
#endif
            }

            float x = 0;
            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("Merge Data", ScanWorkerProgressReportMessageTypes.MainMessage));
            ReportPercentageSubMessage(x, null);
            foreach (var item in MembersInternal)
            {
                item.MergeData();

                x += 1f / MembersInternal.Count;
                ReportPercentageSubMessage(x, null);
            }

            x = 0;
            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport("Add Members", ScanWorkerProgressReportMessageTypes.MainMessage));
            ReportPercentageSubMessage(x, null);
            foreach (var item in MembersInternal)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () =>
                {
                    item.BestScoreChanged += Member_BestScoreChanged;
                    item.BestLevelChanged += Member_BestLevelChanged;
                    item.BestPowerChanged += Member_BestPowerChanged;
                    Members.Add(item);

                    x += 1f / MembersInternal.Count;
                    ReportPercentageSubMessage(x, null);
                });
            }

            Finish();
        }
        private void Bgw_Scanner_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;

#if DEBUG
            if (stopwatch != null)
            {
                stopwatch.Stop();
                ElapsedTime = stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
                stopwatch = null;
            }
#endif
        }
        private void Bgw_Scanner_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ScanWorkerProgressReport report)
            {
                if (report.MessageType == ScanWorkerProgressReportMessageTypes.MainMessage)
                {
                    PanelMainMessage = report.Message;
                }
                else if (report.MessageType == ScanWorkerProgressReportMessageTypes.SubMessage)
                {
                    PanelSubMessage = report.Message;
                }

            }

#if DEBUG
            if (stopwatch != null)
            {
                ElapsedTime = stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
            }
#endif
        }

        private void Member_BestScoreChanged(object? sender, EventArgs e)
        {
            AllianceScore = Members.Sum(_ => (decimal?)_.BestScore);

            int i = 1;
            foreach (var item in Members.OrderBy(_ => _.BestScore, new DescendingNullableUlongComparer()))
            {
                if (item.BestScore == null || item.BestScore == 0)
                    Members[Members.IndexOf(item)].EventRanking = null;
                else
                    Members[Members.IndexOf(item)].EventRanking = i++;
            }

            OnPropertyChanged(nameof(MembersWithScore));
        }
        private void Member_BestLevelChanged(object? sender, EventArgs e)
        {
            if (Members.Count > 0)
                LevelAverage = Members.Sum(_ => (decimal?)_.BestLevel) / Members.Count;
        }
        private void Member_BestPowerChanged(object? sender, EventArgs e)
        {
            if (Members.Count > 0)
                PowerAverage = Members.Sum(_ => (decimal?)_.BestPower) / Members.Count;

            int i = 1;
            foreach (var item in Members.OrderBy(_ => _.BestPower, new DescendingNullableUlongComparer()))
            {
                if (item.BestPower == null || item.BestPower == 0)
                    Members[Members.IndexOf(item)].PowerRanking = null;
                else
                    Members[Members.IndexOf(item)].PowerRanking = i++;
            }
        }

        private void InvertImages()
        {
            float x = 0;
            float y = 1f / (V.allianceLeaderBoard.FilesToScan.Count / 2);
            ReportPercentageSubMessage(x, null);

            List<Task> tasks = new();
            List<SSTypeAnalyzer> tmpFiles = new();
            using SemaphoreSlim semaphore = new(SelectedUserConfig.MaxParallelTasks);
            foreach (var file in V.allianceLeaderBoard.FilesToScan)
            {
                semaphore.Wait();

                var t = Task.Factory.StartNew(() =>
                {
                    ReportPercentageSubMessage(x += y, file.FileName);

                    using Image img = Image.FromFile(file.FileName);
                    using Image imgNeg = ImageFunctions.InvertUnsafe(img);

                    string fileNeg = Path.GetTempFileName();
                    imgNeg.Save(fileNeg);
                    tmpFiles.Add(new SSTypeAnalyzer(fileNeg, ImageTypes.Negative));

                    ReportPercentageSubMessage(x += y, file.FileName);

                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());

            V.allianceLeaderBoard.FilesToScan.AddRange(tmpFiles);
        }
        private void AnalyzeScreenshots()
        {
            float x = 0;
            float y = 1f / (V.allianceLeaderBoard.FilesToScan.Count / 2);
            ReportPercentageSubMessage(x, null);

            List<Task> tasks = new();
            using SemaphoreSlim semaphore = new(SelectedUserConfig.MaxParallelTasks);
            foreach (var file in V.allianceLeaderBoard.FilesToScan)
            {
                semaphore.Wait();

                var t = Task.Factory.StartNew(() =>
                {
                    ReportPercentageSubMessage(x += y, file.FileName);

                    file.Analyze();

                    ReportPercentageSubMessage(x += y, file.FileName);

                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        private void ScanScreenshots()
        {
            float x = 0;
            float y = 0;
            ReportPercentageSubMessage(x, null);

            List<Task> tasks = new();

            using SemaphoreSlim semaphore = new(SelectedUserConfig.MaxParallelTasks);

            foreach (var file in FilesToScan)
            {
                semaphore.Wait();
                var t = Task.Factory.StartNew(() =>
                {
                    Pix image = Pix.LoadFromFile(file.FileName);

                    foreach (var datarow in file.DataRows)
                    {
                        y = 0.8f / (FilesToScan.Count * file.DataRows.Count * 3);

                        var xml1 = ScanArea(image, datarow.Rect2, ScanMethods.Tesseract);
                        var xml2 = ScanArea(image, datarow.Rect3, ScanMethods.Tesseract);
                        datarow.AddData(xml1, xml2, ScanMethods.Tesseract);
                        ReportPercentageSubMessage(x += y, file.FileName);

                        xml1 = ScanArea(image, datarow.Rect2, ScanMethods.Fast);
                        xml2 = ScanArea(image, datarow.Rect3, ScanMethods.Fast);
                        datarow.AddData(xml1, xml2, ScanMethods.Fast);
                        ReportPercentageSubMessage(x += y, file.FileName);

                        xml1 = ScanArea(image, datarow.Rect2, ScanMethods.Best);
                        xml2 = ScanArea(image, datarow.Rect3, ScanMethods.Best);
                        datarow.AddData(xml1, xml2, ScanMethods.Best);
                        ReportPercentageSubMessage(x += y, file.FileName);

                    }

                    ReportPercentageSubMessage(x += 0.1f / FilesToScan.Count, file.FileName);

                    image.Dispose();
                    semaphore.Release();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        private void DoSomeMoreStuff()
        {
            foreach (var file in FilesToScan)
            {
                foreach (var dr in file.DataRows)
                {
                    var ale = AllianceListEntry.FromSsTypeAnalyzer(dr, file);
                    var ele = EventListEntry.FromSsTypeAnalyzer(dr, file);

                    if (ale != null && ale.Name != null)
                        AllianceListEntries.Add(ale);

                    if (ele != null && ele.Name != null)
                        EventListEntries.Add(ele);
                }
            }

            foreach (var entry in AllianceListEntries)
            {
                var member = new AllianceMember(entry);
                if (entry.RecognisedName)
                {
                    if (MembersInternal.Contains(member))
                    {
                        int idx = MembersInternal.IndexOf(member);
                        MembersInternal[idx].Levels.AddRange(member.Levels);
                        MembersInternal[idx].Powers.AddRange(member.Powers);
                    }
                    else
                    {
                        MembersInternal.Add(member);
                    }
                }
                else
                {
                    NotRecognisedNames.Add(member);
                }
            }
            foreach (var entry in EventListEntries)
            {
                AllianceMember? member = MembersInternal.FirstOrDefault(_ => _.Name == entry.Name);
                if (member != null)
                {
                    int idx = MembersInternal.IndexOf(member);

                    member.EventListName = entry.Name;
                    member.Scores.AddRange(entry.Scores);
                    member.EventNameImage ??= entry.NameImage;
                    member.EventScoreImage ??= entry.ScoreImage;
                }
                else
                {
                    if (entry.RecognisedName)
                    {
                        MembersInternal.Add(new AllianceMember(entry));
                    }
                    else
                    {
                        NotRecognisedNames.Add(new AllianceMember(entry));
                    }
                }
            }
        }

        private void ReportPercentageSubMessage(float percentage, string? filename)
        {
            if (percentage >= 1)
                percentage = 1;

            bgw_Scanner.ReportProgress(0, new ScanWorkerProgressReport($"{percentage,0:p1}", ScanWorkerProgressReportMessageTypes.SubMessage));
            last_scanned_file = filename;
        }

        private void HandleNotRecognizedNames()
        {
            // taking screenshots from the name
            foreach (var item in NotRecognisedNames)
            {
                item.Name.Value = null;
                using var img = Image.FromFile(item.Name.FileName);

                item.Name.Image = ImageFunctions.ImageFromBuffer(ImageFunctions.CropImage(
                    img,
                    item.Name.X1 - 5,
                    item.Name.Y1 - 5,
                    item.Name.X2 + 5,
                    item.Name.Y2 + 5,
                    System.Drawing.Imaging.ImageFormat.Png));
            }

            NotRecognizedNamesWindow window = new();
            window.ShowDialog();

            foreach (var item in NotRecognisedNames)
            {
                AllianceMember? member = MembersInternal.SingleOrDefault(_ => _.Name == item.Name);
                if (member != null)
                {
                    int idx = MembersInternal.IndexOf(member);

                    if (item.PageType == PageTypes.MemberList)
                    {
                        member.Levels.AddRange(item.Levels);
                        member.Powers.AddRange(item.Powers);
                    }
                    else if (item.PageType == PageTypes.EventList)
                    {
                        member.Scores.AddRange(item.Scores);
                    }
                }
                else if (item.Name.Value != null && item.Name.Value != string.Empty && item.Name.Content != null && item.Name.Content != string.Empty)
                {
                    MembersInternal.Add(item);
                }
                else
                {
                    continue;
                }

                if (item.Name.Value != null && item.Name.Value != string.Empty && item.Name.Content != null && item.Name.Content != string.Empty)
                {
                    if (V.OcrGarbage.ContainsKey(item.Name.Value))
                    {
                        if (!V.OcrGarbage[item.Name.Value].Contains(item.Name.Content))
                            V.OcrGarbage[item.Name.Value].Add(item.Name.Content);
                    }
                    else
                    {
                        V.OcrGarbage.Add(item.Name.Value, new List<string>() { item.Name.Content });
                    }


                    if (V.NameDicts.ContainsKey(item.Name.Value))
                    {
                        if (!V.NameDicts[item.Name.Value].Contains(item.Name.Content))
                            V.NameDicts[item.Name.Value].Add(item.Name.Content);
                    }
                    else
                    {
                        V.NameDicts.Add(item.Name.Value, new List<string>() { item.Name.Content });
                    }
                }
            }
        }

        private void CopyData()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Member Count\tName\tOPS\tScore\tRating\tNotiz\tPower Rank\tName\tOPS\tMacht");

            foreach (var item in Members.OrderBy(_ => _.Name.Value))
            {
                if (item.BestLevel != null && item.BestScore != null && item.BestPower != null)
                    sb.AppendLine($"{item.EventRanking}\t{item.Name.Value}\t{item.BestLevel.Value}\t{item.BestScore.Value}\t\t\t{item.PowerRanking}\t{item.Name.Value}\t{item.BestLevel.Value}\t{item.BestPower.Value}");
            }
            Clipboard.SetText(sb.ToString());
        }

        #endregion

        #region #- Static Methods -#

        private static void GetEngineModeData(ScanMethods scanMethod, out string tessdata, out EngineMode engineMode)
        {
            switch (scanMethod)
            {
                case ScanMethods.Tesseract:
                    tessdata = @"tessdata";
                    engineMode = EngineMode.TesseractOnly;
                    break;
                case ScanMethods.Fast:
                    tessdata = @"tessdata_fast";
                    engineMode = EngineMode.LstmOnly;
                    break;
                case ScanMethods.Best:
                    tessdata = @"tessdata_best";
                    engineMode = EngineMode.LstmOnly;
                    break;
                default:
                    tessdata = @"tessdata";
                    engineMode = EngineMode.TesseractOnly;
                    break;
            }
        }

        [Obsolete]
        private static OcrPower ScanMemberPower(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrPower ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.Start.X < 0 | scanArea.Start.Y < 0 | scanArea.End.X > image.Width | scanArea.End.Y > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrPower(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }
        [Obsolete]
        private static OcrLevel ScanMemberLevel(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrLevel ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.Start.X < 0 | scanArea.Start.Y < 0 | scanArea.End.X > image.Width | scanArea.End.Y > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrLevel(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }
        [Obsolete]
        private static OcrScore ScanMemberScore(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrScore ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.Start.X < 0 | scanArea.Start.Y < 0 | scanArea.End.X > image.Width | scanArea.End.Y > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrScore(nodes[0], file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }
        [Obsolete]
        private static OcrName ScanMemberName(Pix image, Rect scanArea, SSTypeAnalyzer file, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);

            OcrName ret = new()
            {
                FileName = file.FileName
            };
            if (scanArea.Start.X < 0 | scanArea.Start.Y < 0 | scanArea.End.X > image.Width | scanArea.End.Y > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                XmlDocument xdoc = new();
                xdoc.LoadXml(page.GetAltoText(0));
                //Name = new OcrName(xml.SelectNodes("./TextLine[2]/String[position()>1]"), file);
                var nodes = xdoc.SelectNodes("//String");
                if (nodes != null && nodes.Count > 0)
                {
                    ret = new OcrName(nodes, file);
                }
            }
            catch (AccessViolationException) { }

            return ret;
        }

        private static string ScanArea(Pix image, Rect scanArea, ScanMethods scanMethod)
        {
            GetEngineModeData(scanMethod, out string tessdata, out EngineMode engineMode);
            string ret = string.Empty;

            if (scanArea.Start.X < 0 | scanArea.Start.Y < 0 | scanArea.End.X > image.Width | scanArea.End.Y > image.Height)
                return ret;

            try
            {
                using var engine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
                using var page = engine.Process(image, scanArea);
                ret = page.GetAltoText(0);
            }
            catch (AccessViolationException) { }

            return ret;
        }

        #endregion

        #region #- Interface/Overridden Methods -#



        #endregion

        #region #- Operators -#



        #endregion
    }
}

