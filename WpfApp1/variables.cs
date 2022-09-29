﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using STFC_EventLogger.AllianceClasses;

namespace STFC_EventLogger
{
    public class UserSettings
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public Dictionary<string, string[]> Aliase { get; set; }

        public Rect RectEventNames { get; set; }
        public Rect RectEventScores { get; set; }
        public Rect RectAllianceNames { get; set; }
        public Rect RectAlliancePower { get; set; }
        public Rect RectSsTypeAnalyzer { get; set; }
        public byte MaxParallelTasks { get; set; }
        public bool UseInvertedImages { get; set; }

        public List<AccuracyBrushLimits> AccuracyLevelBrushLimits { get; set; }
        public List<AccuracyBrushLimits> AccuracyScoreBrushLimits { get; set; }
        public List<AccuracyBrushLimits> AccuracyPowerBrushLimits { get; set; }

#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    }

    /// <summary>
    /// Enthält Variablen und Konstanten
    /// </summary>
    internal static class V
    {
        /// <summary>
        /// User Settings
        /// </summary>
        internal static UserSettings us = new();

        internal static BackgroundWorker bgw_Scanner = new();
        internal static List<string> filenamesToScan = new();
        internal static List<string> tmpFiles = new();
        internal static List<SSTypeAnalyzer> filesToScan = new();
        internal static List<AllianceMember> allianceMembers = new();
        internal static List<OcrName> notRecognizedNames = new();
        internal static ObservableCollection<AllianceMember> obsAllianceMembers = new();
    }
}
