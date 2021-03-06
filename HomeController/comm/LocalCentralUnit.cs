﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HomeController.utils;
using Windows.Data.Json;
using System.Net;
using System.Diagnostics;
using Windows.UI.Core;
using HomeController.model;
using Windows.UI.Xaml;
using System.Threading;
using Windows.System.Threading;
using HomeController.config;
using Moq;

namespace HomeController.comm {

    /// <summary>
    /// Local Central Unit is in control of the local equipment.
    /// Typically this is a door with a LED, a siren. Also it has communication channels to other LCU:s, RemoteCentralUnits.
    /// </summary>
    public class LocalCentralUnit : ILocalCentralUnit
    {
        //private DispatcherTimer timer;
        private ThreadPoolTimer SurveillancePoolTimer;
        public IAlarmHandler LcuAlarmHandler { get; set; }
        public LedHandler LcuLedHandler;

        private IRemoteCentralUnitsController lcuRemoteCentralUnitsController;

        //public HomeStatus CurrentHomeStatus;

        // Latest status that has been gathered from all RCU:s and the LCU.
        public CompoundStatus LatestCompoundStatus;

        
        public IRemoteCentralUnitsController LcuRemoteCentralUnitsController
        {
            get
            {
                if (lcuRemoteCentralUnitsController == null)
                {
                    throw new Exception("Should never occur. A Controller should always have been created in the constructor");
                    lcuRemoteCentralUnitsController = new RemoteCentralUnitsController(this, LcuConfigHandler.GetRemoteLcus());
                }

                return lcuRemoteCentralUnitsController;
            }
            set { lcuRemoteCentralUnitsController = value; }
        }

        public bool UseAnyMockedDoorProperty
        {
            get => UseVirtualDoorOpen || UseVirtualDoorFloating || UseVirtualDoorLocked;
            //set
            //{
            //    var oldValue = useAnyMockedDoorProperty;
            //    useAnyMockedDoorProperty = value;
            //    if (oldValue != useAnyMockedDoorProperty)
            //    {
            //        // Setting has changed.
            //        Door = HouseModelFactory.GetDoor(this);
            //    }
            //}
        }

        public bool UseVirtualDoorOpen { get; set; }
        public bool UseVirtualDoorFloating { get; set; }
        public bool UseVirtualDoorLocked { get; set; }

        public bool? IsSabotaged { get; }

        public void OnRcuReceivedMessage(IRemoteCentralUnitProxy rcu, Definition.MessageType messageType, string loggMessage)
        {
            lcuHandler.OnRcuReceivedMessage(this, rcu, messageType, loggMessage);
        }

        public void Action(string rcuName, string action)
        {
            var rcu = lcuRemoteCentralUnitsController.RcuList.Find(r => r.NameOfRemoteLcu == rcuName);

            rcu.Action(action);


        }

        public string PortNumber
        {
            get => LcuConfigHandler.LcuPortNumber;
        }

        //public ConfigHandler LcuConfigHandler;

        //private static LocalCentralUnit instance;

        //public static LocalCentralUnit GetInstance()
        //{
        //    if (instance != null)
        //    {
        //        return instance;
        //    }
        //    return new LocalCentralUnit(null);//todo 190418
        //}

        // Can be used to inject a config handler before
        public IConfigHandler LcuConfigHandler { get; set; }

        // Constructor for normal use (not unit tests).
        //public LocalCentralUnit() : this(new ConfigHandler())
        //{
        //}

        //public IRemoteCentralUnitsController RemoteCentralUnitsController { get; set; }

        //public LocalCentralUnit(): this(new ConfigHandler())
        //{
        //}

        private readonly ILcuHandler lcuHandler;

        public LocalCentralUnit(ILcuHandler lcuHandler, IConfigHandler configHandler)
        {
            Logger.Logg(configHandler.GetLCUName(), Logger.LCU_Cat,
                "Creating Lcu with name " + configHandler.GetLCUName());

            this.lcuHandler = lcuHandler;
            this.LcuConfigHandler = configHandler;
            SetName();

            // The HouseModelFactory is used to retrieve correct object depending on if this object is created for a normal execution or for a unit test.
            // Door
            var door = HouseModelFactory.GetDoor(this);
            var doorController = new DoorController();

            // LED
            var doorLed = HouseModelFactory.GetRgbLed(this);
            var ledController = new LEDController(this, doorLed);

            // Remote Central Unit Controller
            LcuRemoteCentralUnitsController = new RemoteCentralUnitsController(this, configHandler.GetRemoteLcus());
LcuRemoteCentralUnitsController.Setup(this);
            // Siren
            var lcuSiren = HouseModelFactory.GetSiren(this);
            var lcuSirenController = new SirenController(this);

            SetupLcu(door, doorLed, ledController, LcuRemoteCentralUnitsController, lcuSiren, lcuSirenController);
            //LcuRemoteCentralUnitsController.SendStartUpMessage();
        }

