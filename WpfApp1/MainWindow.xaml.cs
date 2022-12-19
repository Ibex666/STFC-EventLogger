﻿using System.Text;
using System.Threading;
using System.Windows;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using STFC_EventLogger.AllianceClasses;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Http.Headers;
using STFC_EventLogger.Windows;

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

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ToolBar toolBar)
            {
                if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
                {
                    overflowGrid.Visibility = Visibility.Collapsed;
                }
                if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
                {
                    mainPanelBorder.Margin = new Thickness();
                }
            }
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            var am = new AllianceMember();
            am.Name = OcrName.FromStrings("îbéybbb", "Ibex666");
            V.allianceLeaderBoard.NotRecognisedNames.Add(am);

            am = new AllianceMember();
            am.Name = OcrName.FromStrings("eeede", "eVo");
            V.allianceLeaderBoard.NotRecognisedNames.Add(am);

            NotRecognizedNamesWindow window = new();
            window.ShowDialog();
        }
    }
}