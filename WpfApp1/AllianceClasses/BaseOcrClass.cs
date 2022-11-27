using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public abstract class BaseOcrClass
    {
        #region #- Public Properties -#

        public string? Content { get; protected set; }
        public string ScannerXml { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int X1 => X;
        public int Y1 => Y;
        public int X2 => X + Width;
        public int Y2 => Y + Height;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public float WC { get; protected set; }
        public bool Recognised { get; protected set; }
        public BitmapImage? Image { get; set; }
        public string FileName { get; set; }
        public ImageTypes ImageType { get; set; }

        #endregion

        #region #- Constructor -#

        public BaseOcrClass()
        {
            FileName = string.Empty;
        }
        public BaseOcrClass(XmlNode? xml, SSTypeAnalyzer file)
        {
            if (xml == null)
            {
                ImageType = ImageTypes.Unknown;
                FileName = string.Empty;
                return;
            }
            XmlElement _xmlE = (XmlElement)xml;

            X = int.Parse(_xmlE.GetAttribute("HPOS"));
            Y = int.Parse(_xmlE.GetAttribute("VPOS"));
            Width = int.Parse(_xmlE.GetAttribute("WIDTH"));
            Height = int.Parse(_xmlE.GetAttribute("HEIGHT"));
            WC = float.Parse(_xmlE.GetAttribute("WC").Replace(".", ","));
            Content = _xmlE.GetAttribute("CONTENT");
            FileName = file.FileName;
            ImageType = file.ImageType;
        }

        #endregion

        #region #- Methods -#

        protected static string CleanContentNumberString(string _content)
        {
            _content = _content.Replace(".", "");
            _content = _content.Replace(",", "");
            _content = _content.Replace("'", "");
            _content = _content.Replace("`", "");
            _content = _content.Replace("´", "");
            _content = _content.Replace("»", "");

            _content = _content.Replace("l+", "4");
            _content = _content.Replace("1+", "4");
            _content = _content.Replace("£+", "4");
            _content = _content.Replace("#", "4");
            _content = _content.Replace("l", "1");
            _content = _content.Replace("W", "14");
            _content = _content.Replace("N", "11");

            return _content;
        }

        #endregion
    }
}