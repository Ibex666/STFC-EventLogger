using System;
using System.Collections.Generic;
using STFC_EventLogger.AllianceClasses;

namespace STFC_EventLogger
{
    public class UserConfig : IEquatable<UserConfig?>
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public string DisplayName { get; set; }

        public Dictionary<string, List<string>> Aliase { get; set; }

        public Rect EventListAnalyzerRect { get; set; }
        public Rect AllianceListAnalyzerRect { get; set; }
        
        /// <summary>
        /// Alliance List Boundary Points
        /// </summary>
        public BoundaryPoints AllianceListBP { get; set; }

        /// <summary>
        /// Event List Boundary Points
        /// </summary>
        public BoundaryPoints EventListBP { get; set; }

        [Obsolete]
        public Rect RectEventNames { get; set; }
        [Obsolete]
        public Rect RectEventScores { get; set; }
        [Obsolete]
        public Rect RectAllianceNames { get; set; }
        [Obsolete]
        public Rect RectAlliancePower { get; set; }        
        
        public Rect RectSsTypeAnalyzer { get; set; }
        public byte MaxParallelTasks { get; set; }
        public bool UseInvertedImages { get; set; }

        public List<AccuracyBrushLimits> AccuracyLevelBrushLimits { get; set; }
        public List<AccuracyBrushLimits> AccuracyScoreBrushLimits { get; set; }
        public List<AccuracyBrushLimits> AccuracyPowerBrushLimits { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UserConfig);
        }

        public bool Equals(UserConfig? other)
        {
            return other is not null &&
                   DisplayName == other.DisplayName &&
                   EqualityComparer<Dictionary<string, List<string>>>.Default.Equals(Aliase, other.Aliase) &&
                   EqualityComparer<Rect>.Default.Equals(RectEventNames, other.RectEventNames) &&
                   EqualityComparer<Rect>.Default.Equals(RectEventScores, other.RectEventScores) &&
                   EqualityComparer<Rect>.Default.Equals(EventListAnalyzerRect, other.EventListAnalyzerRect) &&
                   EqualityComparer<Rect>.Default.Equals(RectAllianceNames, other.RectAllianceNames) &&
                   EqualityComparer<Rect>.Default.Equals(RectAlliancePower, other.RectAlliancePower) &&
                   EqualityComparer<Rect>.Default.Equals(AllianceListAnalyzerRect, other.AllianceListAnalyzerRect) &&
                   EqualityComparer<Rect>.Default.Equals(RectSsTypeAnalyzer, other.RectSsTypeAnalyzer) &&
                   EqualityComparer<BoundaryPoints>.Default.Equals(AllianceListBP, other.AllianceListBP) &&
                   EqualityComparer<BoundaryPoints>.Default.Equals(EventListBP, other.EventListBP) &&
                   MaxParallelTasks == other.MaxParallelTasks &&
                   UseInvertedImages == other.UseInvertedImages &&
                   EqualityComparer<List<AccuracyBrushLimits>>.Default.Equals(AccuracyLevelBrushLimits, other.AccuracyLevelBrushLimits) &&
                   EqualityComparer<List<AccuracyBrushLimits>>.Default.Equals(AccuracyScoreBrushLimits, other.AccuracyScoreBrushLimits) &&
                   EqualityComparer<List<AccuracyBrushLimits>>.Default.Equals(AccuracyPowerBrushLimits, other.AccuracyPowerBrushLimits);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(DisplayName);
            hash.Add(Aliase);
            hash.Add(RectEventNames);
            hash.Add(RectEventScores);
            hash.Add(EventListAnalyzerRect);
            hash.Add(RectAllianceNames);
            hash.Add(RectAlliancePower);
            hash.Add(AllianceListAnalyzerRect);
            hash.Add(RectSsTypeAnalyzer);
            hash.Add(MaxParallelTasks);
            hash.Add(UseInvertedImages);
            hash.Add(AccuracyLevelBrushLimits);
            hash.Add(AccuracyScoreBrushLimits);
            hash.Add(AccuracyPowerBrushLimits);
            hash.Add(AllianceListBP);
            hash.Add(EventListBP);
            return hash.ToHashCode();
        }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.



        public override string? ToString()
        {
            return $"{DisplayName}";
        }

        public static bool operator ==(UserConfig? left, UserConfig? right)
        {
            return EqualityComparer<UserConfig>.Default.Equals(left, right);
        }

        public static bool operator !=(UserConfig? left, UserConfig? right)
        {
            return !(left == right);
        }
    }
}