        // Constructor for unit tests.
        public LocalCentralUnit(ILcuHandler lcuHandler, IRgbLed doorLed, ILEDController ledController, IDoor door, IRemoteCentralUnitsController remoteCentralUnitsController, ISiren siren, ISirenController sirenController)
        {
            this.lcuHandler = lcuHandler;
            SetupLcu(door, doorLed, ledController, remoteCentralUnitsController, siren, sirenController);
        }

        private void SetupLcu(IDoor door, IRgbLed doorLed, ILEDController ledController, IRemoteCentralUnitsController remoteCentralUnitsController, ISiren siren, ISirenController sirenController)
        {
            // LED
            DoorLed = doorLed;
            LcuLedController = ledController;

            // Door
            Door = door;
            //DoorController = doorController;
            LcuDoorController = new DoorController();
            LcuDoorController.Door = Door;

            // Siren
            //LcuSiren = siren;
            LcuSirenController = sirenController;
            LcuSirenController.Siren = siren;

            LcuAlarmHandler = new AlarmHandler(this);
            LcuLedHandler = new LedHandler(this);

            // Remote central units
            LcuRemoteCentralUnitsController = remoteCentralUnitsController;
            LcuRemoteCentralUnitsController.Lcu = this;

            // Perform start sequence.
            //LcuLedController.PerformStartUpLedFlash();


            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(1000);
            //timer.Tick += Timer_Tick;


        }

        public ISirenController LcuSirenController { get; set; }

        public void StartSurveillance(bool activateCommunications = true)
        {
            // Start never ending timer loop
            SurveillancePoolTimer = ThreadPoolTimer.CreatePeriodicTimer(SurveillancePoolTimerElapsedHandler, TimeSpan.FromMilliseconds(1000));
            if (activateCommunications)
            {
                ActivateCommunication();

                Logger.Logg(Name,Logger.LCU_Cat, "Starting surveillance and activating communications.");
            }
            else
            {
                Logger.Logg(Name, Logger.LCU_Cat, "Starting surveillance but NOT activating communications.");
            }
        }

        public void ActivateCommunication()
        {
            LcuRemoteCentralUnitsController.ActivateCommunicationOnAllProxys();
        }

        public void StopSurveillance()
        {
            SurveillancePoolTimer.Cancel();

        }

        // This is the central surveillance loop.
        // It is typically called once a second or more and checks for local and remote statuses and might start actions as a reaction of that.
        private void SurveillancePoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            Logger.Logg(Name, Logger.LCU_Cat, "Time to do surveillance!");

            // First we need to find out the current overall (compound) status of the home.
            var compoundStatus = LcuRemoteCentralUnitsController.GetCompoundStatus();
            Logger.Logg(LcuConfigHandler.GetLCUName(), Logger.LCU_Cat, "CompoundStatus is " + compoundStatus);
            var localLcuStatus = GetLocalStatus();
            compoundStatus.AddLocalStatus(localLcuStatus);
            // The compound status is now ready.

            LatestCompoundStatus = compoundStatus; 


            // Check for different alarm situations, local or remote.
            LcuAlarmHandler.CheckSituation();

            // The color and pattern of the local LED might be affected of remote statuses.
// Setting LED causes problems during debug "wrong thread".
// Temporarily commented. maklchange SetLedCorrectly not invoked now.
//LcuLedHandler.SetLedCorrectly();

        }

        //private CompoundStatus CalculateCurrentCompoundStatus()
        //{
        //    //compoundStatus.GetNewestAlarmStatus()
        //    return null;
        //}

        public string Name { get; set; }

        private bool autoStatusUpdate = false;

        public bool AutoStatusUpdate
        {
            get => autoStatusUpdate;
            set
            {
                
            }

        }

        public IDoor Door { get; set; }

        public ILEDController LcuLedController { get; set; }

        public IDoorController LcuDoorController { get; set; }

        // Delay before the alarm turns the siren on when opening the door.
        public int EntranceDelay { get; set; }

       
        public string ReadEnvironmentVariable()
        {
            // Check whether the environment variable exists.
            var value = Environment.GetEnvironmentVariable("LCUName");
            // If necessary, create it.
            if (value == null)
            {
                try
                {
                    Environment.SetEnvironmentVariable("LCUName", "TheBackDoor");
                }
                catch (Exception ex)
                {
                    value = "ERR Could not create:" + ex.Message;
                }
                // Now retrieve it.
                value += Environment.GetEnvironmentVariable("LCUName");
            }
            //todo doLoggInGui("Environment variable LCUName is " + value);
            return value;
        }

        public List<string> GetLoggings()
        {
            return loggings;
        }



        //void CheckStatus()
        //{
        //    // Code from https://blogs.msdn.microsoft.com/benwilli/2016/06/30/asynchronous-infinite-loops-instead-of-timers/
        //    Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            // do the work in the loop
        //            string newData = DateTime.Now.ToString(Definition.StandardDateTimeFormat);

        //            // update the UI on the UI thread
        //            Dispatcher.Invoke(() => txtTicks.Text = "TASK - " + newData);

