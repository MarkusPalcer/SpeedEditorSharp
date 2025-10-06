using SimpleDemo;
using SpeedEditorSharp;

Console.WriteLine($"Blackmagic Speed Editor - Windows Demo");
Console.WriteLine($"Started at: {DateTime.Now}");
Console.WriteLine("Connecting to SpeedEditor");
Console.WriteLine("Make sure the Speed Editor is connected and NOT in use by DaVinci Resolve.");
Console.WriteLine("Press Ctrl+C to exit.");
Console.WriteLine();

Console.CancelKeyPress += (sender, e) =>
{
    Environment.Exit(0);
};

try
{
    using var speedEditor = SpeedEditor.Instance;
    
    Console.WriteLine("Speed Editor device found!");
    
    // Authenticate with the device
    await speedEditor.Initialization;
    Console.WriteLine("Initialization successful.");
    
    // Set up the demo handler
    using var demoHandler = new DemoHandler(speedEditor);
    
    await Task.Delay(Timeout.Infinite);
}
catch (InvalidOperationException ex)
{
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