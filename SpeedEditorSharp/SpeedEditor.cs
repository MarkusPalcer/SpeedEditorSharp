using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Events;
using SpeedEditorSharp.Hardware;
using SpeedEditorSharp.Hardware.Reports;

namespace SpeedEditorSharp
{
    /// <summary>
    /// Main Speed Editor device interface
    /// </summary>
    public sealed class SpeedEditor : IDisposable
    {
        private readonly IHardware _hardware;
        private static readonly Lazy<SpeedEditor> Singleton = new(() => new SpeedEditor(new Hardware.Hardware()));

        public static SpeedEditor Instance => Singleton.Value;
        
        private bool _disposed;
        private HashSet<Keys> _previousKeys = new();

        // Controllers

        // Jog state fields (moved from JogController)
        private JogModes _currentJogMode;
        private JogLedStates _currentJogLeds;

        // Battery state fields
        private bool _lastChargingState;
        private int _lastBatteryLevel;

        // Events
        public event EventHandler<JogEventArgs>? JogWheelMoved;
        public event EventHandler<KeyEventArgs>? KeyDown;
        public event EventHandler<KeyEventArgs>? KeyUp;
        public event EventHandler<KeyEventArgs>? KeyPress;
        public event EventHandler<BatteryEventArgs>? BatteryStatusUpdate;

        public SpeedEditor(IHardware hardware)
        {
            _hardware = hardware;
            _hardware.ReportReceived += (_, report) => ProcessReport(report.Report);
            
            // Initialize controllers
            Leds = new LedController(leds => _hardware.SetLedsInternal(leds));
            
            // Initialize jog state - always send initial values to hardware
            _currentJogMode = JogModes.RELATIVE_0;
            _hardware.SendJogModeToHardware(_currentJogMode);
            
            _currentJogLeds = JogLedStates.JOG;
            _hardware.SendJogLedStateToHardware(_currentJogLeds);
        }

        private void ProcessReport(Report report)
        {
            switch (report)
            {
                case BatteryUpdate batteryUpdate:
                    OnBatteryChanged(batteryUpdate.Charging, batteryUpdate.Level);
                    break;
                case JogUpdate jogUpdate:
                    OnJogChanged(jogUpdate.Mode, jogUpdate.Value);
                    break;
                case KeyUpdate keyUpdate:
                    ProcessKeyUpdate(keyUpdate.Keys);
                    break;
            }
        }

        private void ProcessKeyUpdate(HashSet<Keys> currentKeys)
        {
            
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

        /// <summary>
        /// Gets an awaitable task that completes when the hardware connection and initialization is done
        /// </summary>
        public Task Initialization => _hardware.Initialization;
        
        /// <summary>
        /// Gets the LED controller for managing individual LED states
        /// </summary>
        public LedController Leds { get; }

        /// <summary>
        /// Gets or sets the current jog wheel mode
        /// </summary>
        public JogModes JogMode
        {
            get => _currentJogMode;
            set
            {
                if (_currentJogMode != value)
                {
                    _currentJogMode = value;
                    _hardware.SendJogModeToHardware(_currentJogMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the active jog LED states
        /// </summary>
        public JogLedStates ActiveJogLed
        {
            get => _currentJogLeds;
            set
            {
                if (_currentJogLeds != value)
                {
                    _currentJogLeds = value;
                    _hardware.SendJogLedStateToHardware(_currentJogLeds);
                }
            }
        }

        /// <summary>
        /// Gets the charging state from the last battery status update
        /// </summary>
        public bool Charging => _lastChargingState;

        /// <summary>
        /// Gets the battery level from the last battery status update
        /// </summary>
        public int BatteryLevel => _lastBatteryLevel;

        /// <summary>
        /// Safely invoke the JogChanged event
        /// </summary>
        private void OnJogChanged(JogModes modes, int value)
        {
            JogWheelMoved?.Invoke(this, new JogEventArgs(modes, value));
        }

        /// <summary>
        /// Safely invoke the KeyDown event
        /// </summary>
        private void OnKeyDown(Keys key)
        {
            KeyDown?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the KeyUp event
        /// </summary>
        private void OnKeyUp(Keys key)
        {
            KeyUp?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the KeyPress event
        /// </summary>
        private void OnKeyPress(Keys key)
        {
            KeyPress?.Invoke(this, new KeyEventArgs(key));
        }

        /// <summary>
        /// Safely invoke the BatteryChanged event
        /// </summary>
        private void OnBatteryChanged(bool charging, int level)
        {
            _lastChargingState = charging;
            _lastBatteryLevel = level;
            BatteryStatusUpdate?.Invoke(this, new BatteryEventArgs(charging, level));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _hardware.Dispose();
                _disposed = true;
            }
        }
    }
}