using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware.Reports;

public class JogUpdate : Report
{
    public JogModes Mode { get; init; }
    public int Value { get; init; }
}