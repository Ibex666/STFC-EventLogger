using System;
using System.Collections.Generic;
using System.Xml;

namespace STFC_EventLogger
{
    public class OcrPower : BaseOcrClass, IEquatable<OcrPower?>, IComparable<OcrPower>
    {
        public ulong? Value { get; set; }

        public OcrPower() : base() { }
        public OcrPower(XmlNode? xml, SSTypeAnalyzer file) : base(xml, file)
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
            return Equals(obj as OcrPower);
        }

        public bool Equals(OcrPower? other)
        {
            return other is not null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public int CompareTo(OcrPower? other)
        {
            if (other != null && other.Value != null && Value != null)
                return Value.Value.CompareTo(other.Value.Value);
            return 0;
        }

        public static bool operator ==(OcrPower? left, OcrPower? right)
        {
            return EqualityComparer<OcrPower>.Default.Equals(left, right);
        }

        public static bool operator !=(OcrPower? left, OcrPower? right)
        {
            return !(left == right);
        }

        public static OcrPower? FromAllianceList(string xml, SSTypeAnalyzer file)
        {
            XmlDocument xdoc = new();
            xdoc.LoadXml(xml);
            var nodes = xdoc.SelectNodes("//String");
            if (nodes != null && nodes.Count > 0)
            {
                return new OcrPower(nodes[0], file)
                {
                    ScannerXml = xml,
                };
            }

            return null;
        }
    }
}