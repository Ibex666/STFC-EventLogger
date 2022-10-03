﻿namespace STFC_EventLogger.AllianceClasses
{
    public class LevensteinNameDistance
    {
        public LevensteinNameDistance(string name, string scanContent, int distance)
        {
            Name = name;
            ScanContent = scanContent;
            Distance = distance;

            Accuracy = 1 - ((float)distance / scanContent.Length);
        }

        public int Distance { get; set; }
        public string ScanContent { get; set; }
        public string Name { get; set; }
        public float Accuracy { get; set; }

    }
}