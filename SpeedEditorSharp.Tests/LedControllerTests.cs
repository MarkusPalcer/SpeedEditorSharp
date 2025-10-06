using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp.Tests;

[TestClass]
public sealed class LedControllerTests
{
    private readonly List<Leds> _ledStateUpdates = new();
    private LedController _sut = null!;
    private bool _isConnected = false;

    [TestInitialize]
    public void TestInitialize()
    {
        _ledStateUpdates.Clear();
        _isConnected = true;
        _sut = new(_ledStateUpdates.Add, () => _isConnected);
    }

    [TestMethod]
    public void Constructor_InitializesWithAllLedsOffWithoutCallingHardware()
    {
        // Arrange & Act
        // Constructor already called in TestInitialize
        
        // Assert
        Assert.AreEqual(0, _ledStateUpdates.Count, "Constructor should not call hardware");
        Assert.IsFalse(_sut.CloseUp, "CloseUp LED should be off by default");
        Assert.IsFalse(_sut.Cut, "Cut LED should be off by default");
        // All LEDs should be off by default
    }

    #region Individual LED Property Tests

    [TestMethod]
    public void CloseUp_SetTrueWhenConnected_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.CloseUp = true;

        // Assert
        Assert.IsTrue(_sut.CloseUp, "CloseUp should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CLOSE_UP, _ledStateUpdates[0], "CloseUp LED should be set");
    }

    [TestMethod]
    public void CloseUp_SetTrueWhenDisconnected_UpdatesPropertyButNotHardware()
    {
        // Arrange
        _isConnected = false;
        _ledStateUpdates.Clear();

        // Act
        _sut.CloseUp = true;

        // Assert
        Assert.IsTrue(_sut.CloseUp, "CloseUp should return true");
        Assert.AreEqual(0, _ledStateUpdates.Count, "Hardware should not be updated when disconnected");
    }

