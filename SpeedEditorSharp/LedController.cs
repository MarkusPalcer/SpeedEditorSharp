using SpeedEditorSharp.Enums;

namespace SpeedEditorSharp
{
    /// <summary>
    /// Controller for managing LED states on the Speed Editor with individual boolean properties
    /// </summary>
    public class LedController
    {
        private readonly Action<Leds> _updateHardware;
        private Leds _currentLeds;

        internal LedController(Action<Leds> updateHardware)
        {
            _updateHardware = updateHardware;
            _currentLeds = 0;
            
            // Initialize hardware with all LEDs turned off
            _updateHardware(_currentLeds);
        }

        /// <summary>
        /// Gets or sets the Close Up LED
        /// </summary>
        public bool CloseUp
        {
            get => (_currentLeds & Leds.CLOSE_UP) != 0;
            set => SetLed(Leds.CLOSE_UP, value);
        }

        /// <summary>
        /// Gets or sets the Cut LED
        /// </summary>
        public bool Cut
        {
            get => (_currentLeds & Leds.CUT) != 0;
            set => SetLed(Leds.CUT, value);
        }

        /// <summary>
        /// Gets or sets the Dis LED
        /// </summary>
        public bool Dissolve
        {
            get => (_currentLeds & Leds.DIS) != 0;
            set => SetLed(Leds.DIS, value);
        }

        /// <summary>
        /// Gets or sets the Smooth Cut LED
        /// </summary>
        public bool SmoothCut
        {
            get => (_currentLeds & Leds.SMTH_CUT) != 0;
            set => SetLed(Leds.SMTH_CUT, value);
        }

        /// <summary>
        /// Gets or sets the Trans LED
        /// </summary>
        public bool Transition
        {
            get => (_currentLeds & Leds.TRANS) != 0;
            set => SetLed(Leds.TRANS, value);
        }

        /// <summary>
        /// Gets or sets the Snap LED
        /// </summary>
        public bool Snap
        {
            get => (_currentLeds & Leds.SNAP) != 0;
            set => SetLed(Leds.SNAP, value);
        }

        /// <summary>
        /// Gets or sets the Camera 1 LED
        /// </summary>
        public bool Cam1
        {
            get => (_currentLeds & Leds.CAM1) != 0;
            set => SetLed(Leds.CAM1, value);
        }

        /// <summary>
        /// Gets or sets the Camera 2 LED
        /// </summary>
        public bool Cam2
        {
            get => (_currentLeds & Leds.CAM2) != 0;
            set => SetLed(Leds.CAM2, value);
        }

        /// <summary>
        /// Gets or sets the Camera 3 LED
        /// </summary>
        public bool Cam3
        {
            get => (_currentLeds & Leds.CAM3) != 0;
            set => SetLed(Leds.CAM3, value);
        }

        /// <summary>
        /// Gets or sets the Camera 4 LED
        /// </summary>
        public bool Cam4
        {
            get => (_currentLeds & Leds.CAM4) != 0;
            set => SetLed(Leds.CAM4, value);
        }

        /// <summary>
        /// Gets or sets the Camera 5 LED
        /// </summary>
        public bool Cam5
        {
            get => (_currentLeds & Leds.CAM5) != 0;
            set => SetLed(Leds.CAM5, value);
        }

        /// <summary>
        /// Gets or sets the Camera 6 LED
        /// </summary>
        public bool Cam6
        {
            get => (_currentLeds & Leds.CAM6) != 0;
            set => SetLed(Leds.CAM6, value);
        }

        /// <summary>
        /// Gets or sets the Camera 7 LED
        /// </summary>
        public bool Cam7
        {
            get => (_currentLeds & Leds.CAM7) != 0;
            set => SetLed(Leds.CAM7, value);
        }

        /// <summary>
        /// Gets or sets the Camera 8 LED
        /// </summary>
        public bool Cam8
        {
            get => (_currentLeds & Leds.CAM8) != 0;
            set => SetLed(Leds.CAM8, value);
        }

        /// <summary>
        /// Gets or sets the Camera 9 LED
        /// </summary>
        public bool Cam9
        {
            get => (_currentLeds & Leds.CAM9) != 0;
            set => SetLed(Leds.CAM9, value);
        }

        /// <summary>
        /// Gets or sets the Live OWR LED
        /// </summary>
        public bool LiveOverwrite
        {
            get => (_currentLeds & Leds.LIVE_OWR) != 0;
            set => SetLed(Leds.LIVE_OWR, value);
        }

        /// <summary>
        /// Gets or sets the Video Only LED
        /// </summary>
        public bool VideoOnly
        {
            get => (_currentLeds & Leds.VIDEO_ONLY) != 0;
            set => SetLed(Leds.VIDEO_ONLY, value);
        }

        /// <summary>
        /// Gets or sets the Audio Only LED
        /// </summary>
        public bool AudioOnly
        {
            get => (_currentLeds & Leds.AUDIO_ONLY) != 0;
            set => SetLed(Leds.AUDIO_ONLY, value);
        }

        /// <summary>
        /// Switches to the specified camera LED, turning off all other camera LEDs
        /// </summary>
        /// <param name="camera">The camera to switch to</param>
        public void SwitchCameraLed(Cameras camera)
        {
            // Clear all camera LEDs first
            const Leds cameraLeds = Leds.CAM1 | Leds.CAM2 | Leds.CAM3 | Leds.CAM4 | Leds.CAM5 | 
                                    Leds.CAM6 | Leds.CAM7 | Leds.CAM8 | Leds.CAM9;
            
            _currentLeds &= ~cameraLeds;

            // Set the specified camera LED
            Leds targetLed = camera switch
            {
                Cameras.CAM1 => Leds.CAM1,
                Cameras.CAM2 => Leds.CAM2,
                Cameras.CAM3 => Leds.CAM3,
                Cameras.CAM4 => Leds.CAM4,
                Cameras.CAM5 => Leds.CAM5,
                Cameras.CAM6 => Leds.CAM6,
                Cameras.CAM7 => Leds.CAM7,
                Cameras.CAM8 => Leds.CAM8,
                Cameras.CAM9 => Leds.CAM9,
                Cameras.None => 0, // No camera selected, turn all LEDs off
                _ => throw new ArgumentOutOfRangeException(nameof(camera))
            };

            _currentLeds |= targetLed;
            _updateHardware(_currentLeds);
        }

        /// <summary>
        /// Switches to the specified transition LED, turning off all other transition LEDs
        /// </summary>
        /// <param name="transition">The transition to switch to</param>
        public void SwitchTransitionLed(Transitions transition)
        {
            // Clear all transition LEDs first
            const Leds transitionLeds = Leds.CUT | Leds.DIS | Leds.SMTH_CUT;
            
            _currentLeds &= ~transitionLeds;

            // Set the specified transition LED
            Leds targetLed = transition switch
            {
                Transitions.CUT => Leds.CUT,
                Transitions.DISSOLVE => Leds.DIS,
                Transitions.SMOOTH_CUT => Leds.SMTH_CUT,
                Transitions.NONE => 0, // No transition selected, turn all LEDs off
                _ => throw new ArgumentOutOfRangeException(nameof(transition))
            };

            _currentLeds |= targetLed;
            _updateHardware(_currentLeds);
        }

        private void SetLed(Leds ledFlag, bool state)
        {
            if (state)
            {
                _currentLeds |= ledFlag;
            }
            else
            {
                _currentLeds &= ~ledFlag;
            }

            _updateHardware(_currentLeds);
        }
    }
}