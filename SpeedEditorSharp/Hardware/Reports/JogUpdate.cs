using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware.Reports;

/// <summary>
/// Represents a jog wheel movement report received from the Speed Editor hardware.
/// This report contains information about the jog wheel's current mode and position/movement value.
/// </summary>
/// <remarks>
/// This report corresponds to Hardware Report ID 03 and is parsed from incoming hardware data.
/// The report is used to update the jog wheel state and trigger <see cref="SpeedEditor.JogWheelMoved"/> events.
/// The jog wheel can operate in different modes that affect how the Value property should be interpreted.
/// </remarks>
internal class JogUpdate : Report
{
    /// <summary>
    /// Gets or initializes the current jog wheel mode.
    /// </summary>
    /// <value>
    /// A <see cref="JogModes"/> value indicating the current operational mode of the jog wheel.
    /// The mode determines how the <see cref="Value"/> property should be interpreted (relative vs absolute movement).
    /// </value>
    public JogModes Mode { get; init; }

    /// <summary>
    /// Gets or initializes the jog wheel movement or position value.
    /// </summary>
    /// <value>
    /// An integer value representing either the relative movement delta or absolute position of the jog wheel,
    /// depending on the current <see cref="Mode"/>. For absolute modes, the range is typically -4096 to 4096
    /// (approximately half a turn). For relative modes, this represents the movement delta since the last report.
    /// </value>
    public int Value { get; init; }
}