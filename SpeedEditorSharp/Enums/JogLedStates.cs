namespace SpeedEditorSharp.Enums;

/// <summary>
/// LED flags for the Jog mode buttons
/// Setting those leds is done with SET_REPORT on Output Report ID 4
/// which takes a single 8 bits bitfield of the LEDs to enable
/// </summary>
[Flags]
public enum JogLedStates : byte
{
    JOG = (1 << 0),
    SHTL = (1 << 1),
    SCRL = (1 << 2)
}