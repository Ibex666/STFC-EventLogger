using System;
using System.Collections.Generic;

namespace STFC_EventLogger
{
    public class MemberAdministrationAlias : IEquatable<MemberAdministrationAlias?>, IComparable<MemberAdministrationAlias>
    {
        #region #- Private Fields -#



        #endregion

        #region #- Constructor -#

        public MemberAdministrationAlias()
        {
            Name = string.Empty;
            AKA = new();
            OcrGarbage = new();
        }

        #endregion

        #region #- Public Properties -#

        public string Name { get; set; }
        public List<StringWrapper> AKA { get; set; }
        public List<StringWrapper> OcrGarbage { get; set; }

        #endregion

        #region #- Events -#



        #endregion

        #region #- Instance Methods -#



        #endregion

        #region #- Static Methods -#



        #endregion

        #region #- Interface/Overridden Methods -#

        public int CompareTo(MemberAdministrationAlias? other)
        {
            if (other == null) return 1;
            return Name.CompareTo(other.Name);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as MemberAdministrationAlias);
        }
        public bool Equals(MemberAdministrationAlias? other)
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

        public static bool operator ==(MemberAdministrationAlias? left, MemberAdministrationAlias? right)
        {
            return EqualityComparer<MemberAdministrationAlias>.Default.Equals(left, right);
        }
        public static bool operator !=(MemberAdministrationAlias? left, MemberAdministrationAlias? right)
        {
            return !(left == right);
        }

        #endregion

    }
}

