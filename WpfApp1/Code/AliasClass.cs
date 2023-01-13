using System;
using System.Collections.Generic;

namespace STFC_EventLogger
{
    [Obsolete("only for compability, will be removed later")]
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
            if (other == null) return 1;
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

