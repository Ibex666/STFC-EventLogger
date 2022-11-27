using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace STFC_EventLogger
{
    public class DataRow
    {
        public DataRow()
        {
            Data = new();
        }
        public DataRow(Rect rect1, Rect rect2, Rect rect3)
        {
            Rect1 = rect1;
            Rect2 = rect2;
            Rect3 = rect3;
            Data = new();
        }

        /// <summary>
        /// Total
        /// </summary>
        public Rect Rect1 { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public Rect Rect2 { get; set; }

        /// <summary>
        /// Power / Score
        /// </summary>
        public Rect Rect3 { get; set; }

        /// <summary>
        /// Image from Name
        /// </summary>
        public string Rect2Image { get; set; }

        /// <summary>
        /// Image from Power / Score
        /// </summary>
        public string Rect3Image { get; set; }


        public List<DataRowEntry> Data { get; set; }

        public Rect TotalRect => Rect1;
        public Rect NameRect => Rect2;
        public Rect ScoreRect => Rect3;
        public Rect PowerRect => Rect3;


        public void AddData(string xml1, string xml2, ScanMethods scanMethod)
        {
            Data.Add(new DataRowEntry(xml1, xml2, scanMethod));
        }
    }

    public class DataRowEntry
    {
        public DataRowEntry(string xml1, string xml2, ScanMethods scanMethod)
        {
            Xml1 = xml1;
            Xml2 = xml2;
            ScanMethod = scanMethod;
        }

        public string Xml1 { get; set; }
        public string Xml2 { get; set; }
        public ScanMethods ScanMethod { get; set; }
    }
}
