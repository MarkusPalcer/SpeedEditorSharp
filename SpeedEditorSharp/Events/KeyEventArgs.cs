using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Events
{
    /// <summary>
    /// Event args for key events
    /// </summary>
    public class KeyEventArgs(IEnumerable<Keys> keys) : EventArgs
    {
        public IEnumerable<Keys> Keys { get; } = keys;
    }
}