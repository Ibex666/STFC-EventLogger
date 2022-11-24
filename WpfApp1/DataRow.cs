using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace STFC_EventLogger
{
    public abstract class DataRow
    {
        protected DataRow(Rect rect1, Rect rect2, Rect rect3)
        {
            Rect1 = rect1;
            Rect2 = rect2;
            Rect3 = rect3;
            Data = new();
        }

        protected virtual Rect Rect1 { get; set; }
        protected virtual Rect Rect2 { get; set; }
        protected virtual Rect Rect3 { get; set; }
        protected virtual List<DataRowEntry> Data { get; set; }

        public void AddData(string? xml1, string? xml2, ScanMethods scanMethod)
        {
            Data.Add(new DataRowEntry(xml1, xml2, scanMethod));
        }
    }


    public class EventListDataRow : DataRow
    {
        public EventListDataRow(Rect total, Rect name, Rect score) : base(total, name, score)
        {
        }

        public Rect TotalRect => Rect1;
        public Rect NameRect => Rect2;
        public Rect ScoreRect => Rect3;
    }

    public class AllianceListDataRow : DataRow
    {
        public AllianceListDataRow(Rect total, Rect name, Rect power) : base(total, name, power)
        {
        }

        public Rect TotalRect => Rect1;
        public Rect NameRect => Rect2;
        public Rect PowerRect => Rect3;
    }


    public class DataRowEntry
    {
        public DataRowEntry(string? xml1, string? xml2, ScanMethods scanMethod)
        {
            Xml1 = xml1;
            Xml2 = xml2;
            ScanMethod = scanMethod;
        }

        public string? Xml1 { get; set; }
        public string? Xml2 { get; set; }
        public ScanMethods ScanMethod { get; set; }
    }
}
