using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware.Reports;

/// <summary>
/// Represents a key state update report received from the Speed Editor hardware.
/// This report contains information about which keys are currently being pressed on the device.
/// </summary>
/// <remarks>
/// This report corresponds to Hardware Report ID 04 and is parsed from incoming hardware data.
/// The report contains an array of up to 6 key codes that are currently being held down.
/// The report is used to determine key press/release events and trigger corresponding events
/// such as <see cref="SpeedEditor.KeyDown"/>, <see cref="SpeedEditor.KeyUp"/>, and <see cref="SpeedEditor.KeyPress"/>.
/// </remarks>
internal class KeyUpdate : Report
{
    /// <summary>
    /// Gets or initializes the set of keys currently being pressed on the Speed Editor device.
    /// </summary>
    /// <value>
    /// A <see cref="HashSet{T}"/> of <see cref="Keys"/> values representing all keys that are
    /// currently being held down. An empty set indicates no keys are currently pressed.
    /// The hardware can report up to 6 simultaneous key presses.
    /// </value>
    public required HashSet<Keys> Keys { get; init; }
}