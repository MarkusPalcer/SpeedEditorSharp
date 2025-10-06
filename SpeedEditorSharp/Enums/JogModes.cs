namespace SpeedEditorSharp.Enums
{
    /// <summary>
    /// Jog wheel modes for the Speed Editor
    /// </summary>
    public enum JogModes : byte
    {
        RELATIVE_0 = 0,           // Relative mode
        ABSOLUTE_CONTINUOUS = 1,   // Send an "absolute" position (based on the position when mode was set) -4096 -> 4096 range ~ half a turn
        RELATIVE_2 = 2,           // Same as mode 0 ?
        ABSOLUTE_DEADZERO = 3     // Same as mode 1 but with a small dead band around zero that maps to 0
    }
}