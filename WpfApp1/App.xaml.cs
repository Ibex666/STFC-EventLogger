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
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                V.us = deserializer.Deserialize<UserSettings>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\config.yaml")));
                V.Aliase = deserializer.Deserialize<List<AliasClass>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\alias.yaml")));
                V.OcrGarbage = deserializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\ocr_garbage.yaml")));

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
                foreach (var item in V.OcrGarbage)
                {
                    V.NameDicts[item.Key].AddRange(item.Value);
                }
            }
            catch (Exception)
            {
                throw;
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
