using System;
using System.Collections.Generic;

namespace SpeedEditorWindows
{
    /// <summary>
    /// Event args for jog wheel events
    /// </summary>
    public class JogEventArgs : EventArgs
    {
        public SpeedEditorJogMode Mode { get; }
        public int Value { get; }

        public JogEventArgs(SpeedEditorJogMode mode, int value)
        {
            Mode = mode;
            Value = value;
        }
    }

    /// <summary>
    /// Event args for key events
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        public List<SpeedEditorKey> Keys { get; }

        public KeyEventArgs(List<SpeedEditorKey> keys)
        {
            Keys = keys ?? new List<SpeedEditorKey>();
        }
    }

    /// <summary>
    /// Event args for battery events
    /// </summary>
    public class BatteryEventArgs : EventArgs
    {
        public bool IsCharging { get; }
        public int Level { get; }

        public BatteryEventArgs(bool isCharging, int level)
        {
            IsCharging = isCharging;
            Level = level;
        }
    }
}