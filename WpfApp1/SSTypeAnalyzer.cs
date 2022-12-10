using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Xps.Packaging;
using System.Xml;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace STFC_EventLogger
{
    public class SSTypeAnalyzer : IEquatable<SSTypeAnalyzer?>
    {
        public SSTypeAnalyzer(string file, ImageTypes imageType)
        {
            FileName = file;
            ImageType = imageType;
            DataRows = new();
        }

        public string FileName { get; set; }
        public PageTypes PageType { get; set; }
        public ImageTypes ImageType { get; set; }
        public List<DataRow> DataRows { get; set; }

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
            var config = V.allianceLeaderBoard.SelectedUserConfig;
            var scanRect = V.allianceLeaderBoard.SelectedUserConfig.EventListAnalyzerRect;
            var lbp = config.EventListBP;

            F.GetEngineModeData(ScanMethods.Fast, out string tessdata, out EngineMode engineMode);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
            using var page = engine.Process(image, scanRect);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
            var nodes = xdoc.SelectNodes("//TextLine");
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    int y = int.Parse(node.GetAttribute("VPOS"));
                    int h = int.Parse(node.GetAttribute("HEIGHT"));

                    int d = (int)(h * 1.5);
                    y -= d;
                    h += (d * 2);

                    var dr = new DataRow()
                    {
                        Rect1 = new Rect(lbp.X1, y, lbp.Width, h),
                        Rect2 = new Rect(lbp.X1, y, lbp.WidthX1X2, h),
                        Rect3 = new Rect(lbp.X3, y, lbp.WidthX3X4, h)
                    };

                    if (dr.TotalRect.Y1 < (config.EventListAnalyzerRect.Y1 - 5) || dr.TotalRect.Y2 > config.EventListAnalyzerRect.Y2)
                        continue;

                    dr.Rect2Image = ImageFunctions.CropImage(FileName, dr.Rect2, ImageFormat.Png);
                    dr.Rect3Image = ImageFunctions.CropImage(FileName, dr.Rect3, ImageFormat.Png);

                    DataRows.Add(dr);
                }
            }

            // check if Datarows are complete
            // check if first/last Datarow is really the first/last or if something is missing
            var maxh = DataRows.Select(_ => _.TotalRect.Height).Max();
            var minY1 = DataRows.Select(_ => _.TotalRect.Y1).Min();
            var maxY2 = DataRows.Select(_ => _.TotalRect.Y2).Max();
            var minhb = int.MaxValue;
            for (int i = 0; i < DataRows.Count - 1; i++)
            {
                var hb = (DataRows[i + 1].TotalRect.Y1 - DataRows[i].TotalRect.Y2);
                if (hb < minhb)
                {
                    minhb = hb;
                }
            }


            if (!((minY1 - maxh) < scanRect.Y1))
            {
                // first row/s is/are missing

            }

            if (!((maxY2 + maxh) > scanRect.Y2))
            {
                // last row/s is/are missing

            }

            DataRows.Sort((x, y) => x.TotalRect.Y1.CompareTo(y.TotalRect.Y1));


        CheckRows:
            // check if a row is missing in between
            for (int i = 0; i < DataRows.Count - 1; i++)
            {
                if (!((DataRows[i + 1].TotalRect.Y1 - DataRows[i].TotalRect.Y2) < maxh))
                {
                    // missing a row here :-(
                    var dr = new DataRow()
                    {
                        Rect1 = new Rect(lbp.X1, DataRows[i].TotalRect.Y2 + minhb, lbp.Width, maxh),
                        Rect2 = new Rect(lbp.X1, DataRows[i].TotalRect.Y2 + minhb, lbp.WidthX1X2, maxh),
                        Rect3 = new Rect(lbp.X3, DataRows[i].TotalRect.Y2 + minhb, lbp.WidthX3X4, maxh)
                    };

                    if (dr.TotalRect.Y1 < (config.EventListAnalyzerRect.Y1 - 5) || dr.TotalRect.Y2 > config.EventListAnalyzerRect.Y2)
                        continue;

                    dr.Rect2Image = ImageFunctions.CropImage(FileName, dr.Rect2, ImageFormat.Png);
                    dr.Rect3Image = ImageFunctions.CropImage(FileName, dr.Rect3, ImageFormat.Png);

                    DataRows.Insert(i + 1, dr);
                    goto CheckRows;
                }
            }
        }
        private void GetAllianceListRects(Pix image)
        {
            var config = V.allianceLeaderBoard.SelectedUserConfig;
            var lbp = config.AllianceListBP;

            F.GetEngineModeData(ScanMethods.Fast, out string tessdata, out EngineMode engineMode);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tessdata), "eng", engineMode);
            using var page = engine.Process(image, V.allianceLeaderBoard.SelectedUserConfig.AllianceListAnalyzerRect);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
            var nodes = xdoc.SelectNodes("//TextLine");
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    int y = int.Parse(node.GetAttribute("VPOS"));
                    int h = int.Parse(node.GetAttribute("HEIGHT"));

                    int y1 = y + (h / 2);
                    int h1 = h + (h / 2);

                    y -= h;
                    h += (int)(h * 2.3);

                    var dr = new DataRow()
                    {
                        Rect1 = new Rect(lbp.X1, y, lbp.Width, h),
                        Rect2 = new Rect(lbp.X1, y1, lbp.WidthX1X2, h1),
                        Rect3 = new Rect(lbp.X3, y, lbp.WidthX3X4, h)
                    };

                    if (dr.TotalRect.Y1 < (config.AllianceListAnalyzerRect.Y1 - 5) || dr.TotalRect.Y2 > config.AllianceListAnalyzerRect.Y2)
                        continue;

                    dr.Rect2Image = ImageFunctions.CropImage(FileName, dr.Rect2, ImageFormat.Png);
                    dr.Rect3Image = ImageFunctions.CropImage(FileName, dr.Rect3, ImageFormat.Png);

                    DataRows.Add(dr);
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
