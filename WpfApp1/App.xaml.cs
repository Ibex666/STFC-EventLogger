using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Core;
using STFC_EventLogger.MVVM;

namespace STFC_EventLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            F.LoadConfigs();
            F.LoadAliase();
            F.LoadOcrGarbage();

            V.frmMain = new MainWindow();
            V.frmMain.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            F.SaveAliase();
            F.SaveOcrGarbage();

            base.OnExit(e);
        }
    }
}
