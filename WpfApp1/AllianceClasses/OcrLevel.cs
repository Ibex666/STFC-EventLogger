using System;
using System.Collections.Generic;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class OcrLevel : BaseOcrClass, IEquatable<OcrLevel?>
    {
        public uint? Value { get; set; }

        public OcrLevel() : base() { }
        public OcrLevel(XmlNode? xml, SSTypeAnalyzer file) : base(xml, file)
        {
            if (Content != null)
            {
                Recognised = uint.TryParse(CleanContentNumberString(Content), out uint tmp);
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

        public static OcrLevel? FromAllianceList(string xml, SSTypeAnalyzer file)
        {
            XmlDocument xdoc = new();
            xdoc.LoadXml(xml);
            var nodes = xdoc.SelectSingleNode("//String[1]");
            if (nodes != null)
            {
                return new OcrLevel(nodes, file)
                {
                    ScannerXml = xml,
                };
            }

            return null;
        }
    }
}