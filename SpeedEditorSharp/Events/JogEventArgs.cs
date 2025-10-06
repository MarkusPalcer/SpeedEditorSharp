using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Events;

/// <summary>
/// Event args for jog wheel events
/// </summary>
public class JogEventArgs(JogModes modes, int value) : EventArgs
{
    public JogModes Modes { get; } = modes;
    public int Value { get; } = value;
}