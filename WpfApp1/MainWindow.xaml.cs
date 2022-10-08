using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using STFC_EventLogger.AllianceClasses;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Input;

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
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

