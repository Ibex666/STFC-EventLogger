using System.Collections.Generic;

namespace STFC_EventLogger.AllianceClasses
{
    internal class AllianceMemberScoreComparer : IComparer<AllianceMember>
    {
        public int Compare(AllianceMember? x, AllianceMember? y)
        {
            if (x == null || y == null) return 0;
            if (x.BestScore == y.BestScore) return 0;
            if (x.BestScore < y.BestScore) return 1;
            if (x.BestScore > y.BestScore) return -1;

            return 0;
        }
    }
}