    [TestMethod]
    public void SyncToHardware_WhenConnected_SendsCurrentStateToHardware()
    {
        // Arrange
        _isConnected = false;
        _sut.CloseUp = true; // Set some state while disconnected
        _sut.Cut = true;
        _ledStateUpdates.Clear();
        _isConnected = true; // Now connect

        // Act
        _sut.SyncToHardware();

        // Assert
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CLOSE_UP | Leds.CUT, _ledStateUpdates[0], "Current LED state should be synced");
    }

    [TestMethod]
    public void SyncToHardware_WhenDisconnected_DoesNotCallHardware()
    {
        // Arrange
        _isConnected = false;
        _sut.CloseUp = true;
        _ledStateUpdates.Clear();

        // Act
        _sut.SyncToHardware();

        // Assert
        Assert.AreEqual(0, _ledStateUpdates.Count, "Hardware should not be called when disconnected");
    }

    [TestMethod]
    public void CloseUp_SetFalse_UpdatesHardwareCorrectly()
    {
        // Arrange
        _sut.CloseUp = true;
        _ledStateUpdates.Clear();

        // Act
        _sut.CloseUp = false;

        // Assert
        Assert.IsFalse(_sut.CloseUp, "CloseUp should return false");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual((Leds)0, _ledStateUpdates[0], "CloseUp LED should be cleared");
    }

    [TestMethod]
    public void Cut_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Cut = true;

        // Assert
        Assert.IsTrue(_sut.Cut, "Cut should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CUT, _ledStateUpdates[0], "Cut LED should be set");
    }

    [TestMethod]
    public void Cut_SetFalse_UpdatesHardwareCorrectly()
    {
        // Arrange
        _sut.Cut = true;
        _ledStateUpdates.Clear();

        // Act
        _sut.Cut = false;

        // Assert
        Assert.IsFalse(_sut.Cut, "Cut should return false");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual((Leds)0, _ledStateUpdates[0], "Cut LED should be cleared");
    }

    [TestMethod]
    public void Dissolve_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Dissolve = true;

        // Assert
        Assert.IsTrue(_sut.Dissolve, "Dissolve should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.DIS, _ledStateUpdates[0], "Dissolve LED should be set");
    }

    [TestMethod]
    public void SmoothCut_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.SmoothCut = true;

        // Assert
        Assert.IsTrue(_sut.SmoothCut, "SmoothCut should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.SMTH_CUT, _ledStateUpdates[0], "SmoothCut LED should be set");
    }

    [TestMethod]
    public void Transition_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Transition = true;

        // Assert
        Assert.IsTrue(_sut.Transition, "Transition should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.TRANS, _ledStateUpdates[0], "Transition LED should be set");
    }

    [TestMethod]
    public void Snap_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Snap = true;

        // Assert
        Assert.IsTrue(_sut.Snap, "Snap should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.SNAP, _ledStateUpdates[0], "Snap LED should be set");
    }

    [TestMethod]
    public void Cam1_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Cam1 = true;

        // Assert
        Assert.IsTrue(_sut.Cam1, "Cam1 should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM1, _ledStateUpdates[0], "Cam1 LED should be set");
    }

    [TestMethod]
    public void Cam2_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Cam2 = true;

        // Assert
        Assert.IsTrue(_sut.Cam2, "Cam2 should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM2, _ledStateUpdates[0], "Cam2 LED should be set");
    }

    [TestMethod]
    public void LiveOverwrite_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.LiveOverwrite = true;

        // Assert
        Assert.IsTrue(_sut.LiveOverwrite, "LiveOverwrite should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.LIVE_OWR, _ledStateUpdates[0], "LiveOverwrite LED should be set");
    }

    [TestMethod]
    public void VideoOnly_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.VideoOnly = true;

        // Assert
        Assert.IsTrue(_sut.VideoOnly, "VideoOnly should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.VIDEO_ONLY, _ledStateUpdates[0], "VideoOnly LED should be set");
    }

    [TestMethod]
    public void AudioOnly_SetTrue_UpdatesHardwareCorrectly()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.AudioOnly = true;

        // Assert
        Assert.IsTrue(_sut.AudioOnly, "AudioOnly should return true");
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.AUDIO_ONLY, _ledStateUpdates[0], "AudioOnly LED should be set");
    }

    #endregion

    #region Multiple LED Tests

    [TestMethod]
    public void MultipleLeds_SetIndependently_MaintainCombinedState()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.Cut = true;
        _sut.Cam1 = true;
        _sut.VideoOnly = true;

        // Assert
        Assert.IsTrue(_sut.Cut, "Cut should be true");
        Assert.IsTrue(_sut.Cam1, "Cam1 should be true");
        Assert.IsTrue(_sut.VideoOnly, "VideoOnly should be true");
        
        Assert.AreEqual(3, _ledStateUpdates.Count, "Hardware should be updated three times");
        Assert.AreEqual(Leds.CUT, _ledStateUpdates[0], "First update should be Cut");
        Assert.AreEqual(Leds.CUT | Leds.CAM1, _ledStateUpdates[1], "Second update should be Cut + Cam1");
        Assert.AreEqual(Leds.CUT | Leds.CAM1 | Leds.VIDEO_ONLY, _ledStateUpdates[2], "Third update should be all three");
    }

    [TestMethod]
    public void TurnOffOneLed_PreservesOtherLeds()
    {
        // Arrange
        _sut.Cut = true;
        _sut.Cam1 = true;
        _sut.VideoOnly = true;
        _ledStateUpdates.Clear();

        // Act
        _sut.Cut = false;

        // Assert
        Assert.IsFalse(_sut.Cut, "Cut should be false");
        Assert.IsTrue(_sut.Cam1, "Cam1 should still be true");
        Assert.IsTrue(_sut.VideoOnly, "VideoOnly should still be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM1 | Leds.VIDEO_ONLY, _ledStateUpdates[0], "Should preserve other LEDs");
    }

    #endregion

    #region Camera Switch Tests

    [TestMethod]
    public void SwitchCameraLed_Cam1_TurnsOnOnlyCam1()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchCameraLed(Cameras.CAM1);

        // Assert
        Assert.IsTrue(_sut.Cam1, "Cam1 should be true");
        Assert.IsFalse(_sut.Cam2, "Cam2 should be false");
        Assert.IsFalse(_sut.Cam3, "Cam3 should be false");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM1, _ledStateUpdates[0], "Only Cam1 LED should be set");
    }

    [TestMethod]
    public void SwitchCameraLed_FromCam1ToCam2_SwitchesCorrectly()
    {
        // Arrange
        _sut.SwitchCameraLed(Cameras.CAM1);
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchCameraLed(Cameras.CAM2);

        // Assert
        Assert.IsFalse(_sut.Cam1, "Cam1 should be false");
        Assert.IsTrue(_sut.Cam2, "Cam2 should be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM2, _ledStateUpdates[0], "Only Cam2 LED should be set");
    }

    [TestMethod]
    public void SwitchCameraLed_WithOtherLedsOn_PreservesNonCameraLeds()
    {
        // Arrange
        _sut.Cut = true;
        _sut.VideoOnly = true;
        _sut.SwitchCameraLed(Cameras.CAM1);
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchCameraLed(Cameras.CAM3);

        // Assert
        Assert.IsTrue(_sut.Cut, "Cut should still be true");
        Assert.IsTrue(_sut.VideoOnly, "VideoOnly should still be true");
        Assert.IsFalse(_sut.Cam1, "Cam1 should be false");
        Assert.IsTrue(_sut.Cam3, "Cam3 should be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CUT | Leds.VIDEO_ONLY | Leds.CAM3, _ledStateUpdates[0], "Should preserve non-camera LEDs");
    }

    [TestMethod]
    public void SwitchCameraLed_None_TurnsOffAllCameraLeds()
    {
        // Arrange
        _sut.SwitchCameraLed(Cameras.CAM1);
        _sut.Cut = true; // Add a non-camera LED
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchCameraLed(Cameras.None);

        // Assert
        Assert.IsFalse(_sut.Cam1, "Cam1 should be false");
        Assert.IsFalse(_sut.Cam2, "Cam2 should be false");
        Assert.IsTrue(_sut.Cut, "Cut should still be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CUT, _ledStateUpdates[0], "Only non-camera LEDs should remain");
    }

    [TestMethod]
    [DataRow(Cameras.CAM1, Leds.CAM1)]
    [DataRow(Cameras.CAM2, Leds.CAM2)]
    [DataRow(Cameras.CAM3, Leds.CAM3)]
    [DataRow(Cameras.CAM4, Leds.CAM4)]
    [DataRow(Cameras.CAM5, Leds.CAM5)]
    [DataRow(Cameras.CAM6, Leds.CAM6)]
    [DataRow(Cameras.CAM7, Leds.CAM7)]
    [DataRow(Cameras.CAM8, Leds.CAM8)]
    [DataRow(Cameras.CAM9, Leds.CAM9)]
    public void SwitchCameraLed_AllCameras_MapsCorrectly(Cameras camera, Leds expectedLed)
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchCameraLed(camera);

        // Assert
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(expectedLed, _ledStateUpdates[0], $"Camera {camera} should map to LED {expectedLed}");
    }

    [TestMethod]
    public void SwitchCameraLed_InvalidCamera_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidCamera = (Cameras)999;

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => _sut.SwitchCameraLed(invalidCamera));
    }

    #endregion

    #region Transition Switch Tests

    [TestMethod]
    public void SwitchTransitionLed_Cut_TurnsOnOnlyCut()
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchTransitionLed(Transitions.CUT);

        // Assert
        Assert.IsTrue(_sut.Cut, "Cut should be true");
        Assert.IsFalse(_sut.Dissolve, "Dissolve should be false");
        Assert.IsFalse(_sut.SmoothCut, "SmoothCut should be false");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CUT, _ledStateUpdates[0], "Only Cut LED should be set");
    }

    [TestMethod]
    public void SwitchTransitionLed_FromCutToDissolve_SwitchesCorrectly()
    {
        // Arrange
        _sut.SwitchTransitionLed(Transitions.CUT);
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchTransitionLed(Transitions.DISSOLVE);

        // Assert
        Assert.IsFalse(_sut.Cut, "Cut should be false");
        Assert.IsTrue(_sut.Dissolve, "Dissolve should be true");
        Assert.IsFalse(_sut.SmoothCut, "SmoothCut should be false");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.DIS, _ledStateUpdates[0], "Only Dissolve LED should be set");
    }

    [TestMethod]
    public void SwitchTransitionLed_WithOtherLedsOn_PreservesNonTransitionLeds()
    {
        // Arrange
        _sut.Cam1 = true;
        _sut.VideoOnly = true;
        _sut.SwitchTransitionLed(Transitions.CUT);
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchTransitionLed(Transitions.SMOOTH_CUT);

        // Assert
        Assert.IsTrue(_sut.Cam1, "Cam1 should still be true");
        Assert.IsTrue(_sut.VideoOnly, "VideoOnly should still be true");
        Assert.IsFalse(_sut.Cut, "Cut should be false");
        Assert.IsTrue(_sut.SmoothCut, "SmoothCut should be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM1 | Leds.VIDEO_ONLY | Leds.SMTH_CUT, _ledStateUpdates[0], "Should preserve non-transition LEDs");
    }

    [TestMethod]
    public void SwitchTransitionLed_None_TurnsOffAllTransitionLeds()
    {
        // Arrange
        _sut.SwitchTransitionLed(Transitions.CUT);
        _sut.Cam1 = true; // Add a non-transition LED
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchTransitionLed(Transitions.NONE);

        // Assert
        Assert.IsFalse(_sut.Cut, "Cut should be false");
        Assert.IsFalse(_sut.Dissolve, "Dissolve should be false");
        Assert.IsFalse(_sut.SmoothCut, "SmoothCut should be false");
        Assert.IsTrue(_sut.Cam1, "Cam1 should still be true");
        
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(Leds.CAM1, _ledStateUpdates[0], "Only non-transition LEDs should remain");
    }

    [TestMethod]
    [DataRow(Transitions.CUT, Leds.CUT)]
    [DataRow(Transitions.DISSOLVE, Leds.DIS)]
    [DataRow(Transitions.SMOOTH_CUT, Leds.SMTH_CUT)]
    public void SwitchTransitionLed_AllTransitions_MapsCorrectly(Transitions transition, Leds expectedLed)
    {
        // Arrange
        _ledStateUpdates.Clear();

        // Act
        _sut.SwitchTransitionLed(transition);

        // Assert
        Assert.AreEqual(1, _ledStateUpdates.Count, "Hardware should be updated once");
        Assert.AreEqual(expectedLed, _ledStateUpdates[0], $"Transition {transition} should map to LED {expectedLed}");
    }

    [TestMethod]
    public void SwitchTransitionLed_InvalidTransition_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidTransition = (Transitions)999;

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => _sut.SwitchTransitionLed(invalidTransition));
    }

    #endregion
}