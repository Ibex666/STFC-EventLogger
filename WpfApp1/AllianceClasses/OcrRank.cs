using System;
using System.Collections.Generic;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class OcrRank : BaseOcrClass, IEquatable<OcrRank?>
    {
        public AllianceRanks Value { get; set; }

        public OcrRank() : base() { }
        public OcrRank(XmlNode? xml, SSTypeAnalyzer file) : base(xml, file)
        {
            if (Content != null)
            {
                int _dist = int.MaxValue;
                foreach (var item in Enum.GetValues<AllianceRanks>())
                {
                    int _x = F.LevensteinDistance(Content.ToLower(), item.ToString().ToLower());
                    if (_x < _dist)
                    {
                        _dist = _x;
                        Value = item;
                    }
                }
            }
        }

        public override string? ToString()
        {
            return $"{Value} / {Content} / {WC}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as OcrRank);
        }

        public bool Equals(OcrRank? other)
        {
            return other is not null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(OcrRank? left, OcrRank? right)
        {
            return EqualityComparer<OcrRank>.Default.Equals(left, right);
        }

        public static bool operator !=(OcrRank? left, OcrRank? right)
        {
            return !(left == right);
        }
    }
}