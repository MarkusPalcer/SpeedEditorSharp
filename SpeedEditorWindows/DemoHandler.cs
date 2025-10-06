using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;

namespace SpeedEditorWindows
{
    /// <summary>
    /// Demo handler for Speed Editor events that simulates keyboard input
    /// </summary>
    public class DemoHandler : IDisposable
    {
        private readonly SpeedEditor _speedEditor;
        private readonly InputSimulator _inputSimulator;
        private List<SpeedEditorKey> _currentKeys;
        private SpeedEditorLed _currentLeds;

        // Jog mode configuration
        private readonly Dictionary<SpeedEditorKey, (SpeedEditorJogLed JogLed, SpeedEditorJogMode JogMode)> _jogModes;

        public DemoHandler(SpeedEditor speedEditor)
        {
            _speedEditor = speedEditor;
            _inputSimulator = new InputSimulator();
            _currentKeys = new List<SpeedEditorKey>();
            _currentLeds = 0;

            // Initialize jog mode mappings
            _jogModes = new Dictionary<SpeedEditorKey, (SpeedEditorJogLed, SpeedEditorJogMode)>
            {
                { SpeedEditorKey.SHTL, (SpeedEditorJogLed.SHTL, SpeedEditorJogMode.ABSOLUTE_DEADZERO) },
                { SpeedEditorKey.JOG, (SpeedEditorJogLed.JOG, SpeedEditorJogMode.RELATIVE_2) },
                { SpeedEditorKey.SCRL, (SpeedEditorJogLed.SCRL, SpeedEditorJogMode.RELATIVE_2) }
            };

            // Set initial state
            _speedEditor.SetLeds(_currentLeds);
            SetJogModeForKey(SpeedEditorKey.SCRL);

            // Subscribe to events
            _speedEditor.JogChanged += OnJogChanged;
            _speedEditor.KeyChanged += OnKeyChanged;
            _speedEditor.BatteryChanged += OnBatteryChanged;
        }

        private void SetJogModeForKey(SpeedEditorKey key)
        {
            if (_jogModes.TryGetValue(key, out var jogConfig))
            {
                _speedEditor.SetJogLeds(jogConfig.JogLed);
                _speedEditor.SetJogMode(jogConfig.JogMode);
            }
        }

        private void OnJogChanged(object? sender, JogEventArgs e)
        {
            Console.WriteLine($"Jog mode {(int)e.Mode}: {e.Value}");

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
            var keyNames = e.Keys.Count > 0 ? string.Join(", ", e.Keys.Select(k => k.ToString())) : "None";
            Console.WriteLine($"Keys held: {keyNames}");

            // Find keys being released and toggle LED if there is one
            foreach (var releasedKey in _currentKeys.Where(k => !e.Keys.Contains(k)))
            {
                // Select jog mode
                SetJogModeForKey(releasedKey);

                // Toggle LEDs - check if this key has a corresponding LED
                if (Enum.TryParse<SpeedEditorLed>(releasedKey.ToString(), out var ledFlag))
                {
                    _currentLeds ^= ledFlag;
                    _speedEditor.SetLeds(_currentLeds);
                }
            }

            _currentKeys = new List<SpeedEditorKey>(e.Keys);

            // Example key mappings
            HandleKeyMappings(e.Keys);
        }

        private void HandleKeyMappings(List<SpeedEditorKey> keys)
        {
            // Example: pressing CAM1 will press the '1' key on the keyboard
            if (keys.Contains(SpeedEditorKey.CAM1) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_1);
            }
            else if (keys.Contains(SpeedEditorKey.CAM2) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_2);
            }
            else if (keys.Contains(SpeedEditorKey.CAM3) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_3);
            }
            else if (keys.Contains(SpeedEditorKey.CAM4) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_4);
            }
            else if (keys.Contains(SpeedEditorKey.CAM5) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_5);
            }
            else if (keys.Contains(SpeedEditorKey.CAM6) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_6);
            }
            else if (keys.Contains(SpeedEditorKey.CAM7) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_7);
            }
            else if (keys.Contains(SpeedEditorKey.CAM8) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_8);
            }
            else if (keys.Contains(SpeedEditorKey.CAM9) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_9);
            }
            else if (keys.Contains(SpeedEditorKey.STOP_PLAY) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            }
            else if (keys.Contains(SpeedEditorKey.CUT) && keys.Count == 1)
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X);
            }
            else if (keys.Contains(SpeedEditorKey.ESC) && keys.Count == 1)
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
            if (_speedEditor != null)
            {
                _speedEditor.JogChanged -= OnJogChanged;
                _speedEditor.KeyChanged -= OnKeyChanged;
                _speedEditor.BatteryChanged -= OnBatteryChanged;
            }
        }
    }
}