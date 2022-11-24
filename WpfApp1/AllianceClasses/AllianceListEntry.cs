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
using System.Windows.Threading;
using System.Xml;

namespace STFC_EventLogger.AllianceClasses
{
    public class AllianceListEntry : IEquatable<AllianceListEntry?>
    {
        #region #- Private Fields -#

        #endregion

        #region #- Constructor -#

        public AllianceListEntry(string nameXML, string powerXML, SSTypeAnalyzer file)
        {
            //Name = new OcrName(name.SelectNodes("./TextLine[2]/String"), file);
            //Rank = new OcrRank(name.SelectSingleNode("./TextLine[1]/String"), file);
            //Level = new OcrLevel(name.SelectSingleNode("./TextLine[2]/String[1]"), file);
            //Power = new OcrPower(power, file);

            ImageType = file.ImageType;
            FileName = file.FileName;
        }

        #endregion

        #region #- Public Properties -#

        public OcrName Name { get; set; }
        public OcrRank Rank { get; set; }
        public OcrLevel Level { get; set; }
        public OcrPower Power { get; set; }

        public string FileName { get; set; }
        public ImageTypes ImageType { get; set; }

        #endregion

        #region #- Events -#

        #endregion

        #region #- Instance Methods -#

        #endregion

        #region #- Static Methods -#

        #endregion

        #region #- Interface/Overridden Methods -#

        public override bool Equals(object? obj)
        {
            return Equals(obj as AllianceListEntry);
        }
        public bool Equals(AllianceListEntry? other)
        {
            return other is not null &&
                   EqualityComparer<OcrName>.Default.Equals(Name, other.Name);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
        public override string? ToString()
        {
            return $"{Name.Value} / {Rank.Value} / {Level.Value} / {Power.Value}";
        }

        #endregion

        #region #- Operators -#

        public static bool operator ==(AllianceListEntry? left, AllianceListEntry? right)
        {
            return EqualityComparer<AllianceListEntry>.Default.Equals(left, right);
        }
        public static bool operator !=(AllianceListEntry? left, AllianceListEntry? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
