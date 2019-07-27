using HomeController.comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.config;
using HomeController.utils;

namespace HomeController.model
{
    /// <summary>
    /// The one and only instance of the LcuHandler class.
    /// Alarm and status are summarized in this class.
    /// </summary>
    public class LcuHandler : IHouseModel
    {
        //private readonly LocalCentralUnit lcu;
        public bool AlarmIsActive { get; set; }

        private List<ILocalCentralUnit> lcuList = new List<ILocalCentralUnit>();

        /// <summary>
        /// Constructs the one and only LcuHandler which is the model in the MVP.
        /// </summary>
        public LcuHandler()
        {
            // Currently the config handlers are construncted from hard coded data but later on this will be read from XML-file(s).
            ConfigHandler configHandlerFrontDoor = new ConfigHandler("FrontLCU", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Baksidan", "2","localhost", "1340", "1341"),
                }
            );
            var frontDoorLcu = new LocalCentralUnit(configHandlerFrontDoor);
            lcuList.Add(frontDoorLcu);

            ConfigHandler configHandlerBackDoor = new ConfigHandler("BackLCU", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Framsidan", "1","localhost", "1341", "1340"),
                }
            );
            var backDoorLcu = new LocalCentralUnit(configHandlerBackDoor);
            lcuList.Add(backDoorLcu);

            //foreach(var lcu in lcuList)
            //{
            //    var lcuUserControl = new LcuUserControl();
            //    lcuUserControl.NameText = lcu.Name;
            //    lcuUserControl.AddTextToOutput("Lcu " + lcu.Name + " created.");
            //    //MainStackPanel.Children.Add(lcuUserControl);

            //}
            SendEventThatLcuInstancesHasChanged();
        }

        public void InitLcuHandler() { 
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
        public event Definition.VoidEventHandler LcuInstancesHasChanged;
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

        // This is mainly intended for debug purpose.
        // This is called when the user clicks on a direct Connect-button to test the connection.
        public void ConnectToRemoteLCU()
        {
            lcuList[0].LcuRemoteCentralUnitsController.ConnectToOnlyRcu();//todo Ska inte ha kvar denna ref till 0
        }

        public void ListenToRemoteLCU()
        {
            lcuList[0].LcuRemoteCentralUnitsController.ListenToTheOnlyRcu();//todo Ska inte ha kvar denna ref till 0
        }

        // Returns the list of the Lcu:s that this LcuHandler handles.
        public List<ILocalCentralUnit> GetLcuList()
        {
            return lcuList;
        }

        private static LcuHandler lcuHandler;
        public static LcuHandler GetInstance()
        {
            if (lcuHandler == null)
            {
                lcuHandler = new LcuHandler();
            }

            return lcuHandler;
        }

        public void SendEventThatModelHasChanged()
        {
            if (ModelHasChanged != null)
            {
                ModelHasChanged();
            }
        }
        public void SendEventThatLcuInstancesHasChanged()
        {
            if (LcuInstancesHasChanged != null)
            {
                LcuInstancesHasChanged();
            }
        }
    }
}
