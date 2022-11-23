using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Windows.Xps.Packaging;
using System.Xml;
using Tesseract;

namespace STFC_EventLogger
{
    public class SSTypeAnalyzer : IEquatable<SSTypeAnalyzer?>
    {
        public SSTypeAnalyzer(string file, ImageTypes imageType)
        {
            FileName = file;
            ImageType = imageType;
            EventListDataRows = new List<Rect>();
            AllianceListDataRows = new List<Rect>();
        }

        public string FileName { get; set; }
        public PageTypes PageType { get; set; }
        public ImageTypes ImageType { get; set; }
        public List<Rect> EventListDataRows { get; set; }
        public List<Rect> AllianceListDataRows { get; set; }

        public void Analyze()
        {
            using var image = Pix.LoadFromFile(FileName);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tessdata"), "eng", EngineMode.TesseractOnly);
            using var page = engine.Process(image, V.allianceLeaderBoard.SelectedUserConfig.RectSsTypeAnalyzer);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            var nodes = xdoc.SelectNodes("//@CONTENT");
            if (nodes != null && nodes.Count > 0 && nodes[0].Value != null && nodes[0].Value != null)
            {
                switch (nodes[0].Value.ToLower())
                {
                    case "members":
                        PageType = PageTypes.MemberList;
                        GetAllianceListRects(image);
                        break;
                    default:
                        PageType = PageTypes.EventList;
                        GetEventListRects(image);
                        break;
                }
            }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
        }

        private void GetEventListRects(Pix image)
        {
            F.GetEngineModeData(ScanMethods.Fast, out string tessdata, out EngineMode engineMode);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
            using var page = engine.Process(image, V.allianceLeaderBoard.SelectedUserConfig.RectEventScoresAnalyzer);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
            var nodes = xdoc.SelectNodes("//ComposedBlock");
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    int x = V.allianceLeaderBoard.SelectedUserConfig.RectEventNames.X;
                    int y = int.Parse(node.GetAttribute("VPOS"));
                    int w = V.allianceLeaderBoard.SelectedUserConfig.RectEventScores.X2 - x;
                    int h = int.Parse(node.GetAttribute("HEIGHT"));

                    int d = (int)(h * 1.5);
                    y -= d;
                    h += (d * 2);

                    var r = new Rect(x, y, w, h);
                    EventListDataRows.Add(r);
                }
            }
        }
        private void GetAllianceListRects(Pix image)
        {
            F.GetEngineModeData(ScanMethods.Fast, out string tessdata, out EngineMode engineMode);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
            using var page = engine.Process(image, V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePowerAnalyzer);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
            var nodes = xdoc.SelectNodes("//ComposedBlock");
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    int x = V.allianceLeaderBoard.SelectedUserConfig.RectAllianceNames.X;
                    int y = int.Parse(node.GetAttribute("VPOS"));
                    int w = V.allianceLeaderBoard.SelectedUserConfig.RectAlliancePower.X2 - x;
                    int h = int.Parse(node.GetAttribute("HEIGHT"));

                    y -= h;
                    h += (int)(h * 2.3);

                    var r = new Rect(x, y, w, h);
                    AllianceListDataRows.Add(r);
                }
            }
        }

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
            return $"{FileName}, {PageType}, {ImageType}";
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
