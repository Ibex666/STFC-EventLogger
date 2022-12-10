using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using STFC_EventLogger.MVVM;
using STFC_EventLogger.Windows;
using System.Linq;
using STFC_EventLogger.AllianceClasses;
using System.IO;

namespace STFC_EventLogger
{
    /// <summary>
    /// Enthält Variablen und Konstanten
    /// </summary>
    internal static class V
    {
        /// <summary>
        /// User Settings
        /// </summary>
        internal static AllianceLeaderBoard allianceLeaderBoard = new();
        internal static MemberAdministrationMVVM memberAdministrationMVVM = new();

        internal static Dictionary<string, List<string>> NameDicts = new();

        [Obsolete("only for compability, will be removed later")]
        internal static List<AliasClass> Aliase = new();
        [Obsolete("only for compability, will be removed later")]
        internal static Dictionary<string, List<string>> OcrGarbage = new();

        internal static string file_settings_alias = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\alias.yaml");
        internal static string file_settings_ocr_garbage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\ocr_garbage.yaml");
        internal static string file_settings_members = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Settings\members.yaml");

        internal static MainWindow? frmMain;
    }
}
