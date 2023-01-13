using System;

namespace STFC_EventLogger
{
    public class StringWrapper : IComparable<StringWrapper>, IEquatable<StringWrapper?>
    {

        #region #- Private Fields -#



        #endregion

        #region #- Constructor -#

        public StringWrapper()
        {
            Text = string.Empty;
        }
        public StringWrapper(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        #endregion

        #region #- Public Properties -#

        public string Text { get; set; }

        #endregion

        #region #- Events -#



        #endregion

        #region #- Instance Methods -#



        #endregion

        #region #- Static Methods -#



        #endregion

        #region #- Interface/Overridden Methods -#

        public int CompareTo(StringWrapper? other)
        {
            if (other is null) return 1;
            return Text.CompareTo(other.Text);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as StringWrapper);
        }
        public bool Equals(StringWrapper? other)
        {
            return other is not null &&
                   Text == other.Text;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }
        public override string ToString()
        {
            return Text;
        }

        #endregion

        #region #- Operators -#

        public static bool operator ==(StringWrapper left, StringWrapper right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }
        public static bool operator !=(StringWrapper left, StringWrapper right)
        {
            return !(left == right);
        }
        public static implicit operator StringWrapper(string text)
        {
            return new StringWrapper(text);
        }
        public static implicit operator string(StringWrapper sw)
        {
            return sw.Text;
        }

        #endregion
    }
}

