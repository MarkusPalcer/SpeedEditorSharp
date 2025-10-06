# Troubleshooting

Common issues and solutions when working with SpeedEditorSharp.

## Connection Issues

### Device Not Found

**Symptoms:**
- `ConnectAsync()` does not return

**Solutions:**

1. **Check USB Connection**
- Verify device is physically connected
- Try different USB port or cable

2. **Close DaVinci Resolve**
DaVinci Resolve locks the device exclusively. 
Close Resolve completely before using SpeedEditorSharp.
Check Task Manager for lingering DaVinci processes.

3. **Device Recognition**
Check if Windows recognizes the device. If you plug it in, windows should play the USB connect sound
and the hardware manager should reload its list. Also no device with errors should be visible.

Sadly the SpeedEditor does not show up with its name in the device manager

## Runtime Issues

### Events Not Firing

**Symptoms:**
- Key presses not detected
- No jog wheel events
- Silent failures

**Debugging Steps:**

1. **Check Connection Status**
If you never connect to the device, you will never get any events.
You can check the `IsConnected` property while debugging to see if you are connected
or you can wait for the Task returned by `ConnectAsync` to complete to ensure connection is established.


2. **Test with Simple Handler**
   ```csharp
   speedEditor.KeyPress += (s, e) => Console.WriteLine($"Key: {e.Key}");
   ```

### LED Commands Not Working

**Symptoms:**
- LEDs don't turn on/off

**Solutions:**

1. **Check Connection First**
If you don't connect to the device, changes won't be sent to it.
As soon as you connect, the state you have created will be sent to the device.
   
## Performance Issues

### Slow Response Times

**Symptoms:**
- Delayed key press responses
- Laggy jog wheel input

**Solutions:**

1. **Lightweight Event Handlers**
```csharp
// ❌ Bad: Heavy processing in event handler
speedEditor.KeyPress += (s, e) =>
{
    DoHeavyProcessing(); // Blocks device communication
};

// ✅ Good: Offload to background thread
speedEditor.KeyPress += (s, e) =>
{
    Task.Run(() => DoHeavyProcessing());
};
```

Mind you that jog wheel updates come in pretty fast, like mouse movement events do.

2. **Reduce LED Updates**
```csharp
// ❌ Bad: Frequent LED updates
// Each set causes a data packet to be sent to the device
for (int i = 0; i < 100; i++)
{
    speedEditor.Leds.Cut = i % 2 == 0;
}

// ✅ Good: Batch LED updates
speedEditor.Leds.Cut = finalState;
```

### High CPU Usage

**Solution: Check Event Handler Performance**

```csharp
private static void OnKeyPress(object? sender, KeyEventArgs e)
{
    var stopwatch = Stopwatch.StartNew();
    
    // Your handler code here
    
    stopwatch.Stop();
    if (stopwatch.ElapsedMilliseconds > 10)
    {
        Console.WriteLine($"Slow handler: {stopwatch.ElapsedMilliseconds}ms");
    }
}
```

## Application-Specific Issues

### Windows Forms Threading

**Problem:**
- UI updates from Speed Editor events cause cross-thread exceptions

**Solution:**
```csharp
private void OnSpeedEditorKeyPress(object? sender, KeyEventArgs e)
{
    if (InvokeRequired)
    {
        Invoke(() => UpdateUI(e.Key));
    }
    else
    {
        UpdateUI(e.Key);
    }
}
```

### WPF Threading

**Problem:**
- Similar threading issues in WPF applications

**Solution:**
```csharp
private void OnSpeedEditorKeyPress(object? sender, KeyEventArgs e)
{
    Dispatcher.Invoke(() =>
    {
        statusLabel.Content = $"Key: {e.Key}";
    });
}
```

### Console Application Exit

**Problem:**
- Application exits without proper cleanup

**Solution:**
```csharp
static async Task Main(string[] args)
{
    var speedEditor = SpeedEditor.Instance;
    
    // Handle Ctrl+C gracefully
    Console.CancelKeyPress += async (s, e) =>
    {
        e.Cancel = true;
        await speedEditor.DisconnectAsync();
        Environment.Exit(0);
    };
    
    try
    {
        await speedEditor.ConnectAsync();
        // Your application logic
    }
    finally
    {
        await speedEditor.DisconnectAsync();
    }
}
```

