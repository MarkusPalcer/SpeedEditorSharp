using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware.Reports;

public class KeyUpdate : Report
{
    public HashSet<Keys> Keys { get; init; }
}