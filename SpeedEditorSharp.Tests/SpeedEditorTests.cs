using SpeedEditorSharp.Enums;
using SpeedEditorSharp.Events;
using SpeedEditorSharp.Hardware;
using SpeedEditorSharp.Hardware.Reports;

namespace SpeedEditorSharp.Tests;

[TestClass]
public sealed class SpeedEditorTests
{
    private IHardware _mockHardware = null!;
    private SpeedEditor _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockHardware = Substitute.For<IHardware>();
        _mockHardware.Initialization.Returns(Task.CompletedTask);
        _sut = new SpeedEditor(_mockHardware);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _sut.Dispose();
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_InitializesHardware()
    {
        // Arrange & Act
        // Constructor already called in TestInitialize

        // Assert
        Assert.IsNotNull(_sut.Leds, "LED controller should be initialized");
        Assert.AreEqual(JogModes.RELATIVE_0, _sut.JogMode, "Default jog mode should be RELATIVE_0");
        Assert.AreEqual(JogLedStates.JOG, _sut.ActiveJogLed, "Default jog LED should be JOG");
        
        // Verify that hardware has been initialized on all accounts
        _mockHardware.Received().SetLedsInternal(0); // LEDs initialized as off
        _mockHardware.Received().SendJogLedStateToHardware(JogLedStates.JOG);
        _mockHardware.Received().SendJogModeToHardware(JogModes.RELATIVE_0);
    }

    #endregion

    #region Property Tests

    [TestMethod]
    public void Initialization_ReturnsHardwareInitializationTask()
    {
        // Arrange
        var expectedTask = Task.CompletedTask;
        _mockHardware.Initialization.Returns(expectedTask);

        // Act
        var result = _sut.Initialization;

        // Assert
        Assert.AreEqual(expectedTask, result, "Should return the hardware initialization task");
    }

    [TestMethod]
    public void JogMode_WhenSet_CallsHardwareAndUpdatesProperty()
    {
        // Arrange
        var newMode = JogModes.RELATIVE_2;
        _mockHardware.ClearReceivedCalls(); // Clear initialization calls

        // Act
        _sut.JogMode = newMode;

        // Assert
        Assert.AreEqual(newMode, _sut.JogMode, "Property should be updated");
        _mockHardware.Received(1).SendJogModeToHardware(newMode);
    }

    [TestMethod]
    public void JogMode_WhenSetToSameValue_DoesNotCallHardware()
    {
        // Arrange
        var currentMode = _sut.JogMode;
        _mockHardware.ClearReceivedCalls(); // Clear initialization calls

        // Act
        _sut.JogMode = currentMode;

        // Assert
        _mockHardware.DidNotReceive().SendJogModeToHardware(Arg.Any<JogModes>());
    }

    [TestMethod]
    public void ActiveJogLed_WhenSet_CallsHardwareAndUpdatesProperty()
    {
        // Arrange
        var newLedState = JogLedStates.SHTL;
        _mockHardware.ClearReceivedCalls(); // Clear initialization calls

        // Act
        _sut.ActiveJogLed = newLedState;

        // Assert
        Assert.AreEqual(newLedState, _sut.ActiveJogLed, "Property should be updated");
        _mockHardware.Received(1).SendJogLedStateToHardware(newLedState);
    }

    [TestMethod]
    public void ActiveJogLed_WhenSetToSameValue_DoesNotCallHardware()
    {
        // Arrange
        var currentLedState = _sut.ActiveJogLed;
        _mockHardware.ClearReceivedCalls(); // Clear initialization calls

        // Act
        _sut.ActiveJogLed = currentLedState;

        // Assert
        _mockHardware.DidNotReceive().SendJogLedStateToHardware(Arg.Any<JogLedStates>());
    }

    [TestMethod]
    public void BatteryProperties_InitiallyReturnDefaultValues()
    {
        // Act & Assert
        Assert.IsFalse(_sut.Charging, "Initial charging state should be false");
        Assert.AreEqual(0, _sut.BatteryLevel, "Initial battery level should be 0");
    }

