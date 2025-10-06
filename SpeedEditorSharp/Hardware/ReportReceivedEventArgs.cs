namespace SpeedEditorSharp.Hardware;

internal class ReportReceivedEventArgs : EventArgs
{
    public required Reports.Report Report { get; init; }
}