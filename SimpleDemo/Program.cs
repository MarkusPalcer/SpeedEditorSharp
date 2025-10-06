using SpeedEditorSharp;
using SimpleDemo;

Console.WriteLine($"Blackmagic Speed Editor - Windows Demo");
Console.WriteLine($"Started at: {DateTime.Now}");
Console.WriteLine("Make sure the Speed Editor is connected and NOT in use by DaVinci Resolve.");
Console.WriteLine("Press Ctrl+C to exit.");
Console.WriteLine();

try
{
    using var speedEditor = new SpeedEditor();
    
    Console.WriteLine("Speed Editor device found!");
    
    // Authenticate with the device
    int timeout = speedEditor.Authenticate();
    Console.WriteLine($"Authentication successful. Timeout: {timeout} seconds");
    
    // Set up the demo handler
    using var handler = new DemoHandler(speedEditor);
    
    Console.WriteLine("Demo handler initialized. Ready to receive input...");
    Console.WriteLine();
    Console.WriteLine("NEW BATTERY PROPERTIES DEMONSTRATION:");
    Console.WriteLine("- You can now access Charging and BatteryLevel properties directly");
    Console.WriteLine("- No need to subscribe to events if you just want to check the values");
    Console.WriteLine();
    
    // Demonstrate the new battery properties
    Console.WriteLine("LED INTERFACE DEMONSTRATION:");
    Console.WriteLine("- Camera buttons now use SwitchCameraLed() method");
    Console.WriteLine("- Other LEDs toggle individually using properties");
    Console.WriteLine("- Jog mode and LEDs controlled via speedEditor.Jog properties");
    Console.WriteLine();
    Console.WriteLine("Key mappings:");
    Console.WriteLine("- CUT: Ctrl+X");
    Console.WriteLine("- ESC: Ctrl+Z (Undo)");
    Console.WriteLine("- Jog wheel: Left/Right arrow keys");
    Console.WriteLine();
    
    // Main polling loop
    while (true)
    {
        speedEditor.Poll(100); // 100ms timeout
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Troubleshooting:");
    Console.WriteLine("1. Make sure the Blackmagic Speed Editor is connected via USB");
    Console.WriteLine("2. Close DaVinci Resolve if it's running (it locks the device)");
    Console.WriteLine("3. Try unplugging and reconnecting the device");
    Console.WriteLine("4. Run this program as Administrator if needed");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}