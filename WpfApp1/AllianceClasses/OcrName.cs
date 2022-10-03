using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class OcrName : BaseOcrClass, IEquatable<OcrName?>, IComparable<OcrName>
    {
        public string? Value { get; set; }
        public List<LevensteinNameDistance>? ClosestNames { get; set; }
        public bool RecognizedName { get; set; }

        public OcrName() : base() { }
        public OcrName(XmlNodeList? xml, SSTypeAnalyzer file) : base()
        {
            if (xml == null)
                return;
            FileName = file.FileName;
            ImageType = file.ImageType;
            RecognizedName = false;

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

            var a = V.Aliase.FirstOrDefault(item => item.Value.Contains(Content, StringComparer.OrdinalIgnoreCase));
            if (a.Key != null)
            {
                Value = a.Key;
                RecognizedName = true;
                WC = 1;
            }
            else
            {
                List<int> distances = new();
                List<LevensteinNameDistance> nameDistances = new();
                foreach (var item in V.Aliase)
                {
                    foreach (var v in item.Value)
                    {
                        int d = F.LevensteinDistance(Content, v);
                        nameDistances.Add(new LevensteinNameDistance(item.Key, Content, d));
                        distances.Add(d);
                    }
                }
                ClosestNames = nameDistances.Where(_ => _.Distance == distances.Min()).ToList();
                if (ClosestNames.Count == 1)
                {
                    Value = ClosestNames[0].Name;
                    WC = ClosestNames[0].Accuracy;
                    RecognizedName = true;
                }
                else if (ClosestNames.Count > 1)
                {
                    if (ClosestNames.All(_ => _.Name == ClosestNames[0].Name))
                    {
                        Value = ClosestNames[0].Name;
                        WC = ClosestNames[0].Accuracy;
                        RecognizedName = true;
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