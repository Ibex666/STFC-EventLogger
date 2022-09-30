using System;
using System.Collections.Generic;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    internal class OcrLevel : BaseOcrClass, IEquatable<OcrLevel?>
    {
        public uint? Value { get; set; }

        public OcrLevel() : base() { }
        public OcrLevel(XmlNode? xml, SSTypeAnalyzer file) : base(xml, file)
        {
            if (Content != null)
            {
                if (uint.TryParse(CleanContentNumberString(Content), out uint tmp))
                {
                    Value = tmp;
                }
            }
        }

        public override string? ToString()
        {
            return $"{Value} / {Content} / {WC}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as OcrLevel);
        }

        public bool Equals(OcrLevel? other)
        {
            return other is not null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(OcrLevel? left, OcrLevel? right)
        {
            return EqualityComparer<OcrLevel>.Default.Equals(left, right);
        }

        public static bool operator !=(OcrLevel? left, OcrLevel? right)
        {
            return !(left == right);
        }
    }
}