using System;

namespace STFC_EventLogger
{
    public class StringWrapper : IComparable<StringWrapper>
    {
        public string Text { get; set; }

        public int CompareTo(StringWrapper? other)
        {
            if (other == null) return 1;
            return Text.CompareTo(other.Text);
        }
    }
}

