using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using STFC_EventLogger.AllianceClasses;

namespace STFC_EventLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dataGrid.ItemsSource = V.obsAllianceMembers;

            V.bgw_Scanner.DoWork += Bgw_Scanner_DoWork;
            V.bgw_Scanner.RunWorkerCompleted += Bgw_Scanner_RunWorkerCompleted;

            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                V.us = deserializer.Deserialize<UserSettings>(System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml")));
            }
            catch (Exception)
            {
                throw;
            }
        }

        Stopwatch sw = new();

        private void Btn_Scan_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                Filter = "Bilddateien|*.jpg;*.png;*.bmp;*.tiff"
            };

            if (ofd.ShowDialog() == false)
                return;

            V.tmpFiles.Clear();
            V.filenamesToScan.Clear();
            V.filenamesToScan.AddRange(ofd.FileNames);

            V.allianceMembers.Clear();
            V.obsAllianceMembers.Clear();
            V.notRecognizedNames.Clear();

            btn_scan.IsEnabled = false;

            sw = Stopwatch.StartNew();
            V.bgw_Scanner.RunWorkerAsync();
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

            List<Task> tasks = new();
            using (SemaphoreSlim semaphore = new(V.us.MaxParallelTasks))
            {
                foreach (var item in V.allianceMembers)
                {
                    semaphore.Wait();
                    var t = Task.Factory.StartNew(() =>
                    {
                        Dispatcher.Invoke(() => item.MergeData());

                        semaphore.Release();
                    });
                    tasks.Add(t);
                }
                Task.WaitAll(tasks.ToArray());
            }

            foreach (var item in V.allianceMembers)
            {
                Dispatcher.Invoke(() => V.obsAllianceMembers.Add(item));
            }

            foreach (var item in V.tmpFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }
        }
        private void Bgw_Scanner_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            btn_scan.IsEnabled = true;

            sw.Stop();
            MessageBox.Show(sw.Elapsed.ToString());
        }


        private void Btn_confirm_score(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is ItemsPresenter data)
                {
                    AllianceMember allianceMember = (AllianceMember)data.DataContext;
                    allianceMember.ConfirmScore();
                    break;
                }
        }
        private void Btn_confirm_level(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is ItemsPresenter data)
                {
                    AllianceMember allianceMember = (AllianceMember)data.DataContext;
                    allianceMember.ConfirmLevel();
                    break;
                }
        }
        private void Btn_confirm_power(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is ItemsPresenter data)
                {
                    AllianceMember allianceMember = (AllianceMember)data.DataContext;
                    allianceMember.ConfirmPower();
                    break;
                }
        }
        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            CopyDataToClipboard();
        }

        private static void CopyDataToClipboard()
        {
            List<AllianceMember> am = new();
            am.AddRange(V.obsAllianceMembers);

            am.Sort(new AllianceMemberScoreComparer());
            for (int i = 0; i < am.Count; i++)
            {
                am[i].EventRanking = i + 1;
            }

            am.Sort(new AllianceMemberPowerComparer());
            for (int i = 0; i < am.Count; i++)
            {
                am[i].PowerRanking = i + 1;
            }

            StringBuilder sb = new();
            am.Sort((a, b) => a.Name.CompareTo(b.Name));
            sb.AppendLine($"Member Count\tName\tOPS\tScore\tRating\tNotiz\tPower Rank\tName\tOPS\tMacht");
            for (int i = 0; i < am.Count; i++)
            {
                if (am[i].BestLevel != null && am[i].BestScore != null && am[i].BestPower != null)
#pragma warning disable CS8629 // Ein Werttyp, der NULL zulässt, kann NULL sein.
                    sb.AppendLine($"{am[i].EventRanking}\t{am[i].Name.Value}\t{am[i].BestLevel.Value}\t{am[i].BestScore.Value}\t\t\t{am[i].PowerRanking}\t{am[i].Name.Value}\t{am[i].BestLevel.Value}\t{am[i].BestPower.Value}");
#pragma warning restore CS8629 // Ein Werttyp, der NULL zulässt, kann NULL sein.
            }
            Clipboard.SetText(sb.ToString());
        }
    }
}
