using System;
using System.Collections.Generic;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class OcrScore : BaseOcrClass, IEquatable<OcrScore?>, IComparable<OcrScore>
    {
        public ulong? Value { get; set; }

        public OcrScore() : base() { }
        public OcrScore(XmlNode? xml, SSTypeAnalyzer file) : base(xml, file)
        {
            if (Content != null)
            {
                Recognised = ulong.TryParse(CleanContentNumberString(Content), out ulong tmp);
                if (Recognised)
                {
                    Value = tmp;
                }
            }
        }

        public override string? ToString()
        {
            return $"{Value} / {Content} / {WC} / {Recognised}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as OcrScore);
        }

        public bool Equals(OcrScore? other)
        {
            return other is not null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public int CompareTo(OcrScore? other)
        {
            if (other != null && other.Value != null && Value != null)
                return Value.Value.CompareTo(other.Value.Value);
            return 0;
        }

        public static bool operator ==(OcrScore? left, OcrScore? right)
        {
            return EqualityComparer<OcrScore>.Default.Equals(left, right);
        }

        public static bool operator !=(OcrScore? left, OcrScore? right)
        {
            return !(left == right);
        }

        public static OcrScore? FromEventList(string xml, SSTypeAnalyzer file)
        {
            XmlDocument xdoc = new();
            xdoc.LoadXml(xml);
            var nodes = xdoc.SelectNodes("//String");
            if (nodes != null && nodes.Count > 0)
            {
                return new OcrScore(nodes[0], file)
                {
                    ScannerXml = xml,
                };
            }

            return null;
        }
    }
}