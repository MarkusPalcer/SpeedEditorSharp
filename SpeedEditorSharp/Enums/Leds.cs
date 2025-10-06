namespace SpeedEditorSharp.Enums;

/// <summary>
/// LED flags for the main Speed Editor buttons
/// Setting the leds is done with SET_REPORT on Output Report ID 2
/// which takes a single LE32 bitfield of the LEDs to enable
/// </summary>
[Flags]
public enum Leds : uint
{
    CLOSE_UP = (1u << 0),
    CUT = (1u << 1),
    DIS = (1u << 2),
    SMTH_CUT = (1u << 3),
    TRANS = (1u << 4),
    SNAP = (1u << 5),
    CAM7 = (1u << 6),
    CAM8 = (1u << 7),
    CAM9 = (1u << 8),
    LIVE_OWR = (1u << 9),
    CAM4 = (1u << 10),
    CAM5 = (1u << 11),
    CAM6 = (1u << 12),
    VIDEO_ONLY = (1u << 13),
    CAM1 = (1u << 14),
    CAM2 = (1u << 15),
    CAM3 = (1u << 16),
    AUDIO_ONLY = (1u << 17)
}