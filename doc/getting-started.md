# Getting Started with SpeedEditorSharp

This guide will help you quickly set up and start using the SpeedEditorSharp library to interface with your Blackmagic Speed Editor device.

## Prerequisites

Before you begin, ensure you have:

- Windows 10 or later
- .NET 9.0 or later
- A Blackmagic Design Speed Editor connected via USB
- **DaVinci Resolve must be closed** (it locks the device exclusively)

## Installation

### Option 1: NuGet Package (Recommended)

```bash
dotnet add package SpeedEditorSharp
```

### Option 2: Build from Source

1. Clone the repository:
```bash
git clone https://github.com/MarkusPalcer/SpeedEditorSharp.git
cd SpeedEditorSharp
```

2. Build the solution:
```bash
dotnet build
```

3. Reference the built library in your project.

## Basic Usage

Here's a minimal example to get you started:

### Simple Key Handling

```csharp
using SpeedEditorSharp;
using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Events;

class Program
{
    static async Task Main(string[] args)
    {
        // Get the Speed Editor instance
        var speedEditor = SpeedEditor.Instance;
        
        // Subscribe to key press events
        speedEditor.KeyPress += OnKeyPress;
        
        // Connect to the device
        await speedEditor.ConnectAsync();
        
        Console.WriteLine("Speed Editor connected! Press keys on the device...");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        
        // Clean up
        await speedEditor.DisconnectAsync();
    }
    
    private static void OnKeyPress(object? sender, KeyEventArgs e)
    {
        Console.WriteLine($"Key pressed: {e.Key}");
        
        // Handle specific keys
        switch (e.Key)
        {
            case Keys.STOP_PLAY:
                Console.WriteLine("Play/Stop button pressed!");
                break;
            case Keys.CAM1:
                Console.WriteLine("Camera 1 selected!");
                break;
            case Keys.CUT:
                Console.WriteLine("Cut button pressed!");
                break;
        }
    }
}
```

### LED Control

```csharp
using SpeedEditorSharp;

// Get the Speed Editor instance and connect
var speedEditor = SpeedEditor.Instance;
await speedEditor.ConnectAsync();

// Turn on some LEDs
speedEditor.Leds.Cut = true;
speedEditor.Leds.Cam1 = true;
speedEditor.Leds.Cam2 = true;

// Turn off LEDs
speedEditor.Leds.Cut = false;
```

### Jog Wheel Handling

```csharp
using SpeedEditorSharp;
using SpeedEditorSharp.Events;

var speedEditor = SpeedEditor.Instance;

// Subscribe to jog wheel events
speedEditor.JogWheelMoved += OnJogWheelMoved;

await speedEditor.ConnectAsync();

// Your app logic here...

private static void OnJogWheelMoved(object? sender, JogEventArgs e)
{
    Console.WriteLine($"Jog wheel moved: {e.Value} (Mode: {e.Mode})");
    
    // Handle jog wheel movement
    if (e.Value > 0)
    {
        Console.WriteLine("Jog wheel turned clockwise");
    }
    else if (e.Value < 0)
    {
        Console.WriteLine("Jog wheel turned counter-clockwise");
    }
}
```

### Device Status

```csharp
// Check connection status
bool isConnected = speedEditor.IsConnected;

// Battery information
int batteryLevel = speedEditor.BatteryLevel;
bool isCharging = speedEditor.Charging;
```

## Troubleshooting

If you encounter issues:

1. **Device not found**: Ensure DaVinci Resolve is closed and the device is connected
2. **Permission errors**: Try running your application as Administrator
3. **Connection fails**: Check the [Troubleshooting](troubleshooting.md) guide
