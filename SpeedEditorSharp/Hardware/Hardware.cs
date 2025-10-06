using HidSharp;
using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Hardware.Reports;

namespace SpeedEditorSharp.Hardware;

internal class Hardware : IHardware
{
    private const int UsbVid = 0x1edb;
    private const int UsbPid = 0xda0e;
    private HidStream? _hidStream;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isDisposed;
    
    public event EventHandler<ReportReceivedEventArgs>? ReportReceived;

    public Task Initialization { get; }

    public Hardware()
    {
        Initialization = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _hidStream = await ConnectAsync();
        Authenticate();
        new Thread(Run).Start();
    }

    private async Task<HidStream> ConnectAsync()
    {
        while (true)
        {
            if (_cancellationTokenSource.IsCancellationRequested) throw new OperationCanceledException();

            // Find the Speed Editor device
            var devices = DeviceList.Local.GetHidDevices(UsbVid, UsbPid);
            var hidDevice = devices.FirstOrDefault();
            if (hidDevice is null)
            {
                // Wait a bit before retrying
                await Task.Delay(1000);
            }
            else
            {
                return hidDevice.Open();
            }
        }
    }

    private void Authenticate()
    {
        if (_hidStream is null) throw new OperationCanceledException();

        // Reset the auth state machine
        var resetReport = new byte[] { 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        _hidStream.SetFeature(resetReport);

        // Read the keyboard challenge (for keyboard to authenticate app)
        var challengeReport = new byte[10];
        challengeReport[0] = 0x06; // Set report ID
        _hidStream.GetFeature(challengeReport);

        if (challengeReport[0] != 0x06 || challengeReport[1] != 0x00)
        {
            throw new InvalidOperationException("Failed authentication get_kbd_challenge");
        }

        // Extract challenge (8 bytes little endian starting at position 2)
        ulong challenge = BitConverter.ToUInt64(challengeReport, 2);

        // Send our challenge (to authenticate keyboard)
        // We don't care... so just send 0x0000000000000000
        var ourChallengeReport = new byte[] { 0x06, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        _hidStream.SetFeature(ourChallengeReport);

        // Read the keyboard response
        // Again, we don't care, ignore the result
        var keyboardResponseReport = new byte[10];
        keyboardResponseReport[0] = 0x06; // Set report ID
        _hidStream.GetFeature(keyboardResponseReport);

        if (keyboardResponseReport[0] != 0x06 || keyboardResponseReport[1] != 0x02)
        {
            throw new InvalidOperationException("Failed authentication get_kbd_response");
        }

        // Compute and send our response
        ulong response = Authentication.ComputeAuthResponse(challenge);
        var responseBytes = BitConverter.GetBytes(response);
        var responseReport = new byte[10];
        responseReport[0] = 0x06;
        responseReport[1] = 0x03;
        Array.Copy(responseBytes, 0, responseReport, 2, 8);
        _hidStream.SetFeature(responseReport);

        // Read the status
        var statusReport = new byte[10];
        statusReport[0] = 0x06; // Set report ID
        _hidStream.GetFeature(statusReport);

        if (statusReport[0] != 0x06 || statusReport[1] != 0x04)
        {
            throw new InvalidOperationException("Failed authentication get_kbd_status");
        }

        // Extract timeout (2 bytes little endian starting at position 2)
        int timeout = BitConverter.ToUInt16(statusReport, 2);

        // Schedule re-authentication 10 seconds before the timeout
        if (timeout > 10)
        {
            Task.Delay((timeout - 10) * 1000, _cancellationTokenSource.Token)
                .ContinueWith(_ =>
                 {
                     if (!_cancellationTokenSource.Token.IsCancellationRequested)
                     {
                         Authenticate();
                     }
                 }, _cancellationTokenSource.Token);
        }
    }
    
    private void Run()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var report = Poll();
            if (report is not null) OnReportReceived(report);
        }
    }


