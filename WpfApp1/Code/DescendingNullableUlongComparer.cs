using System.Collections.Generic;

namespace STFC_EventLogger
{
    internal class DescendingNullableUlongComparer : IComparer<ulong?>
    {
        public int Compare(ulong? x, ulong? y)
        {
            if (x.HasValue & y.HasValue)
            {
                if (x == y) return 0;
                if (x < y) return 1;
                if (x > y) return -1;
            }
            if (x.HasValue & !y.HasValue) return -1;
            if (!x.HasValue & y.HasValue) return 1;
            if (!x.HasValue & !y.HasValue) return 0;

            return 0;
        }
    }
}