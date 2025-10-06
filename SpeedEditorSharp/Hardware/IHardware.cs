using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware;

public interface IHardware : IDisposable
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    void SetLedsInternal(Leds leds);
    void SendJogLedStateToHardware(JogLedStates jogLedsStates);
    void SendJogModeToHardware(JogModes jogModes);
    event EventHandler<ReportReceivedEventArgs>? ReportReceived;
}