    #endregion

    #region Event Tests - Battery

    [TestMethod]
    public void BatteryReport_WhenReceived_UpdatesPropertiesAndFiresEvent()
    {
        // Arrange
        var batteryEventReceived = false;
        BatteryEventArgs? receivedEventArgs = null;
        
        _sut.BatteryStatusUpdate += (_, args) =>
        {
            batteryEventReceived = true;
            receivedEventArgs = args;
        };

        var batteryReport = new BatteryUpdate { Charging = true, Level = 75 };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = batteryReport });

        // Assert
        Assert.IsTrue(batteryEventReceived, "Battery event should be fired");
        Assert.IsNotNull(receivedEventArgs, "Event args should not be null");
        Assert.IsTrue(receivedEventArgs.IsCharging, "Event should indicate charging");
        Assert.AreEqual(75, receivedEventArgs.Level, "Event should have correct battery level");
        
        Assert.IsTrue(_sut.Charging, "Charging property should be updated");
        Assert.AreEqual(75, _sut.BatteryLevel, "BatteryLevel property should be updated");
    }

    #endregion

    #region Event Tests - Jog

    [TestMethod]
    public void JogReport_WhenReceived_FiresJogEvent()
    {
        // Arrange
        var jogEventReceived = false;
        JogEventArgs? receivedEventArgs = null;
        
        _sut.JogWheelMoved += (_, args) =>
        {
            jogEventReceived = true;
            receivedEventArgs = args;
        };

        var jogReport = new JogUpdate { Mode = JogModes.ABSOLUTE_CONTINUOUS, Value = 5 };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = jogReport });

        // Assert
        Assert.IsTrue(jogEventReceived, "Jog event should be fired");
        Assert.IsNotNull(receivedEventArgs, "Event args should not be null");
        Assert.AreEqual(JogModes.ABSOLUTE_CONTINUOUS, receivedEventArgs.Modes, "Event should have correct jog mode");
        Assert.AreEqual(5, receivedEventArgs.Value, "Event should have correct jog value");
    }

    #endregion

    #region Event Tests - Keys

    [TestMethod]
    public void KeyReport_SingleKeyPressed_FiresKeyDownEvent()
    {
        // Arrange
        var keyDownEventReceived = false;
        KeyEventArgs? receivedEventArgs = null;
        
        _sut.KeyDown += (_, args) =>
        {
            keyDownEventReceived = true;
            receivedEventArgs = args;
        };

        var keyReport = new KeyUpdate { Keys = [Keys.STOP_PLAY] };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = keyReport });

        // Assert
        Assert.IsTrue(keyDownEventReceived, "KeyDown event should be fired");
        Assert.IsNotNull(receivedEventArgs, "Event args should not be null");
        Assert.AreEqual(Keys.STOP_PLAY, receivedEventArgs.Key, "Event should have correct key");
    }

    [TestMethod]
    public void KeyReport_KeyReleased_FiresKeyUpAndKeyPressEvents()
    {
        // Arrange
        var keyUpEventReceived = false;
        var keyPressEventReceived = false;
        KeyEventArgs? keyUpEventArgs = null;
        KeyEventArgs? keyPressEventArgs = null;
        
        _sut.KeyUp += (_, args) =>
        {
            keyUpEventReceived = true;
            keyUpEventArgs = args;
        };
        
        _sut.KeyPress += (_, args) =>
        {
            keyPressEventReceived = true;
            keyPressEventArgs = args;
        };

        // First press the key
        var keyPressReport = new KeyUpdate { Keys = [Keys.STOP_PLAY] };
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = keyPressReport });

        // Then release the key
        var keyReleaseReport = new KeyUpdate { Keys = [] };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = keyReleaseReport });

        // Assert
        Assert.IsTrue(keyUpEventReceived, "KeyUp event should be fired");
        Assert.IsNotNull(keyUpEventArgs, "KeyUp event args should not be null");
        Assert.AreEqual(Keys.STOP_PLAY, keyUpEventArgs.Key, "KeyUp event should have correct key");
        
        Assert.IsTrue(keyPressEventReceived, "KeyPress event should be fired");
        Assert.IsNotNull(keyPressEventArgs, "KeyPress event args should not be null");
        Assert.AreEqual(Keys.STOP_PLAY, keyPressEventArgs.Key, "KeyPress event should have correct key");
    }

    [TestMethod]
    public void KeyReport_MultipleKeysPressed_FiresMultipleKeyDownEvents()
    {
        // Arrange
        var keyDownEvents = new List<Keys>();
        
        _sut.KeyDown += (_, args) => keyDownEvents.Add(args.Key);

        var keyReport = new KeyUpdate { Keys = [Keys.STOP_PLAY, Keys.CUT, Keys.SOURCE] };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = keyReport });

        // Assert
        Assert.AreEqual(3, keyDownEvents.Count, "Should fire three KeyDown events");
        Assert.IsTrue(keyDownEvents.Contains(Keys.STOP_PLAY), "Should fire KeyDown for STOP_PLAY");
        Assert.IsTrue(keyDownEvents.Contains(Keys.CUT), "Should fire KeyDown for CUT");
        Assert.IsTrue(keyDownEvents.Contains(Keys.SOURCE), "Should fire KeyDown for SOURCE");
    }

    [TestMethod]
    public void KeyReport_PartialKeyRelease_FiresCorrectEvents()
    {
        // Arrange
        var keyDownEvents = new List<Keys>();
        var keyUpEvents = new List<Keys>();
        var keyPressEvents = new List<Keys>();
        
        _sut.KeyDown += (_, args) => keyDownEvents.Add(args.Key);
        _sut.KeyUp += (_, args) => keyUpEvents.Add(args.Key);
        _sut.KeyPress += (_, args) => keyPressEvents.Add(args.Key);

        // First press multiple keys
        var keyPressReport = new KeyUpdate { Keys = [Keys.STOP_PLAY, Keys.CUT] };
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = keyPressReport });

        // Clear the lists to focus on the release
        keyDownEvents.Clear();
        keyUpEvents.Clear();
        keyPressEvents.Clear();

        // Then release one key but keep the other pressed
        var partialReleaseReport = new KeyUpdate { Keys = [Keys.STOP_PLAY] };

        // Act
        _mockHardware.ReportReceived += Raise.EventWith(new ReportReceivedEventArgs { Report = partialReleaseReport });

        // Assert
        Assert.AreEqual(0, keyDownEvents.Count, "No new KeyDown events should be fired");
        Assert.AreEqual(1, keyUpEvents.Count, "One KeyUp event should be fired");
        Assert.AreEqual(Keys.CUT, keyUpEvents[0], "KeyUp should be for CUT key");
        Assert.AreEqual(1, keyPressEvents.Count, "One KeyPress event should be fired");
        Assert.AreEqual(Keys.CUT, keyPressEvents[0], "KeyPress should be for CUT key");
    }

    #endregion

    #region LED Controller Integration

    [TestMethod]
    public void LedController_WhenUsed_CallsHardwareSetLedsInternal()
    {
        // Arrange
        _mockHardware.ClearReceivedCalls(); // Clear initialization calls

        // Act
        _sut.Leds.Cut = true;

        // Assert
        _mockHardware.Received().SetLedsInternal(Arg.Any<Leds>());
    }

    #endregion

    #region Dispose Tests

    [TestMethod]
    public void Dispose_CallsHardwareDispose()
    {
        // Act
        _sut.Dispose();

        // Assert
        _mockHardware.Received(1).Dispose();
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_OnlyDisposesHardwareOnce()
    {
        // Act
        _sut.Dispose();
        _sut.Dispose();
        _sut.Dispose();

        // Assert
        _mockHardware.Received(1).Dispose();
    }

    #endregion
}