    /// <inheritdoc />
    public void SetLedsInternal(Leds leds)
    {
        if (_hidStream is null)
            throw new InvalidOperationException("You must wait for initialization to complete before using the device.");

        var report = new byte[5];
        report[0] = 2; // Report ID
        var ledBytes = BitConverter.GetBytes((uint)leds);
        Array.Copy(ledBytes, 0, report, 1, 4);
        _hidStream.Write(report);
    }

    public void SendJogLedStateToHardware(JogLedStates jogLedsStates)
    {
        if (_hidStream is null)
            throw new InvalidOperationException("You must wait for initialization to complete before using the device.");

        var report = new byte[2];
        report[0] = 4; // Report ID
        report[1] = (byte)jogLedsStates;
        _hidStream.Write(report);
    }
    
    public void SendJogModeToHardware(JogModes jogModes)
    {
        if (_hidStream is null)
            throw new InvalidOperationException("You must wait for initialization to complete before using the device.");
        
        var report = new byte[7];
        report[0] = 3; // Report ID
        report[1] = (byte)jogModes;
        // bytes 2-5 are zero (4 byte integer)
        report[6] = 255;
        _hidStream.Write(report);
    }
    
    /// <summary>
    /// Poll for reports from the device
    /// </summary>
    private Report? Poll()
    {
        if (_cancellationTokenSource.IsCancellationRequested || _hidStream is null)
            return null;
        
        try
        {
            var report = new byte[64];
            int bytesRead = _hidStream.Read(report, 0, report.Length);
                
            if (bytesRead > 0)
            {
                return ParseReport(report);
            }
        }
        catch (Exception ex) when (ex is TimeoutException || ex is OperationCanceledException)
        {
            // Timeout is expected, just return
        }

        return null;
    }

    
    private Report? ParseReport(byte[] report)
    {
        if (report.Length == 0) return null;

        switch (report[0])
        {
            case 0x03:
                return ParseReport03(report);
            case 0x04:
                return ParseReport04(report);
            case 0x07:
                return ParseReport07(report);
            default:
                throw new NotSupportedException("Unknown report type");
        }
    }
    
      private Report? ParseReport03(byte[] report)
        {
            // Report ID 03
            // u8   - Report ID
            // u8   - Jog mode
            // le32 - Jog value (signed)
            // u8   - Unknown ?
            // 6*u8 - Unknown? (skip the remaining bytes)
            
            if (report.Length < 7) return null;
            
            var jogMode = (JogModes)report[1];
            var jogValue = BitConverter.ToInt32(report, 2);
            
            return new JogUpdate
            {
                Mode = jogMode,
                Value = jogValue
            };
        }

        private Report? ParseReport04(byte[] report)
        {
            // Report ID 04
            // u8      - Report ID
            // le16[6] - Array of keys held down
            
            if (report.Length < 13) return null;
            
            var currentKeys = new HashSet<Keys>();
            
            for (int i = 0; i < 6; i++)
            {
                var keyCode = BitConverter.ToUInt16(report, 1 + (i * 2));
                if (keyCode != 0)
                {
                    var key = (Keys)keyCode;
                    currentKeys.Add(key);
                }
            }
            
            return new KeyUpdate { Keys = currentKeys };
            
            
        }

        private Report? ParseReport07(byte[] report)
        {
            // Report ID 07
            // u8 - Report ID
            // u8 - Charging (1) / Not-charging (0)
            // u8 - Battery level (0-100)
            
            if (report.Length < 3) return null;
            
            var charging = report[1] != 0;
            int batteryLevel = report[2];

            return new BatteryUpdate
            {
                Charging = charging,
                Level = batteryLevel
            };
        }
    
    public void Dispose()
    {
        if (_isDisposed) return;

        _cancellationTokenSource.Cancel();
        if (_hidStream is not null) _hidStream.Dispose();

        _isDisposed = true;
    }

    protected virtual void OnReportReceived(Report e)
    {
        ReportReceived?.Invoke(this, new ReportReceivedEventArgs { Report = e });
    }
}