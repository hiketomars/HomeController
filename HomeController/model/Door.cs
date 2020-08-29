using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using HomeController.comm;

namespace HomeController.model
{
    /// <summary>
    /// Represents a door in the house with sensors for the door being closed, floating and locked.
    /// Values are normally read from electronic values connected the GPIO-pin.
    /// Can also be set to use virtual values (ie software values) instead for any of the sensor values.
    /// </summary>
    public class Door : GpioConnector, IDoor
    {
        public bool UseVirtualDoorClosedSignal { get; set; }
        public bool UseVirtualDoorFloatingSignal { get; set; }
        public bool UseVirtualDoorLockedSignal { get; set; }

        public void UseVirtualIoSignals()
        {
            UseVirtualDoorClosedSignal = true;
            UseVirtualDoorFloatingSignal = true;
            UseVirtualDoorLockedSignal = true;
        }

        private bool VirtualDoorClosed { get; set; }
        private bool VirtualDoorFloating { get; set; }
        private bool VirtualDoorLocked { get; set; }

        private GpioController gpio;

        private readonly int doorOpenPinNumber = 1; // todo what number do I use?
        private readonly int doorFloatingPinNumber = 2; // todo what number do I use?
        private readonly int doorLockedPinNumber = 3; // todo what number do I use?


        public enum LedPattern {
            AlarmActiveAllDoorsLocked,          // Lugnt blinkande rött.
            AlarmActiveDoorNotLocked,           // Lugnt blinkande rött 50%, grönt 50%. 
            AlarmActiveDoorLockedButNotDoor1,   // Lugnt blinkande rött 50%. En kort grönblink 2%.
            AlarmActiveDoorLockedButNotDoor2,   // Lugnt blinkande rött 50%. Två korta grönblink 2%.
            AlarmActiveDoorLockedButNotDoor3,   // Lugnt blinkande rött 50%. Tre korta grönblink 2%.

            AlarmInactiveDoorNotLocked,         // Släckt
            AlarmInactiveDoorLocked,            // Lyser rött
            AlarmInactiveDoorLockedButNotDoor1, // Lyser rött med väldigt korta släckningar.
            AlarmInactiveDoorLockedButNotDoor2, // Lyser rött med väldigt korta dubbla släckningar.
            AlarmInactiveDoorLockedButNotDoor3  // Lyser rött med väldigt korta trippelsläckningar.
        };

        private LEDController doorLedController;

        private bool locked;

        private GpioPin doorOpenPin;
        private GpioPin doorFloatingPin;
        private GpioPin doorLockedPin;


        public RgbLed DoorLed { get; set; }

        // CHecks the statuses on the door and houseAlarm and sets the LED pattern that signals the found status.
        // todo alla statusar tas ej hänsyn till just nu.
        internal void SetAppropriteLedPattern(LcuHandler houseHandler)
        {
            if (houseHandler.AlarmIsActive)
            {
                if (IsLocked)
                {
                    doorLedController.StartLedPattern(LedPatternFactory.CreateFlash1Second50percentRed50PercentBlack());
                }
                else
                {
                    //DoorLed.TheLedPattern = LedPattern.AlarmActiveDoorNotLocked;
                }
            }
            else
            {
                if (IsLocked)
                {
                    //DoorLed.TheLedPattern = LedPattern.AlarmInactiveDoorLocked;
                }
                else
                {
                    //DoorLed.TheLedPattern = LedPattern.AlarmInactiveDoorNotLocked;
                }
            }
        }


        public bool IsOpen
        {
            get
            {
                if (UseVirtualDoorClosedSignal)
                {
                    return !VirtualDoorClosed;
                }
                return doorOpenPin.Read() == GpioPinValue.Low;
            } // todo Kan vara tvärtom...

            set { VirtualDoorClosed = !value; } // todo Kan vara tvärtom...
        }

        public bool? IsFloating
        {
            get
            {
                if(UseVirtualDoorFloatingSignal)
                {
                    return VirtualDoorFloating;
                }
                return doorFloatingPin.Read() == GpioPinValue.High;
            } // todo Kan vara tvärtom...

            set
            {
                if (value == null)
                {
                    throw new Exception("Error: Trying to set null as value for IsFloating");
                }
                VirtualDoorFloating = (bool)value;
            } // todo Kan vara tvärtom...
        }

        public bool IsLocked
        {
            get
            {
                if(UseVirtualDoorLockedSignal)
                {
                    return VirtualDoorLocked;
                }
                return doorLockedPin.Read() == GpioPinValue.High;
            } // todo Kan vara tvärtom...

            set { VirtualDoorLocked = value; } // todo Kan vara tvärtom...
        }

        public override void InitGpio()
        {
            base.InitGpio();
            doorOpenPin = gpio.OpenPin(doorOpenPinNumber); 
            doorFloatingPin = gpio.OpenPin(doorFloatingPinNumber); 
            doorLockedPin = gpio.OpenPin(doorLockedPinNumber); 
        }
    }

}
