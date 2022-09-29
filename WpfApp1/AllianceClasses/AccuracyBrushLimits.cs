using System.Windows.Media;

namespace STFC_EventLogger.AllianceClasses
{
    public class AccuracyBrushLimits
    {
        public AccuracyBrushLimits()
        {
            Brush = Brushes.Transparent;
        }

        public float Min { get; set; }
        public float Max { get; set; }
        public SolidColorBrush Brush { get; set; }
    }
}