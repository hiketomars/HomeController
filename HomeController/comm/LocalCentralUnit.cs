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


namespace HomeController.comm {

    /// <summary>
    /// Local Central Unit is in control of the local equipment.
    /// Typically this is a door with a LED, a siren. Also it has communication chanells to other LCU:s, RemoteCentralUnits.
    /// </summary>
    public class LocalCentralUnit
    {
        public string Name { get; set; }

        public LocalCentralUnit()
        {
            // Get and set the name for this LCU.
            SetName();
        }

        private /*async*/ void SetName()
        {
            string configName = GetNameFromConfigFile();
            Name = configName;

            // Old stuff

            // Use the MAC-address as the name.
            //string MacAddress = await GetMAC();
            //Name = MacAddress +";" + configName;
        }

        private string GetNameFromConfigFile()
        {
            string name = "ErrorReadingLCUName";
            try
            {
                var resources = new Windows.ApplicationModel.Resources.ResourceLoader("HCResources");
                name = resources.GetString("LCUName");
            }catch(Exception ex)
            {
                name += ": " + ex.Message;
            }
            return name;
        }


        //#region Get MAC-address
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

        //#endregion

        private bool autoStatusUpdate = false;

        public bool AutoStatusUpdate
        {
            get
            {
                return autoStatusUpdate;
            }
            set
            {
                
            }

        }

        

        private Definition.LoggInGui doLoggInGui;

        /// <summary>
        /// Sets a delegate function to use for logging information into the GUI for the LCU
        /// </summary>
        /// <param name="doLoggInGui"></param>
        public void SetView(Definition.LoggInGui doLoggInGui)
        {
            this.doLoggInGui = doLoggInGui;
            doLoggInGui("Name of LCU is " + Name);


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
            string value = Environment.GetEnvironmentVariable("LCUName");
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
            doLoggInGui("Environment variable LCUName is " + value);
            return value;
        }


        public async void StartLCUCommunication()
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {
                    // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                    var hostName = new Windows.Networking.HostName("localhost");

                    //this.clientListBox.Items.Add("client is trying to connect...");
                    doLoggInGui("Client is trying to connect...");

                    await streamSocket.ConnectAsync(hostName, "1337"); // Portnummer hårdkodat nu.

                    //this.clientListBox.Items.Add("client connected");
                    doLoggInGui("Client connected");

                    // Send a request to the echo server.
                    string request = "Hello, World!";
                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

                    //this.clientListBox.Items.Add(string.Format("client sent the request: \"{0}\"", request));
                    doLoggInGui(string.Format("client sent the request: \"{0}\"", request));

                    // Read data from the echo server.
                    string response;
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

                    //this.clientListBox.Items.Add(string.Format("client received the response: \"{0}\" ", response));
                    doLoggInGui(string.Format("client received the response: \"{0}\" ", response));
                }

                //this.clientListBox.Items.Add("client closed its socket");
                doLoggInGui("client closed its socket");

            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
                doLoggInGui(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
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
    }
}

