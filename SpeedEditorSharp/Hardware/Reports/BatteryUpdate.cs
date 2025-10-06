namespace SpeedEditorSharp.Hardware.Reports;

public class BatteryUpdate : Report
{
    public bool Charging { get; init; }
    public int Level { get; init; }
}