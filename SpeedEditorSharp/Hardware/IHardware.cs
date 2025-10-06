using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware;

/// <summary>
/// Abstracts the hardware connection away, so it can be mocked for testing.
/// Provides an interface for communicating with SpeedEditor hardware devices.
/// </summary>
internal interface IHardware : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the hardware is currently connected.
    /// </summary>
    /// <value>
    /// <c>true</c> if the hardware is connected; otherwise, <c>false</c>.
    /// </value>
    bool IsConnected { get; }
    
    /// <summary>
    /// Asynchronously establishes a connection to the SpeedEditor hardware.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the connection operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous connection operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the hardware is already connected or when connection fails.
    /// </exception>
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously disconnects from the SpeedEditor hardware.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous disconnection operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the hardware is not currently connected.
    /// </exception>
    Task DisconnectAsync();
    
    /// <summary>
    /// Sends the the LED states to the SpeedEditor hardware.
    /// </summary>
    /// <param name="state">
    /// The LED configuration specifying which LEDs should be turned on or off.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the hardware is not connected.
    /// </exception>
    void SendLedStates(Leds state);
    
    /// <summary>
    /// Sends the jog wheel LED state configuration to the SpeedEditor hardware.
    /// </summary>
    /// <param name="jogLedsState">
    /// The jog wheel LED state configuration to apply.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the hardware is not connected.
    /// </exception>
    void SendJogLedState(JogLedStates jogLedsState);
    
    /// <summary>
    /// Sends the jog wheel mode configuration to the SpeedEditor hardware.
    /// </summary>
    /// <param name="jogMode">
    /// The jog wheel mode configuration to apply.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the hardware is not connected.
    /// </exception>
    void SendJogMode(JogModes jogMode);
    
    /// <summary>
    /// Occurs when a report is received from the SpeedEditor hardware.
    /// </summary>
    /// <remarks>
    /// This event is raised whenever the hardware sends data such as key presses,
    /// jog wheel movements, or battery status updates.
    /// </remarks>
    event EventHandler<ReportReceivedEventArgs>? ReportReceived;
}