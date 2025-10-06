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
    
    Console.WriteLine("Connecting to Speed Editor device...");
    
    // Connect to the device
    await speedEditor.ConnectAsync();
    Console.WriteLine("Connected and initialized successfully.");
    
    // Set up the demo handler
    using var demoHandler = new DemoHandler(speedEditor);
    
    await Task.Delay(Timeout.Infinite);
}
catch (InvalidOperationException)
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