namespace SpeedEditorSharp.Hardware.Reports;

/// <summary>
/// Represents a battery status update report received from the Speed Editor hardware.
/// This report contains information about the device's current charging state and battery level.
/// </summary>
/// <remarks>
/// This report corresponds to Hardware Report ID 07 and is parsed from incoming hardware data.
/// The report is used to update the device's battery status and trigger <see cref="SpeedEditor.BatteryStatusUpdate"/> events.
/// </remarks>
internal class BatteryUpdate : Report
{
    /// <summary>
    /// Gets or initializes a value indicating whether the Speed Editor device is currently charging.
    /// </summary>
    /// <value>
    /// <c>true</c> if the device is currently connected to a power source and charging; otherwise, <c>false</c>.
    /// </value>
    public bool Charging { get; init; }

    /// <summary>
    /// Gets or initializes the current battery level as a percentage.
    /// </summary>
    /// <value>
    /// An integer representing the battery level from 0 to 100, where 0 indicates an empty battery 
    /// and 100 indicates a fully charged battery.
    /// </value>
    public int Level { get; init; }
}