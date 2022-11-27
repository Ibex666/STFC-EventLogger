using STFC_EventLogger.MVVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace STFC_EventLogger.AllianceClasses
{
    public class OcrName : BaseOcrClass, IEquatable<OcrName?>, IComparable<OcrName>
    {
        private const float MinClosestNameAccuracy = 0.65f;

        public string? Value { get; set; }
        public List<LevensteinNameDistance>? ClosestNames { get; set; }

        public OcrName() : base() { }
        public OcrName(XmlNodeList? xml, SSTypeAnalyzer file) : base()
        {
            if (xml == null || xml.Count == 0)
                return;
            FileName = file.FileName;
            ImageType = file.ImageType;
            Recognised = false;

            X = int.MaxValue;
            Y = int.MaxValue;
            Height = 0;
            int _hpos_max = 0;

            StringBuilder _sb = new();
            float _wc = 0;

            for (int i = xml.Count == 1 ? 0 : 1; i < xml.Count; i++)
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var attr = xml[i].Attributes;
                if (attr == null)
                    continue;

                int _HPos = int.Parse(attr["HPOS"].Value);

                if (_HPos < X)
                    X = _HPos;
                if (_HPos > _hpos_max)
                {
                    _hpos_max = _HPos;
                    Width = int.Parse(attr["WIDTH"].Value);
                }

                int _VPos = int.Parse(attr["VPOS"].Value);
                if (_VPos < Y)
                    Y = _VPos;


                int _Height = int.Parse(attr["HEIGHT"].Value);
                if (_Height > Height)
                    Height = _Height;

                _wc += float.Parse(attr["WC"].Value.Replace(".", ","));
                _sb.Append(attr["CONTENT"].Value);
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
            }

            Width += _hpos_max - X;
            WC = (float)Math.Round(_wc / xml.Count, 2);
            Content = _sb.ToString();
            Value = Content;

            var a = V.NameDicts.FirstOrDefault(item => item.Value.Contains(Content, StringComparer.OrdinalIgnoreCase));
            if (a.Key != null)
            {
                Value = a.Key;
                Recognised = true;
                WC = 1;
            }
            else
            {
                List<int> distances = new();
                List<LevensteinNameDistance> nameDistances = new();
                foreach (var item in V.NameDicts)
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
                    if (ClosestNames[0].Accuracy >= MinClosestNameAccuracy)
                    {
                        Value = ClosestNames[0].Name;
                        WC = ClosestNames[0].Accuracy;
                        Recognised = true;
                    }
                }
                else if (ClosestNames.Count > 1)
                {
                    if (ClosestNames.All(_ => _.Name == ClosestNames[0].Name))
                    {
                        if (ClosestNames.Any(_ => _.Accuracy >= MinClosestNameAccuracy))
                        {
                            Value = ClosestNames[0].Name;
                            WC = ClosestNames[0].Accuracy;
                            Recognised = true;
                        }
                    }
                }
            }
        }
        public OcrName(List<XmlNode> xml, SSTypeAnalyzer file) : base()
        {
            if (xml == null || xml.Count == 0)
                return;
            FileName = file.FileName;
            ImageType = file.ImageType;
            Recognised = false;

            X = int.MaxValue;
            Y = int.MaxValue;
            Height = 0;
            int _hpos_max = 0;

            StringBuilder _sb = new();
            float _wc = 0;

            for (int i = xml.Count == 1 ? 0 : 1; i < xml.Count; i++)
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var attr = xml[i].Attributes;
                if (attr == null)
                    continue;

                int _HPos = int.Parse(attr["HPOS"].Value);

                if (_HPos < X)
                    X = _HPos;
                if (_HPos > _hpos_max)
                {
                    _hpos_max = _HPos;
                    Width = int.Parse(attr["WIDTH"].Value);
                }

                int _VPos = int.Parse(attr["VPOS"].Value);
                if (_VPos < Y)
                    Y = _VPos;


                int _Height = int.Parse(attr["HEIGHT"].Value);
                if (_Height > Height)
                    Height = _Height;

                _wc += float.Parse(attr["WC"].Value.Replace(".", ","));
                _sb.Append(attr["CONTENT"].Value);
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
            }

            Width += _hpos_max - X;
            WC = (float)Math.Round(_wc / xml.Count, 2);
            Content = _sb.ToString();
            Value = Content;

            var a = V.NameDicts.FirstOrDefault(item => item.Value.Contains(Content, StringComparer.OrdinalIgnoreCase));
            if (a.Key != null)
            {
                Value = a.Key;
                Recognised = true;
                WC = 1;
            }
            else
            {
                List<int> distances = new();
                List<LevensteinNameDistance> nameDistances = new();
                foreach (var item in V.NameDicts)
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
                    if (ClosestNames[0].Accuracy >= MinClosestNameAccuracy)
                    {
                        Value = ClosestNames[0].Name;
                        WC = ClosestNames[0].Accuracy;
                        Recognised = true;
                    }
                }
                else if (ClosestNames.Count > 1)
                {
                    if (ClosestNames.All(_ => _.Name == ClosestNames[0].Name))
                    {
                        if (ClosestNames.Any(_ => _.Accuracy >= MinClosestNameAccuracy))
                        {
                            Value = ClosestNames[0].Name;
                            WC = ClosestNames[0].Accuracy;
                            Recognised = true;
                        }
                    }
                }
            }
        }

        public override string? ToString()
        {
            return $"{Value} / {Content} / {WC} / {Recognised}";
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

        public static OcrName? FromAllianceList(string xml, SSTypeAnalyzer file)
        {
            XmlDocument xdoc = new();
            xdoc.LoadXml(xml);
            var nodes = xdoc.SelectNodes("//String");
            if (nodes != null)
            {
                return new OcrName(nodes, file)
                {
                    ScannerXml = xml,
                };
            }

            return null;
        }
        public static OcrName? FromEventList(string xml, SSTypeAnalyzer file)
        {
            XmlDocument xdoc = new();
            xdoc.LoadXml(xml);
            var nodes = xdoc.SelectNodes("//String");
            if (nodes != null)
            {
                return new OcrName(nodes, file)
                {
                    ScannerXml = xml,
                };
            }

            return null;
        }
    }
}