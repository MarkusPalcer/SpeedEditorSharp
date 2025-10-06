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
        private Leds _currentLedses;

        // Jog mode configuration
        private readonly Dictionary<Keys, (JogLedStates JogLed, JogModes JogMode)> _jogModes;
        
        // Key mapping dictionaries
        private readonly Dictionary<Keys, VirtualKeyCode> _simpleKeyMappings;
        private readonly Dictionary<Keys, (VirtualKeyCode Modifier, VirtualKeyCode Key)> _modifiedKeyMappings;

        public DemoHandler(SpeedEditor speedEditor)
        {
            _speedEditor = speedEditor;
            _inputSimulator = new InputSimulator();
            _currentLedses = 0;

            // Initialize jog mode mappings
            _jogModes = new Dictionary<Keys, (JogLedStates, JogModes)>
            {
                { Keys.SHTL, (JogLedStates.SHTL, JogModes.ABSOLUTE_DEADZERO) },
                { Keys.JOG, (JogLedStates.JOG, JogModes.RELATIVE_2) },
                { Keys.SCRL, (JogLedStates.SCRL, JogModes.RELATIVE_2) }
            };

            // Initialize simple key mappings (Speed Editor key -> Virtual key)
            _simpleKeyMappings = new Dictionary<Keys, VirtualKeyCode>
            {
                { Keys.CAM1, VirtualKeyCode.VK_1 },
                { Keys.CAM2, VirtualKeyCode.VK_2 },
                { Keys.CAM3, VirtualKeyCode.VK_3 },
                { Keys.CAM4, VirtualKeyCode.VK_4 },
                { Keys.CAM5, VirtualKeyCode.VK_5 },
                { Keys.CAM6, VirtualKeyCode.VK_6 },
                { Keys.CAM7, VirtualKeyCode.VK_7 },
                { Keys.CAM8, VirtualKeyCode.VK_8 },
                { Keys.CAM9, VirtualKeyCode.VK_9 },
                { Keys.STOP_PLAY, VirtualKeyCode.SPACE }
            };

            // Initialize modified key mappings (Speed Editor key -> Modifier + Key)
            _modifiedKeyMappings = new Dictionary<Keys, (VirtualKeyCode, VirtualKeyCode)>
            {
                { Keys.CUT, (VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X) },
                { Keys.ESC, (VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z) }
            };

            // Set initial state
            _speedEditor.SetLeds(_currentLedses);
            SetJogModeForKey(Keys.SCRL);

            // Subscribe to events
            _speedEditor.JogChanged += OnJogChanged;
            _speedEditor.KeyDown += OnKeyDown;
            _speedEditor.KeyUp += OnKeyUp;
            _speedEditor.KeyPress += OnKeyPress;
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

        private void HandleKeyMapping(Keys key)
        {
            // Try simple key mappings first
            if (_simpleKeyMappings.TryGetValue(key, out var virtualKey))
            {
                _inputSimulator.Keyboard.KeyPress(virtualKey);
                return;
            }

            // Try modified key mappings
            if (_modifiedKeyMappings.TryGetValue(key, out var modifiedMapping))
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(modifiedMapping.Modifier, modifiedMapping.Key);
                return;
            }

            // If no mapping found, you could optionally log or handle unmapped keys
            // Console.WriteLine($"No mapping found for key: {key}");
        }

        private void OnBatteryChanged(object? sender, BatteryEventArgs e)
        {
            Console.WriteLine($"Battery {e.Level}%{(e.IsCharging ? " and charging" : "")}");
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            Console.WriteLine($"Key DOWN: {e.Key}");
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            Console.WriteLine($"Key UP: {e.Key}");
        }

        private void OnKeyPress(object? sender, KeyEventArgs e)
        {
            Console.WriteLine($"Key PRESS: {e.Key}");
            
            // Handle jog mode selection
            SetJogModeForKey(e.Key);
            
            // Toggle LEDs - check if this key has a corresponding LED
            if (Enum.TryParse<Leds>(e.Key.ToString(), out var ledFlag))
            {
                _currentLedses ^= ledFlag;
                _speedEditor.SetLeds(_currentLedses);
            }
            
            // Handle key mappings
            HandleKeyMapping(e.Key);
        }

        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            _speedEditor.JogChanged -= OnJogChanged;
            _speedEditor.KeyDown -= OnKeyDown;
            _speedEditor.KeyUp -= OnKeyUp;
            _speedEditor.KeyPress -= OnKeyPress;
            _speedEditor.BatteryChanged -= OnBatteryChanged;
        }
    }
}