# Blackmagic Speed Editor - Windows C# Implementation

This is based on a Python-based Blackmagic Speed Editor interface. 
It provides the same functionality as the original Python script but runs 
natively on Windows using C# and .NET and adheres to a typical C# structure.

The original Python implementation can be found here: 
[GitHub - octimot/blackmagic-speededitor(https://github.com/octimot/blackmagic-speededitor)

## Features

- **HID Device Communication**: Direct communication with the Blackmagic Speed Editor via USB
- **Authentication**: Full implementation of the device authentication protocol
- **Key Mapping**: Configurable key mappings to Windows keyboard inputs
- **Jog Wheel Support**: Handles jog wheel movements and different jog modes
- **LED Control**: Controls the device LEDs based on key presses
- **Battery Monitoring**: Reports battery status and charging state

## Requirements

- Windows 10 or later
- .NET 9.0 or later
- Blackmagic Design Speed Editor connected via USB
- **Important**: DaVinci Resolve must be closed (it locks the device exclusively)

## Installation

1. Ensure you have .NET 9.0 SDK installed
2. Clone or download this project
3. Navigate to the `SpeedEditorCSharp/SpeedEditorWindows` directory
4. Run the following commands:

```bash
dotnet restore
dotnet build
```

## Usage

1. Connect your Blackmagic Speed Editor via USB
2. **Close DaVinci Resolve** if it's running (critical - the device can only be used by one application at a time)
3. Run the application:

```bash
dotnet run
```

Or build and run the executable:

```bash
dotnet build -c Release
./bin/Release/net9.0/SpeedEditorWindows.exe
```

## Default Key Mappings

The demo implementation includes the following key mappings:

| Speed Editor Key | Windows Action |
|------------------|----------------|
| CAM1-CAM9       | Number keys 1-9 |
| STOP/PLAY       | Spacebar |
| CUT             | Ctrl+X (Cut) |
| ESC             | Ctrl+Z (Undo) |
| Jog Wheel       | Left/Right Arrow Keys |

## Architecture

The project consists of several key components:

- **`SpeedEditorEnums.cs`**: Defines all the enums for keys, LEDs, and jog modes
- **`SpeedEditorAuth.cs`**: Handles the authentication protocol with the device
- **`ISpeedEditorHandler.cs`**: Interface for handling device events
- **`SpeedEditor.cs`**: Main device communication class using HidSharp
- **`DemoHandler.cs`**: Example implementation that maps device inputs to keyboard actions
- **`Program.cs`**: Main application entry point

## Customization

To customize the key mappings, modify the `HandleKeyMappings` method in `DemoHandler.cs`. You can:

- Add new key combinations
- Change the keyboard outputs
- Add complex multi-key sequences
- Implement application-specific shortcuts

Example of adding a new mapping:

```csharp
else if (keys.Contains(SpeedEditorKey.TIMELINE) && keys.Count == 1)
{
    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
}
```

## Dependencies

- **HidSharp**: For USB HID device communication
- **InputSimulator**: For Windows keyboard input simulation

## Troubleshooting

### Device Not Found
- Ensure the Speed Editor is connected via USB
- Close DaVinci Resolve completely
- Try unplugging and reconnecting the device
- Run as Administrator if needed

### Authentication Failed
- Make sure no other application is using the device
- Try restarting the Speed Editor (unplug/replug)
- Check USB cable connection

### Input Not Working
- Ensure the target application has focus
- Check if Windows Input Simulator requires elevated privileges
- Verify the key mappings in DemoHandler.cs

## Differences from Python Version

- Uses HidSharp instead of python-hid for device communication
- Uses InputSimulator instead of pynput for keyboard simulation
- Implements proper .NET disposal patterns for resource management
- Uses C# async/await patterns for re-authentication timing

## License

This project maintains the same Apache 2.0 license as the original Python implementation by Sylvain Munaut.

## Contributing

Feel free to submit issues, feature requests, or pull requests to improve the Windows implementation.