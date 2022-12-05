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
        internal static List<AliasClass> Aliase = new();
        internal static Dictionary<string, List<string>> OcrGarbage = new();

        internal static MainWindow? frmMain;
    }
}
