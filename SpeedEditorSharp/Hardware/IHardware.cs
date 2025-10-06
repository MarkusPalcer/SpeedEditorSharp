using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Hardware;

public interface IHardware : IDisposable
{
    public Task Initialization { get; }
    void SetLedsInternal(Leds leds);
    void SendJogLedStateToHardware(JogLedStates jogLedsStates);
    void SendJogModeToHardware(JogModes jogModes);
    event EventHandler<ReportReceivedEventArgs>? ReportReceived;
}