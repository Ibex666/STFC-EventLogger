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
using System.Xml.Linq;

namespace STFC_EventLogger.AllianceClasses
{
    public class AllianceListEntry
    {
        #region #- Private Fields -#

        #endregion

        #region #- Constructor -#

        private AllianceListEntry(DataRow dataRow, SSTypeAnalyzer file)
        {
            File = file;
            Names = new();
            Levels = new();
            Powers = new();

            NameImage = ImageFunctions.BitmapImageFromFile(dataRow.Rect2Image);
            PowerImage = ImageFunctions.BitmapImageFromFile(dataRow.Rect3Image);

            dataRow.Data.ForEach((d) =>
            {
                var n = OcrName.FromAllianceList(d.Xml1, file);
                if (n != null)
                    Names.Add(n);

                var l = OcrLevel.FromAllianceList(d.Xml1, file);
                if (l != null)
                    Levels.Add(l);

                var p = OcrPower.FromAllianceList(d.Xml2, file);
                if (p != null)
                    Powers.Add(p);
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
        public List<OcrLevel> Levels { get; set; }
        public List<OcrPower> Powers { get; set; }

        public BitmapImage NameImage { get; set; }
        public BitmapImage PowerImage { get; set; }

        #endregion

        #region #- Events -#

        #endregion

        #region #- Instance Methods -#

        #endregion

        #region #- Static Methods -#

        public static AllianceListEntry? FromSsTypeAnalyzer(DataRow dataRow, SSTypeAnalyzer file)
        {
            if (file.PageType != PageTypes.MemberList)
                return null;

            var ret = new AllianceListEntry(dataRow, file);

            if (ret.Names.All(_ => string.IsNullOrWhiteSpace(_.Content)) |
                ret.Powers.All(_ => string.IsNullOrWhiteSpace(_.Content)))
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
