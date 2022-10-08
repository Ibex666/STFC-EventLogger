namespace STFC_EventLogger.MVVM
{
    public class ScanWorkerProgressReport
    {
        public ScanWorkerProgressReport(string message, ScanWorkerProgressReportMessageTypes messageType)
        {
            Message = message;
            MessageType = messageType;
        }

        public string Message { get; set; }
        public ScanWorkerProgressReportMessageTypes MessageType { get; set; }
    }
}

