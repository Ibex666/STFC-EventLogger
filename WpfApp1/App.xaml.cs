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
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            try { V.us = deserializer.Deserialize<UserSettings>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\config.yaml"))); }
            catch (Exception)
            {
                MessageBox.Show("error loading 'config.yaml'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            try { V.Aliase = deserializer.Deserialize<List<AliasClass>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\alias.yaml"))); }
            catch (Exception)
            {
                MessageBox.Show("error loading 'alias.yaml'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            if (V.Aliase is not null)
            {
                foreach (var item in V.Aliase)
                {
                    if (!V.NameDicts.ContainsKey(item.Name))
                    {
                        V.NameDicts.Add(item.Name, item.AKA.ToList());
                    }
                    else
                    {
                        V.NameDicts[item.Name].AddRange(item.AKA.ToList());
                    }
                }
            }


            try { V.OcrGarbage = deserializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\ocr_garbage.yaml"))); }
            catch (Exception) { }

            if (V.OcrGarbage is not null)
            {
                foreach (var item in V.OcrGarbage)
                {
                    V.NameDicts[item.Key].AddRange(item.Value);
                }
            }
            else
            {
                V.OcrGarbage = new();
            }


            V.frmMain = new MainWindow();
            V.frmMain.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(V.OcrGarbage);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\ocr_garbage.yaml"), yaml);

            base.OnExit(e);
        }
    }
}
