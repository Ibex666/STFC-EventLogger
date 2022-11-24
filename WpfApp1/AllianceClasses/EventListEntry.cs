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
    public class EventListEntry : IEquatable<EventListEntry?>
    {
        #region #- Private Fields -#

        private readonly string _nameXML;
        private readonly string _scoreXML;

        #endregion

        #region #- Constructor -#

        public EventListEntry(string nameXML, string scoreXML, SSTypeAnalyzer file)
        {
            //Name = new OcrName(name.SelectNodes("./TextLine[2]/String"), file);
            //Score = new(score, file);

            Name = new();
            Score = new();


            ImageType = file.ImageType;
            FileName = file.FileName;

            _nameXML = nameXML;
            _scoreXML = scoreXML;
        }

        #endregion

        #region #- Public Properties -#

        public OcrName Name { get; set; }
        public OcrScore Score { get; set; }

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
            return Equals(obj as EventListEntry);
        }
        public bool Equals(EventListEntry? other)
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
            return $"{Name.Value} / {Score.Value}";
        }

        #endregion

        #region #- Operators -#

        public static bool operator ==(EventListEntry? left, EventListEntry? right)
        {
            return EqualityComparer<EventListEntry>.Default.Equals(left, right);
        }
        public static bool operator !=(EventListEntry? left, EventListEntry? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