        //            // don't run again for at least 200 milliseconds
        //            await Task.Delay(200);
        //        }
        //    });
        //}
        public void GetColorForLED()
        {

        }

        //private Task IntruderSurevillance = new Task();

        // Turns the alarm on so that it will start monitoring doors etc.
        public void ActivateAlarm(int delayInMs)
        {
Logger.Logg(this.Name, Logger.LCU_Cat, "Entering Lcu.ActivateAlarm.");
            LcuAlarmHandler.ActivateAlarm(delayInMs);
            //IsAlarmActive = true;
            //Action checkTheDoor = CheckDoor;
            //Task.Run(checkTheDoor);
Logger.Logg(this.Name, Logger.LCU_Cat, "Leaving Lcu.ActivateAlarm.");

        }

        //private void CheckDoor()
        //{
        //}

        //public bool IsAlarmActive { get; private set; }

        public void DeactivateAlarm()
        {
            LcuAlarmHandler.DeactivateAlarm();
        }

        //// http://embedded101.com/BruceEitman/entryid/676/windows-10-iot-core-getting-the-mac-address-from-raspberry-pi
        //private async Task<string> GetMAC()
        //{
        //    String MAC = "MacUnknown";
        //    StreamReader SR = await GetJsonStreamData("http://localhost:8080/api/networking/ipconfig");
        //    JsonObject ResultData = null;
        //    try
        //    {
        //        String JSONData;

        //        JSONData = SR.ReadToEnd();

        //        ResultData = (JsonObject)JsonObject.Parse(JSONData);
        //        JsonArray Adapters = ResultData.GetNamedArray("Adapters");

        //        //foreach (JsonObject Adapter in Adapters) 
        //        for (uint index = 0; index < Adapters.Count; index++)
        //        {
        //            JsonObject Adapter = Adapters.GetObjectAt(index).GetObject();
        //            String Type = Adapter.GetNamedString("Type");
        //            if (Type.ToLower().CompareTo("ethernet") == 0)
        //            {
        //                MAC = ((JsonObject)Adapter).GetNamedString("HardwareAddress");
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }

        //    return MAC;
        //}

        //// http://embedded101.com/BruceEitman/entryid/676/windows-10-iot-core-getting-the-mac-address-from-raspberry-pi
        //private async Task<StreamReader> GetJsonStreamData(string URL)
        //{
        //    HttpWebRequest wrGETURL = null;
        //    Stream objStream = null;
        //    StreamReader objReader = null;

        //    try
        //    {
        //        wrGETURL = (HttpWebRequest)WebRequest.Create(URL);
        //        wrGETURL.Credentials = new NetworkCredential("Administrator", "p@ssw0rd");
        //        HttpWebResponse Response = (HttpWebResponse)(await wrGETURL.GetResponseAsync());
        //        if (Response.StatusCode == HttpStatusCode.OK)
        //        {
        //            objStream = Response.GetResponseStream();
        //            objReader = new StreamReader(objStream);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine("GetData " + e.Message);
        //    }
        //    return objReader;
        //}
        // Client

        private void SetName()
        {
            var configName = GetNameFromConfig();
            Name = configName;

            // Use the MAC-address as the name.
            //string MacAddress = await GetMAC();
            //Name = MacAddress +";" + configName;
        }

        private string GetNameFromConfig()
        {
            if (LcuConfigHandler != null)
            {
                return LcuConfigHandler.GetLCUName();
            }
            var name = "ErrorReadingLCUName";
            try
            {
                var resources = new Windows.ApplicationModel.Resources.ResourceLoader("HCResources");
                name = resources.GetString("LCUName");
            }
            catch (Exception ex)
            {
                name += ": " + ex.Message;
            }
            return name;
        }

        private List<string> loggings = new List<string>();
        private bool useAnyMockedDoorProperty;
        private IRgbLed DoorLed { get; set; }
        public int Id { get; set; }

        // Adds an item to the list of loggings.
        public void AddLogging(string text)
        {
            Logger.Logg(Name, text);
            loggings.Add(text);
            LcuHandler.GetInstance().SendEventThatModelHasChanged();
        }

 
        // Stops timers, turns off LED and siren.
        public void Reset()
        {
            LcuDoorController?.Reset();
            LcuLedController?.Reset();
            LcuSirenController?.Reset();
        }

        //public void SetupRemoteLCUCommunication()
        //{
        //    StartListeningOnRemoteLcu();
        //    SendCommandToRemoteLcu(TODO);
        //}
        public LocalLcuStatus GetLocalStatus()
        {
            return new LocalLcuStatus()
            {
                IsDoorLocked = LcuDoorController.IsDoorLocked(),
                IsDoorOpen = LcuDoorController.IsDoorOpen(),
                AlarmActivity = LcuAlarmHandler.CurrentLocalStatus
            };
        }

        public void OnLcuRelatedMessage(Definition.MessageType logg, string message)
        {
            lcuHandler.OnLcuRelatedMessage(this, logg, message);
        }
    }
}

