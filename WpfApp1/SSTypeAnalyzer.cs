using System;
using System.Collections.Generic;
using System.Xml;
using Tesseract;

namespace STFC_EventLogger
{
    internal class SSTypeAnalyzer : IEquatable<SSTypeAnalyzer?>
    {
        public SSTypeAnalyzer(string file)
        {
            FileName = file;

            using (var image = Pix.LoadFromFile(file))
            {
                using (var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tessdata"), "eng", EngineMode.TesseractOnly))
                {
                    using (var page = engine.Process(image, V.us.RectSsTypeAnalyzer))
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(page.GetAltoText(0));

                        var nodes = xdoc.SelectNodes("//@CONTENT");
                        if (nodes != null && nodes.Count > 0 && nodes[0].Value != null && nodes[0].Value != null)
                        {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                            switch (nodes[0].Value.ToLower())
                            {
                                case "members":
                                    PageType = PageTypes.MemberList;
                                    break;
                                default:
                                    PageType = PageTypes.EventList;
                                    break;
                            }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                        }

                    }
                }
            }


        }

        public string FileName { get; set; }
        public PageTypes PageType { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SSTypeAnalyzer);
        }

        public bool Equals(SSTypeAnalyzer? other)
        {
            return other is not null &&
                   FileName == other.FileName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FileName);
        }

        public override string? ToString()
        {
            return $"{FileName}, {PageType}";
        }

        public static bool operator ==(SSTypeAnalyzer? left, SSTypeAnalyzer? right)
        {
            return EqualityComparer<SSTypeAnalyzer>.Default.Equals(left, right);
        }

        public static bool operator !=(SSTypeAnalyzer? left, SSTypeAnalyzer? right)
        {
            return !(left == right);
        }
    }
}
