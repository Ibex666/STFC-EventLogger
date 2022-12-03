using STFC_EventLogger.AllianceClasses;
using STFC_EventLogger.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace STFC_EventLogger
{
    public class AliasClass : IEquatable<AliasClass?>, IComparable<AliasClass>
    {
        #region #- Private Fields -#



        #endregion

        #region #- Constructor -#

        public AliasClass()
        {
            Name = string.Empty;
            AKA = new();
        }

        #endregion

        #region #- Public Properties -#

        public string Name { get; set; }
        public List<string> AKA { get; set; }

        #endregion

        #region #- Events -#



        #endregion

        #region #- Instance Methods -#



        #endregion

        #region #- Static Methods -#



        #endregion

        #region #- Interface/Overridden Methods -#

        public int CompareTo(AliasClass? other)
        {
            return Name.CompareTo(other.Name);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as AliasClass);
        }
        public bool Equals(AliasClass? other)
        {
            return other is not null &&
                   Name == other.Name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, AKA);
        }
        public override string ToString()
        {
            return $"{Name}, Count: {AKA.Count}";
        }

        #endregion

        #region #- Operators -#

        public static bool operator ==(AliasClass? left, AliasClass? right)
        {
            return EqualityComparer<AliasClass>.Default.Equals(left, right);
        }
        public static bool operator !=(AliasClass? left, AliasClass? right)
        {
            return !(left == right);
        }

        #endregion

    }
}

