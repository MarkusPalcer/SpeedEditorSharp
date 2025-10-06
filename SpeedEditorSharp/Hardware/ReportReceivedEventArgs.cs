namespace SpeedEditorSharp.Hardware;

public class ReportReceivedEventArgs : EventArgs
{
    public Reports.Report Report { get; init;  }
}