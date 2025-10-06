# SpeedEditorSharp

A .NET library for interfacing with the Blackmagic Speed Editor, based on the Python implementation by octimot.

The original Python implementation can be found here: 
[GitHub - octimot/blackmagic-speededitor](https://github.com/octimot/blackmagic-speededitor)

## 📖 Documentation

- 🚀 **[Getting Started](doc/getting-started.md)** - Basic setup and first steps
- ❓ **[Troubleshooting](doc/troubleshooting.md)** - Solutions to common problems

## Projects Overview

### SpeedEditorSharp (Library)
A .NET 9.0 class library that provides the core functionality for communicating with the Blackmagic Speed Editor device.

### SimpleDemo (Console Application)  
A console application that demonstrates how to use the SpeedEditorSharp library.

## Quick Start

```csharp
using SpeedEditorSharp;
using SpeedEditorSharp.Events;

// Get the Speed Editor instance
var speedEditor = SpeedEditor.Instance;

// Subscribe to events
speedEditor.KeyPress += (sender, e) => Console.WriteLine($"Key: {e.Key}");

// Connect and use
await speedEditor.ConnectAsync();
speedEditor.Leds.Cut = true; // Turn on Cut LED

// Wait for a bit, so the user can do something
await Task.Delay(TimeSpan.FromMinutes(1));

// Clean up
await speedEditor.DisconnectAsync();
```

**For detailed setup and usage instructions, see the [Getting Started Guide](doc/getting-started.md).**

## Requirements

- Windows 10 or later
- .NET 9.0 or later  
- Blackmagic Design Speed Editor connected via USB
- **Important**: DaVinci Resolve must be closed (it locks the device exclusively)

## Installation

### Via NuGet (Recommended)
```bash
dotnet add package SpeedEditorSharp
```

### Build from Source
```bash
git clone https://github.com/MarkusPalcer/SpeedEditorSharp.git
cd SpeedEditorSharp
dotnet build
```

## License

This project maintains the same Apache 2.0 license as the original Python implementation by Sylvain Munaut.

## Contributing

Feel free to submit issues, feature requests, or pull requests to improve the library.