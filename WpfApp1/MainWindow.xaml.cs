using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using STFC_EventLogger.AllianceClasses;
using System.Drawing;
using System.Linq;

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
            DataContext = V.allianceLeaderBoard;

            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                V.us = deserializer.Deserialize<UserSettings>(System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml")));
                V.Aliase = deserializer.Deserialize<Dictionary<string, List<string>>>(System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alias.yaml")));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            //CopyDataToClipboard();
            switch (V.allianceLeaderBoard.ColumnVisibility)
            {
                case Visibility.Visible:
                    V.allianceLeaderBoard.ColumnVisibility = Visibility.Hidden;
                    break;
                default:
                    V.allianceLeaderBoard.ColumnVisibility = Visibility.Visible;
                    break;
            }
        }

        private static void CopyDataToClipboard()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Member Count\tName\tOPS\tScore\tRating\tNotiz\tPower Rank\tName\tOPS\tMacht");

            foreach (var item in V.allianceLeaderBoard.MembersInternal.OrderBy(_ => _.Name.Value))
            {
                if (item.BestLevel != null && item.BestScore != null && item.BestPower != null)
#pragma warning disable CS8629 // Ein Werttyp, der NULL zulässt, kann NULL sein.
                    sb.AppendLine($"{item.EventRanking}\t{item.Name.Value}\t{item.BestLevel.Value}\t{item.BestScore.Value}\t\t\t{item.PowerRanking}\t{item.Name.Value}\t{item.BestLevel.Value}\t{item.BestPower.Value}");
#pragma warning restore CS8629 // Ein Werttyp, der NULL zulässt, kann NULL sein.
            }
            Clipboard.SetText(sb.ToString());
        }
    }
}

