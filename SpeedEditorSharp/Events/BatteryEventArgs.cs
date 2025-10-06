namespace SpeedEditorSharp.Events;

/// <summary>
/// Event args for battery events
/// </summary>
public class BatteryEventArgs(bool isCharging, int level) : EventArgs
{
    public bool IsCharging { get; } = isCharging;
    public int Level { get; } = level;
}