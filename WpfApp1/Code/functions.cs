﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Tesseract;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace STFC_EventLogger
{
    /// <summary>
    /// Enthält statische Funktionen
    /// </summary>
    internal static class F
    {
        private static IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
        private static ISerializer serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        /// <summary>
        /// Calculate the difference between 2 strings using the Levenshtein distance algorithm
        /// </summary>
        /// <param name="source1">First string</param>
        /// <param name="source2">Second string</param>
        /// <returns></returns>
        internal static int LevensteinDistance(string source1, string source2) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanMethod"></param>
        /// <param name="tessdata"></param>
        /// <param name="engineMode"></param>
        internal static void GetEngineModeData(ScanMethods scanMethod, out string tessdata, out EngineMode engineMode)
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

        internal static void LoadConfigs()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\"), "*.config.yaml"))
            {
                try
                {
                    UserConfig config = deserializer.Deserialize<UserConfig>(File.ReadAllText(file));
                    V.allianceLeaderBoard.UserConfigs.Add(config);
                }
                catch (Exception) { }
            }

            if (V.allianceLeaderBoard.UserConfigs.Count > 0)
            {
                V.allianceLeaderBoard.SelectedUserConfig = V.allianceLeaderBoard.UserConfigs[0];
            }
        }
        internal static void LoadMembers()
        {
            try { V.memberAdministrationMVVM.Members = deserializer.Deserialize<List<MemberAdministrationAlias>>(File.ReadAllText(V.file_settings_members)); }
            catch (Exception)
            {
                MessageBox.Show("error loading 'members.yaml'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GenerateNameDicts();
        }

        internal static void SaveMembers()
        {
            using TextWriter textWriter = new StringWriter();

            textWriter.WriteLine("# player names, aliase ans ocr garbage");
            textWriter.WriteLine("# example data");
            textWriter.WriteLine("#");
            textWriter.WriteLine("# - Name: Mara        display name");
            textWriter.WriteLine("#   AKA:              in game player names each per row (one must be the same as the diplay name)");
            textWriter.WriteLine("#   - Text: Mara");
            textWriter.WriteLine("#   - Text: MightyMara");
            textWriter.WriteLine("#   - Text: DarthMara");
            textWriter.WriteLine("");

            serializer.Serialize(textWriter, V.memberAdministrationMVVM.Members);

            File.WriteAllText(V.file_settings_members, textWriter.ToString());
        }

        internal static void GenerateNameDicts()
        {
            if (V.memberAdministrationMVVM.Members != null)
            {
                V.NameDicts.Clear();
                foreach (var member in V.memberAdministrationMVVM.Members)
                {
                    V.NameDicts.Add(member.Name, member.AKA.Select(_ => (string)_).ToList());
                }
            }
        }



        [Obsolete("only for compability, will be removed later")]
        internal static bool LoadAliase()
        {
            try { V.Aliase = deserializer.Deserialize<List<AliasClass>>(File.ReadAllText(V.file_settings_alias)); }
            catch (Exception)
            {
                MessageBox.Show("error loading 'alias.yaml'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (V.Aliase is not null)
            {
                var a = new List<AliasClass>();
                a.AddRange(V.Aliase.ToList());
                a.Sort();

                V.Aliase.Clear();
                foreach (var item in a)
                {
                    if (!V.Aliase.Contains(item))
                    {
                        V.Aliase.Add(item);
                    }
                    else
                    {
                        int idx = V.Aliase.IndexOf(item);
                        V.Aliase[idx].AKA.AddRange(item.AKA);
                    }
                }

                V.Aliase.Sort();
                foreach (var item in V.Aliase)
                {
                    item.AKA.Sort();

                    if (!V.NameDicts.ContainsKey(item.Name))
                    {
                        V.NameDicts.Add(item.Name, item.AKA.ToList());

                        var maa = new MemberAdministrationAlias();
                        maa.Name = item.Name;
                        maa.AKA.AddRange(item.AKA.Select(_ => (StringWrapper)_).ToList());
                        V.memberAdministrationMVVM.Members.Add(maa);
                    }
                    else
                    {
                        V.NameDicts[item.Name].AddRange(item.AKA.ToList());

                        var maa = V.memberAdministrationMVVM.Members.First(_ => _.Name == item.Name);
                        maa.AKA.AddRange(item.AKA.Select(_ => (StringWrapper)_).ToList());
                    }
                }
            }

            return true;
        }
    }
}