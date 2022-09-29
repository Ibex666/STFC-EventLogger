using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    internal class OcrName : BaseOcrClass, IEquatable<OcrName?>, IComparable<OcrName>
    {
        public string? Value { get; set; }

        public OcrName() : base() { }
        public OcrName(XmlNodeList? xml, string fileName) : base()
        {
            if (xml == null)
                return;
            FileName = fileName;

            X = int.MaxValue;
            Y = int.MaxValue;
            Height = 0;
            int _hpos_max = 0;

            StringBuilder _sb = new();
            float _wc = 0;
            foreach (XmlElement node in xml)
            {
                int _HPos = int.Parse(node.GetAttribute("HPOS"));
                if (_HPos < X)
                    X = _HPos;
                if (_HPos > _hpos_max)
                {
                    _hpos_max = _HPos;
                    Width = int.Parse(node.GetAttribute("WIDTH"));
                }

                int _VPos = int.Parse(node.GetAttribute("VPOS"));
                if (_VPos < Y)
                    Y = _VPos;


                int _Height = int.Parse(node.GetAttribute("HEIGHT"));
                if (_Height > Height)
                    Height = _Height;

                _wc += float.Parse(node.GetAttribute("WC").Replace(".", ","));
                _sb.Append(node.GetAttribute("CONTENT"));
            }

            Width += _hpos_max - X;
            WC = (float)Math.Round(_wc / xml.Count, 2);
            Content = _sb.ToString();
            Value = Content;

            var a = V.us.Aliase.FirstOrDefault(item => item.Value.Contains(Content));
            if (a.Key != null)
            {
                Value = a.Key;
                WC = 1;
            }
        }

        public override string? ToString()
        {
            return $"{Value} / {Content} / {WC}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as OcrName);
        }

        public bool Equals(OcrName? other)
        {
            return other is not null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public int CompareTo(OcrName? other)
        {
            if (other != null && other.Value != null && Value != null)
                return Value.CompareTo(other.Value);
            return 0;
        }

        public static bool operator ==(OcrName? left, OcrName? right)
        {
            return EqualityComparer<OcrName>.Default.Equals(left, right);
        }

        public static bool operator !=(OcrName? left, OcrName? right)
        {
            return !(left == right);
        }
    }
}