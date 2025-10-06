using WindowsInput;
using WindowsInput.Native;
using SpeedEditorSharp;
using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Events;

namespace SimpleDemo
{
    /// <summary>
    /// Demo handler for Speed Editor events that simulates keyboard input
    /// </summary>
    public class DemoHandler : IDisposable
    {
        private readonly SpeedEditor _speedEditor;
        private readonly InputSimulator _inputSimulator;
        private List<Keys> _currentKeys;
        private Leds _currentLedses;

        // Jog mode configuration
        private readonly Dictionary<Keys, (JogLedStates JogLed, JogModes JogMode)> _jogModes;

        public DemoHandler(SpeedEditor speedEditor)
        {
            _speedEditor = speedEditor;
            _inputSimulator = new InputSimulator();
            _currentKeys = new List<Keys>();
            _currentLedses = 0;

            // Initialize jog mode mappings
            _jogModes = new Dictionary<Keys, (JogLedStates, JogModes)>
            {
                { Keys.SHTL, (JogLedStates.SHTL, JogModes.ABSOLUTE_DEADZERO) },
                { Keys.JOG, (JogLedStates.JOG, JogModes.RELATIVE_2) },
                { Keys.SCRL, (JogLedStates.SCRL, JogModes.RELATIVE_2) }
            };

            // Set initial state
            _speedEditor.SetLeds(_currentLedses);
            SetJogModeForKey(Keys.SCRL);

            // Subscribe to events
            _speedEditor.JogChanged += OnJogChanged;
            _speedEditor.KeyChanged += OnKeyChanged;
            _speedEditor.BatteryChanged += OnBatteryChanged;
        }

        private void SetJogModeForKey(Keys keys)
        {
            if (_jogModes.TryGetValue(keys, out var jogConfig))
            {
                _speedEditor.SetJogLeds(jogConfig.JogLed);
                _speedEditor.SetJogMode(jogConfig.JogMode);
            }
        }

        private void OnJogChanged(object? sender, JogEventArgs e)
        {
            Console.WriteLine($"Jog mode {(int)e.Modes}: {e.Value}");

            // Example: when the jog wheel is turned, simulate left/right arrow keys
            if (e.Value > 0)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
            }
            else if (e.Value < 0)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT);
            }
        }

        private void OnKeyChanged(object? sender, KeyEventArgs e)
        {
            // Debug message
            var keyNames = e.Keys.Any() ? string.Join(", ", e.Keys.Select(k => k.ToString())) : "None";
            Console.WriteLine($"Keys held: {keyNames}");

            // Find keys being released and toggle LED if there is one
            foreach (var releasedKey in _currentKeys.Where(k => !e.Keys.Contains(k)))
            {
                // Select jog mode
                SetJogModeForKey(releasedKey);

                // Toggle LEDs - check if this key has a corresponding LED
                if (Enum.TryParse<Leds>(releasedKey.ToString(), out var ledFlag))
                {
                    _currentLedses ^= ledFlag;
                    _speedEditor.SetLeds(_currentLedses);
                }
            }

            _currentKeys = new List<Keys>(e.Keys);

            // Example key mappings
            HandleKeyMappings(e.Keys);
        }

        private void HandleKeyMappings(IEnumerable<Keys> keys)
        {
            keys = keys.ToArray();
            
            // Example: pressing CAM1 will press the '1' key on the keyboard
            if (keys.Contains(Keys.CAM1))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_1);
            }
            else if (keys.Contains(Keys.CAM2))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_2);
            }
            else if (keys.Contains(Keys.CAM3))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_3);
            }
            else if (keys.Contains(Keys.CAM4))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_4);
            }
            else if (keys.Contains(Keys.CAM5))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_5);
            }
            else if (keys.Contains(Keys.CAM6))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_6);
            }
            else if (keys.Contains(Keys.CAM7))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_7);
            }
            else if (keys.Contains(Keys.CAM8))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_8);
            }
            else if (keys.Contains(Keys.CAM9))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_9);
            }
            else if (keys.Contains(Keys.STOP_PLAY))
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            }
            else if (keys.Contains(Keys.CUT))
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X);
            }
            else if (keys.Contains(Keys.ESC))
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z);
            }
            // Add more key mappings as needed...
        }

        private void OnBatteryChanged(object? sender, BatteryEventArgs e)
        {
            Console.WriteLine($"Battery {e.Level}%{(e.IsCharging ? " and charging" : "")}");
        }

        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            _speedEditor.JogChanged -= OnJogChanged;
            _speedEditor.KeyChanged -= OnKeyChanged;
            _speedEditor.BatteryChanged -= OnBatteryChanged;
        }
    }
}