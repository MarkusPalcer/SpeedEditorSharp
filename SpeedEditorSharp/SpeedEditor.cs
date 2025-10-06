using HidSharp;
using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Events;

namespace SpeedEditorSharp
{
    /// <summary>
    /// Main Speed Editor device interface
    /// </summary>
    public class SpeedEditor : IDisposable
    {
        private const int UsbVid = 0x1edb;
        private const int UsbPid = 0xda0e;
        
        private HidDevice? _hidDevice;
        private HidStream _hidStream;
        private bool _disposed;
        private CancellationTokenSource _cancellationTokenSource;
        private HashSet<Keys> _previousKeys = new HashSet<Keys>();

        // Events
        public event EventHandler<JogEventArgs>? JogChanged;
        public event EventHandler<KeyEventArgs>? KeyDown;
        public event EventHandler<KeyEventArgs>? KeyUp;
        public event EventHandler<KeyEventArgs>? KeyPress;
        public event EventHandler<BatteryEventArgs>? BatteryChanged;

        public SpeedEditor()
        {
            // Find the Speed Editor device
            var devices = DeviceList.Local.GetHidDevices(UsbVid, UsbPid);
            _hidDevice = devices.FirstOrDefault();
            
            if (_hidDevice == null)
            {
                throw new InvalidOperationException("Blackmagic Speed Editor device not found. Make sure it's connected and not in use by DaVinci Resolve.");
            }

            _hidStream = _hidDevice.Open();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Authenticate with the Speed Editor device
        /// </summary>
        /// <returns>Timeout value for re-authentication</returns>
        public int Authenticate()
        {
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
                        if (!_disposed && !_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            Authenticate();
                        }
                    }, _cancellationTokenSource.Token);
            }

            return timeout;
        }

        /// <summary>
        /// Set LED states
        /// </summary>
        public void SetLeds(Leds ledses)
        {
            var report = new byte[5];
            report[0] = 2; // Report ID
            var ledBytes = BitConverter.GetBytes((uint)ledses);
            Array.Copy(ledBytes, 0, report, 1, 4);
            _hidStream.Write(report);
        }

        /// <summary>
        /// Set jog LED states
        /// </summary>
        public void SetJogLeds(JogLedStates jogLedsStates)
        {
            var report = new byte[2];
            report[0] = 4; // Report ID
            report[1] = (byte)jogLedsStates;
            _hidStream.Write(report);
        }

        /// <summary>
        /// Set jog wheel mode
        /// </summary>
        public void SetJogMode(JogModes jogModes, byte unknown = 255)
        {
            var report = new byte[7];
            report[0] = 3; // Report ID
            report[1] = (byte)jogModes;
            // bytes 2-5 are zero (4 byte integer)
            report[6] = unknown;
            _hidStream.Write(report);
        }

        /// <summary>
        /// Poll for reports from the device
        /// </summary>
        public void Poll(int timeout = -1)
        {
            try
            {
                var report = new byte[64];
                int bytesRead = _hidStream.Read(report, 0, report.Length);
                
                if (bytesRead > 0)
                {
                    ParseReport(report);
                }
            }
            catch (Exception ex) when (ex is TimeoutException || ex is OperationCanceledException)
            {
                // Timeout is expected, just return
            }
        }

        /// <summary>
        /// Safely invoke the JogChanged event
        /// </summary>
        protected virtual void OnJogChanged(JogModes modes, int value)
        {
            JogChanged?.Invoke(this, new JogEventArgs(modes, value));
        }

        /// <summary>
        /// Safely invoke the KeyDown event
        /// </summary>
        protected virtual void OnKeyDown(Keys key)
        {
            KeyDown?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the KeyUp event
        /// </summary>
        protected virtual void OnKeyUp(Keys key)
        {
            KeyUp?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the KeyPress event
        /// </summary>
        protected virtual void OnKeyPress(Keys key)
        {
            KeyPress?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the BatteryChanged event
        /// </summary>
        protected virtual void OnBatteryChanged(bool charging, int level)
        {
            BatteryChanged?.Invoke(this, new BatteryEventArgs(charging, level));
        }

        private void ParseReport(byte[] report)
        {
            if (report.Length == 0) return;

            switch (report[0])
            {
                case 0x03:
                    ParseReport03(report);
                    break;
                case 0x04:
                    ParseReport04(report);
                    break;
                case 0x07:
                    ParseReport07(report);
                    break;
                default:
                    Console.WriteLine($"[!] Unhandled report {BitConverter.ToString(report).Replace("-", "")}");
                    break;
            }
        }

        private void ParseReport03(byte[] report)
        {
            // Report ID 03
            // u8   - Report ID
            // u8   - Jog mode
            // le32 - Jog value (signed)
            // u8   - Unknown ?
            // 6*u8 - Unknown? (skip the remaining bytes)
            
            if (report.Length < 7) return;
            
            var jogMode = (JogModes)report[1];
            var jogValue = BitConverter.ToInt32(report, 2);
            
            OnJogChanged(jogMode, jogValue);
        }

        private void ParseReport04(byte[] report)
        {
            // Report ID 04
            // u8      - Report ID
            // le16[6] - Array of keys held down
            
            if (report.Length < 13) return;
            
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
            
            // Detect key down events (keys that are now pressed but weren't before)
            foreach (var key in currentKeys)
            {
                if (!_previousKeys.Contains(key))
                {
                    OnKeyDown(key);
                }
            }
            
            // Detect key up events (keys that were pressed but aren't now)
            foreach (var key in _previousKeys)
            {
                if (!currentKeys.Contains(key))
                {
                    OnKeyUp(key);
                    // KeyPress event is fired when a key is released (complete press cycle)
                    OnKeyPress(key);
                }
            }
            
            // Update previous keys state
            _previousKeys = currentKeys;
        }

        private void ParseReport07(byte[] report)
        {
            // Report ID 07
            // u8 - Report ID
            // u8 - Charging (1) / Not-charging (0)
            // u8 - Battery level (0-100)
            
            if (report.Length < 3) return;
            
            bool charging = report[1] != 0;
            int batteryLevel = report[2];
            
            OnBatteryChanged(charging, batteryLevel);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();
                _hidStream?.Dispose();
                _disposed = true;
            }
        }
    }
}