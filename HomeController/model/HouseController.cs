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
    public class HouseController : IHouseModel
    {
        public bool AlarmIsActive { get; set; }
        public bool AlarmIsAlarming { get; set; }

        internal void StartEntrance(Door door)
        {
            throw new NotImplementedException();
        }

        internal void RegisterEntrance(Door door)
        {
            throw new NotImplementedException();
        }

        private LocalCentralUnit BackdoorLocalCentralUnit;

        private Definition.LoggInGui loggInGui;
        /// <summary>
        /// Constructs the one and only HouseController which is the model in the MVP.
        /// </summary>
        public HouseController()
        {
            PerformStartUpLEDFlash();
            // Should read config here but now hard coded to have contact with only one other LCU.
            BackdoorLocalCentralUnit = new LocalCentralUnit();
            BackdoorLocalCentralUnit.SetView(loggInGui);
            BackdoorLocalCentralUnit.StartLCUCommunication();

            if(BackdoorLocalCentralUnit.Name == "DefaultNamePleaseUpdateConfigFile")
            {
                CorrectlyInitialized = false;
            }else if(BackdoorLocalCentralUnit.Name == "BackDoor")
            {
                CorrectlyInitialized = true;
            }
        }

        private void PerformStartUpLEDFlash()
        {

        }


        private void PerforStartSequence()
        {
            LEDController ledController = new LEDController(new RgbLed(VisualizeLedInColor), new LedFlashPattern(
                new int[] {
                            // Three fast red flashes.
                            255, 0, 0, 200,
                            0, 0, 0, 200,

                            255, 0, 0, 200,
                            0, 0, 0, 200,

                            //255, 0, 0, 200,
                            //0, 0, 0, 200,

                            0, 0, 0, 500,


                            // Three fast green flashes.
                            0, 255, 0, 200,
                            0, 0, 0, 200,

                            0, 255, 0, 200,
                            0, 0, 0, 200,

                            //0, 255, 0, 200,
                            //0, 0, 0, 200,

                            0, 0, 0, 500,


                            // Three fast blue flashes.
                            0, 0, 255, 200,
                            0, 0, 0, 200,

                            0, 0, 255, 200,
                            0, 0, 0, 200,

                            //0, 0, 255, 200,
                            //0, 0, 0, 200,

                            0, 0, 0, 2000

                            }),
                            VisualizeLedInColor);
            ledController.StartLedPattern();
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
        }

        public event Definition.VoidEventHandler ModelHasChanged;

        public List<string> GetLoggings()
        {
            return loggings;
        }

        private static HouseController houseController;
        public static HouseController GetInstance()
        {
            if (houseController == null)
            {
                houseController = new HouseController();
            }

            return houseController;
        }
    }
}
