using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace STFC_EventLogger.AllianceClasses
{
    public class AccuracyBrushLimits : IEquatable<AccuracyBrushLimits?>
    {
        public AccuracyBrushLimits()
        {
            Brush = Brushes.Transparent;
        }

        public float Min { get; set; }
        public float Max { get; set; }
        public SolidColorBrush Brush { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AccuracyBrushLimits);
        }

        public bool Equals(AccuracyBrushLimits? other)
        {
            return other is not null &&
                   Min == other.Min &&
                   Max == other.Max &&
                   EqualityComparer<SolidColorBrush>.Default.Equals(Brush, other.Brush);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max, Brush);
        }

        public static bool operator ==(AccuracyBrushLimits? left, AccuracyBrushLimits? right)
        {
            return EqualityComparer<AccuracyBrushLimits>.Default.Equals(left, right);
        }

        public static bool operator !=(AccuracyBrushLimits? left, AccuracyBrushLimits? right)
        {
            return !(left == right);
        }
        public static implicit operator AccuracyBrushLimits(string value)
        {
            var split = value.Split(',');
            AccuracyBrushLimits r = new();
            r.Min = int.Parse(split[0].Trim());
            r.Max = int.Parse(split[1].Trim());

            var converter = new BrushConverter();
            r.Brush = (SolidColorBrush)converter.ConvertFromString(split[2].Trim());

            return r;
        }
    }
}