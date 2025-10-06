namespace SpeedEditorSharp.Hardware.Reports;

/// <summary>
/// Abstract base class for all hardware reports received from the Speed Editor device.
/// </summary>
/// <remarks>
/// This class serves as the foundation for all specific report types that represent 
/// different kinds of data received from the Speed Editor hardware, such as battery updates,
/// jog wheel movements, and key press/release events. Each concrete report type corresponds
/// to a specific hardware report ID and contains the relevant data for that report type.
/// </remarks>
public abstract class Report;
