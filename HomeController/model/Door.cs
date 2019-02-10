using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;

namespace HomeController.model
{
    public class Door : GpioConnector, IDoor
    {
        public bool Closed { get; set; }
        public bool Sealed { get; set; } //Reglad = haspad
        public bool Locked { get; set; }
        public bool SabotageIntact { get; set; }
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
        public Door()
        {
        }

        public RgbLed DoorLed { get; set; }

        // CHecks the statuses on the door and houseAlarm and sets the LED pattern that signals the found status.
        // todo alla statusar tas ej hänsyn till just nu.
        internal void SetAppropriteLedPattern(HouseHandler houseHandler)
        {
            if (houseHandler.AlarmIsActive)
            {
                if (Locked)
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
                if (Locked)
                {
                    //DoorLed.TheLedPattern = LedPattern.AlarmInactiveDoorLocked;
                }
                else
                {
                    //DoorLed.TheLedPattern = LedPattern.AlarmInactiveDoorNotLocked;
                }
            }
        }

        internal bool IsDetectedOpenAtThisPoll()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool IsOpen { get; set; }
        public override void InitGpio()
        {
            throw new NotImplementedException();
        }
    }

}
