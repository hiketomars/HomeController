using System;
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
using Moq;

namespace HomeController.comm {

    /// <summary>
    /// Local Central Unit is in control of the local equipment.
    /// Typically this is a door with a LED, a siren. Also it has communication chanells to other LCU:s, RemoteCentralUnits.
    /// </summary>
    public class LocalCentralUnit
    {
        //private DispatcherTimer timer;
        private ThreadPoolTimer SurveillancePoolTimer;
        public AlarmHandler LcuAlarmHandler;
        public LedHandler LcuLedHandler;

        public LocalCentralUnit() 
        {
            // The HouseModelFactory is used to retrieve correct object depending on if this object is created for a normal execution or for a unit test.
            // Door
            var door = HouseModelFactory.GetDoor();
            var doorController = new DoorController();

            // LED
            var doorLed = HouseModelFactory.GetRgbLed();
            var ledController = new LEDController(doorLed);

            // Remote Central Unit Controller
            var remoteCentralUnitsController = new RemoteCentralUnitsController();
            // Siren
            var lcuSiren = HouseModelFactory.GetSiren();
            var lcuSirenController = new SirenController(LcuSiren);

            SetupLcu(door, doorController, doorLed, ledController, remoteCentralUnitsController, lcuSiren, lcuSirenController);

        }

        // Constructor for unit tests.

        public LocalCentralUnit(IRgbLed doorLed, ILEDController ledController, IDoor door, IDoorController doorController, IRemoteCentralUnitsController remoteCentralUnitsController, ISiren siren, ISirenController sirenController)
        {
            SetupLcu(door, doorController, doorLed, ledController, remoteCentralUnitsController, siren, sirenController);
        }

        private void SetupLcu(IDoor door, IDoorController doorController, IRgbLed doorLed, ILEDController ledController, IRemoteCentralUnitsController remoteCentralUnitsController, ISiren siren, ISirenController sirenController)
        {
            // Get and set the name for this LCU.
            SetName();

            // LED
            DoorLed = doorLed;
            LcuLedController = ledController;

            // Door
            Door = door;
            DoorController = doorController;

            // Siren
            LcuSiren = siren;
            LcuSirenController = sirenController;

            LcuAlarmHandler = new AlarmHandler(this);
            LcuLedHandler = new LedHandler(this);

            // Remote central units
            LcuRemoteCentralUnitsController = remoteCentralUnitsController;

            // Perform start sequence.
            //LcuLedController.PerformStartUpLedFlash();

            // Start never ending timer loop
            SurveillancePoolTimer = ThreadPoolTimer.CreatePeriodicTimer(SurveillancePoolTimerElapsedHandler, TimeSpan.FromMilliseconds(1000));

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(1000);
            //timer.Tick += Timer_Tick;


        }

        public ISirenController LcuSirenController { get; set; }


        // This is the central surveillance loop.
        // It is typcally called once a second or more and checks for local and remote statuses and might start actions as a reaction of that.
        private void SurveillancePoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            // Check for different alarm situations, local or remote.
            LcuAlarmHandler.CheckSituation();

            // The color and pattern of the local LED might be affected of remote statuses.
            LcuLedHandler.SetLedCorrectly();

        }

        public string Name { get; set; }

        private ISiren LcuSiren { get; set; }

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

        public IDoorController DoorController { get; set; }

        // Delay before the alarm turns the siren on when opening the door.
        public int EntranceDelay { get; set; }

        //public bool AlarmActive { get; }

        /// <summary>
        /// Sets a delegate function to use for logging information into the GUI for the LCU
        /// </summary>
        /// <param name="doLoggInGui"></param>
        public void SetView()
        {
            //todo this.doLoggInGui = doLoggInGui;
            //todo doLoggInGui("Name of LCU is " + Name);


            // Old stuff

            //System.IO.Path.GetDirectoryName(Environment.ExecutablePath);
            //string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //Environment.GetEnvironmentVariable
            //StorageFolder folder = ApplicationData.Current.LocalFolder;
            //doLoggInGui(Windows.Storage. Environment. "");
        }

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

        public async void StartLCU_Client_Communication()
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {
                    // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                    var hostName = new Windows.Networking.HostName("localhost");

                    AddLogging("The client is trying to connect...");

                    await streamSocket.ConnectAsync(hostName, "1337"); // Portnummer hårdkodat nu.

                    AddLogging("The client connected");

                    // Send a request to the echo server.
                    var request = "Hello, World!";
                    using (var outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

                    // Read data from the echo server.
                    string response;
                    using (var inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (var streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

                    AddLogging(string.Format("client received the response: \"{0}\" ", response));
                }
                AddLogging("The client closed its socket");
            }
            catch (Exception ex)
            {
                var webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                AddLogging(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public async void StartLCU_Server_Communication()
        {
            try
            {
                Logger.Logg("StartLCU_Server_Communication");
                var streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currentlycommunicatio in use.
                await streamSocketListener.BindServiceNameAsync(Definition.PortNumber);
                
                AddLogging("The server is listening...");
            }
            catch (Exception ex)
            {
                var webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                AddLogging(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
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
            LcuAlarmHandler.ActivateAlarm(delayInMs);
            //IsAlarmActive = true;
            //Action checkTheDoor = CheckDoor;
            //Task.Run(checkTheDoor);
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
            var configName = GetNameFromConfigFile();
            Name = configName;

            // Use the MAC-address as the name.
            //string MacAddress = await GetMAC();
            //Name = MacAddress +";" + configName;
        }

        private string GetNameFromConfigFile()
        {
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
        private IRgbLed DoorLed { get; set; }
        public IRemoteCentralUnitsController LcuRemoteCentralUnitsController { get; set; }

        // Adds an item to the list of loggings.
        private void AddLogging(string text)
        {
            Logger.Logg(text);
            loggings.Add(text);
            HouseHandler.GetInstance().SendEventThatModelHasChanged();
        }

        private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));
            AddLogging(string.Format("The server received the request: \"{0}\"", request));

            // Echo the request back as the response.
            using (var outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));
            AddLogging(string.Format("The server sent back the response: \"{0}\"", request));
            sender.Dispose();

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add("server closed its socket"));
            AddLogging("The server closed its socket");
        }

        // Stops timers, turns off LED and siren.
        public void Reset()
        {
            DoorController.Reset();
            LcuLedController.Reset();
            LcuSirenController.Reset();
        }
    }
}

