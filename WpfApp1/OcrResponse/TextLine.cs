using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace STFC_EventLogger.OcrResponse
{

    internal class TextLine
    {
        #region #- Public Properties -#

        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int X1 => X;
        public int Y1 => Y;
        public int X2 => X + Width;
        public int Y2 => Y + Height;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public XmlNodeList? XmlStrings { get; protected set; }

        #endregion

        #region #- Constructor -#

        public TextLine(XmlElement xml)
        {
            if (xml == null)
            {
                return;
            }

            X = int.Parse(xml.GetAttribute("HPOS"));
            Y = int.Parse(xml.GetAttribute("VPOS"));
            Width = int.Parse(xml.GetAttribute("WIDTH"));
            Height = int.Parse(xml.GetAttribute("HEIGHT"));

            XmlStrings = xml.SelectNodes(".//String");
        }

        #endregion

        #region #- Methods -#
        internal static List<TextLine> GetAllTextLines(XmlNodeList? nodes)
        {
            List<TextLine> result = new List<TextLine>();

            if (nodes == null)
                return result;

            foreach (XmlElement node in nodes)
            {
                result.Add(new TextLine(node));
            }

            return result;
        }
        #endregion
    }

}
