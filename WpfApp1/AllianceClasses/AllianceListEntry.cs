using STFC_EventLogger.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace STFC_EventLogger.AllianceClasses
{
    public class AllianceListEntry
    {
        #region #- Private Fields -#

        private readonly SSTypeAnalyzer _file;

        #endregion

        #region #- Constructor -#

        public AllianceListEntry(SSTypeAnalyzer file)
        {
            Data = new();

            FileName = file.FileName;
            ImageType = file.ImageType;

            _file = file;
        }

        #endregion

        #region #- Public Properties -#

        public string FileName { get; set; }
        public ImageTypes ImageType { get; set; }

        public List<AllianceListEntryData> Data { get; set; }

        #endregion

        #region #- Events -#

        #endregion

        #region #- Instance Methods -#

        public void AddData(AllianceListEntryData data)
        {
            Data.Add(data);
        }
        public void AddData(string nameXML, string powerXML, ScanMethods scanMethods)
        {
            Data.Add(new AllianceListEntryData(nameXML, powerXML, scanMethods));
        }

        #endregion

        #region #- Static Methods -#

        #endregion

        #region #- Interface/Overridden Methods -#


        #endregion

        #region #- Operators -#

        #endregion
    }

    public class AllianceListEntryData
    {
        public AllianceListEntryData(string? nameXML, string? powerXML, ScanMethods scanMethods)
        {
            NameXML = nameXML;
            PowerXML = powerXML;
            ScanMethods = scanMethods;
        }

        public string? NameXML { get; set; }
        public string? PowerXML { get; set; }
        public ScanMethods ScanMethods { get; set; }
    }
}
