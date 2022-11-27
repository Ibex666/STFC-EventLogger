using STFC_EventLogger.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class EventListEntry
    {
        #region #- Private Fields -#

        #endregion

        #region #- Constructor -#

        public EventListEntry(DataRow dataRow, SSTypeAnalyzer file)
        {
            File = file;
            Names = new();
            Scores = new();
            
            NameImage = ImageFunctions.BitmapImageFromFile(dataRow.Rect2Image);
            ScoreImage = ImageFunctions.BitmapImageFromFile(dataRow.Rect3Image);

            dataRow.Data.ForEach((d) =>
            {
                var n = OcrName.FromEventList(d.Xml1, file);
                if (n != null)
                    Names.Add(n);

                var s = OcrScore.FromEventList(d.Xml2, file);
                if (s != null)
                    Scores.Add(s);
            });

            Names.Sort((x, y) => y.Recognised.CompareTo(x.Recognised));
            Names.Sort((x, y) => y.WC.CompareTo(x.WC));

            if (Names.All(_ => _.Recognised == false))
            {

            }
            else
            {
                Name = Names[0];
            }
        }

        #endregion

        #region #- Public Properties -#

        public SSTypeAnalyzer File { get; private set; }

        public OcrName Name { get; set; }
        public List<OcrName> Names { get; set; }
        public List<OcrScore> Scores { get; set; }

        public BitmapImage NameImage { get; set; }
        public BitmapImage ScoreImage { get; set; }

        #endregion

        #region #- Events -#

        #endregion

        #region #- Instance Methods -#

        #endregion

        #region #- Static Methods -#

        public static EventListEntry? FromSsTypeAnalyzer(DataRow dataRow, SSTypeAnalyzer file)
        {
            if (file.PageType != PageTypes.EventList)
                return null;

            var ret = new EventListEntry(dataRow, file);

            if (ret.Names.All(_ => string.IsNullOrWhiteSpace(_.Content)) |
                ret.Scores.All(_ => string.IsNullOrWhiteSpace(_.Content)))
            {
                return null;
            }

            return ret;
        }

        #endregion

        #region #- Interface/Overridden Methods -#

        #endregion

        #region #- Operators -#

        #endregion
    }
}
