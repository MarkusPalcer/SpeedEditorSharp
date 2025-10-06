using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Events
{
    /// <summary>
    /// Event args for key events (down, up, and press)
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        public Keys Key { get; }

        public KeyEventArgs(Keys key)
        {
            Key = key;
        }
    }
}