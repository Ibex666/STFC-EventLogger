using System.Collections.Generic;

namespace STFC_EventLogger.AllianceClasses
{
    internal class AllianceMemberPowerComparer : IComparer<AllianceMember>
    {
        public int Compare(AllianceMember? x, AllianceMember? y)
        {
            if (x == null || y == null) return 0;
            if (x.BestPower == y.BestPower) return 0;
            if (x.BestPower < y.BestPower) return 1;
            if (x.BestPower > y.BestPower) return -1;

            return 0;
        }
    }
}