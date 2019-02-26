using HomeController.comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    /// <summary>
    /// The one and only instance of the HouseController class represents the house, ie not a specific LCU.
    /// Alarm and status are summarized in this class.
    /// </summary>
    public class HouseHandler : IHouseModel
    {
        public bool AlarmIsActive { get; set; }

        private LocalCentralUnit BackdoorLocalCentralUnit;

        /// <summary>
        /// Constructs the one and only HouseHandler which is the model in the MVP.
        /// </summary>
        public HouseHandler()
        {
        }

        public void InitHouseHandler() { 
            //PerformStartUpLEDFlash();
            // Should read config here but now hard coded to have contact with only one other LCU.
            BackdoorLocalCentralUnit = new LocalCentralUnit();
            BackdoorLocalCentralUnit.SetView();

            // Let us listen to changes to LED for the backdoor so that we can update the GUI.
            BackdoorLocalCentralUnit.LcuLedController.ControlledRgbLed.LEDHasChanged += EventHandler_LedHasChanged;

            BackdoorLocalCentralUnit.StartLCU_Server_Communication();
            BackdoorLocalCentralUnit.StartLCU_Client_Communication();


            if(BackdoorLocalCentralUnit.Name == "DefaultNamePleaseUpdateConfigFile")
            {
                CorrectlyInitialized = false;
            }else if(BackdoorLocalCentralUnit.Name == "BackDoor")
            {
                CorrectlyInitialized = true;
            }
        }

        // Handles the event that a LED has changed.
        private void EventHandler_LedHasChanged(RGBValue rgbValue)
        {
            OnLCULedChanged(rgbValue);
        }

        // Send the event LCULedChanged.
        private void OnLCULedChanged(RGBValue rgbValue)
        {
            if (LCULedHasChanged != null)
            {
                LCULedHasChanged(rgbValue);
            }
        }


        public bool CorrectlyInitialized { get; set; }

        public string GetInfo()
        {
            return "Currently only LCU with name " + BackdoorLocalCentralUnit.Name + " is present.";
        }

        private List<string> loggings = new List<string>();

        // Adds an item to the list of loggings.
        private void AddLogging(string text)
        {
            loggings.Add(text);
            SendEventThatModelHasChanged();
        }

        public event Definition.VoidEventHandler ModelHasChanged;
        public event Definition.LEDChangedEventHandler LCULedHasChanged;

        public List<string> GetLoggings()
        {
            List<string> backdoórLoggings = BackdoorLocalCentralUnit.GetLoggings();
            return loggings.Concat(backdoórLoggings).ToList();
        }

        public void GetColorForBackdoorLED()
        {
            BackdoorLocalCentralUnit.GetColorForLED();
        }

        private static HouseHandler houseHandler;
        public static HouseHandler GetInstance()
        {
            if (houseHandler == null)
            {
                houseHandler = new HouseHandler();
                houseHandler.InitHouseHandler();
            }

            return houseHandler;
        }

        public void SendEventThatModelHasChanged()
        {
            if (ModelHasChanged != null)
            {
                ModelHasChanged();
            }
        }
    }
}
