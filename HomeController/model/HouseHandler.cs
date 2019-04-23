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
        //private readonly LocalCentralUnit lcu;
        public bool AlarmIsActive { get; set; }

        //private RemoteCentralUnit BackdoorRemoteCentralUnit;

        /// <summary>
        /// Constructs the one and only HouseHandler which is the model in the MVP.
        /// </summary>
        public HouseHandler()
        {
        }

        public void InitHouseHandler() { 
            //PerformStartUpLEDFlash();
            // Should read config here but now hard coded to have contact with only one other LCU.
            //BackdoorRemoteCentralUnit = new RemoteCentralUnit("", "", "");//todo
            //BackdoorRemoteCentralUnit.SetView();

            // Let us listen to changes to LED for the backdoor so that we can update the GUI.
            //BackdoorRemoteCentralUnit.LcuLedController.ControlledRgbLed.LEDHasChanged += EventHandler_LedHasChanged;
            //BackdoorRemoteCentralUnit.RemoteLcuStatusHasChanged += BackdoorRemoteCentralUnit_RemoteLcuStatusHasChanged;

            //lcu.StartListeningOnRemoteLcu();
            //lcu.SendCommandToRemoteLcu("LCU_STARTED: " + lcu.Name + ";");


            //if(BackdoorRemoteCentralUnit.Name == "DefaultNamePleaseUpdateConfigFile")
            //{
            //    CorrectlyInitialized = false;
            //}else if(BackdoorRemoteCentralUnit.Name == "BackDoor")
            //{
            //    CorrectlyInitialized = true;
            //}
        }

        //private void BackdoorRemoteCentralUnit_RemoteLcuStatusHasChanged(string todoType)
        //{
        //    throw new NotImplementedException();
        //}

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

        //public string GetInfo()
        //{
        //    return "Currently only LCU with name " + BackdoorRemoteCentralUnit.Name + " is present.";
        //}

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
            //todo List<string> backdoórLoggings = BackdoorRemoteCentralUnit.GetLoggings();
            //todo return loggings.Concat(backdoórLoggings).ToList();
            return null;
        }

        public void GetColorForBackdoorLED()
        {
            //todo BackdoorRemoteCentralUnit.GetColorForLED();
        }

        public void ConnectToRemoteLCU()
        {
            //lcu.SetupRemoteLCUCommunication();
        }

        private static HouseHandler houseHandler;
        public static HouseHandler GetInstance()
        {
            if (houseHandler == null)
            {
                houseHandler = new HouseHandler();
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
