using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.System.Threading;
using HomeController.comm;
using HomeController.utils;

namespace HomeController.model
{
    // This class is a proxy for a remote central unit.
    // It regularly checks status for the real remote lcu using various communication methods.
    public class RemoteCentralUnitProxy : IRemoteCentralUnitProxy
    {
        //private readonly LocalCentralUnit lcu;
        private readonly string name;
        private readonly string ipAddress;
        private readonly string portNumber;
        private ThreadPoolTimer periodicTimer;

        private const string MessageLcuStarting = "msgLcuStarting;";
        private const string MessageIsDoorUnlocked = "msgIsDoorUnlocked;";
        private const string MessageHasIntrusionOccurred = "msgHasIntrusionOccurred;";
        private const string MessageGetStatus = "msgGetStatus;";
        public const string MessageACK = "ACK: ";


        private RemoteLcuStatus RemoteLcuStatus { get; set; }
        private object remoteLcuStatusHasChanged;
        public event Definition.RemoteLcuStatusChangedEventHandler RemoteLcuStatusHasChanged;

        public RemoteCentralUnitProxy(/*LocalCentralUnit lcu,*/ string name, string ipAddress, string portNumber)
        {
            //this.lcu = lcu;
            this.name = name;
            this.ipAddress = ipAddress;
            this.portNumber = portNumber;
            RemoteLcuStatus = new RemoteLcuStatus(); // Initialize.
            CreateAndStartPeriodicTimer();
        }

        private void CreateAndStartPeriodicTimer()
        {
            int period = 1000;
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(timerElapsedHandler, TimeSpan.FromMilliseconds(period));
        }

        private void timerElapsedHandler(ThreadPoolTimer timer)
        {
            var response = SendCommand(ipAddress, MessageGetStatus);
            RemoteLcuStatus = GetRemoteLcuStatus(response);
        }

        private RemoteLcuStatus GetRemoteLcuStatus(Task<string> response)
        {
            return new RemoteLcuStatus(); //todo här ska jag kommunicera med remote lcu.
        }

        public string Name { get; set; }
        public string PortNumber { get; set; }
        public string IpAddress { get; set; }

        public async Task<string> SendCommand(/*LocalCentralUnit lcu,*/ string hostIP, string command)
        {
            // Create the StreamSocket and establish a connection to the echo server.
            using(var streamSocket = new Windows.Networking.Sockets.StreamSocket())
            {
                //// The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                ////var hostName = new Windows.Networking.HostName("localhost");
                var hostName = new Windows.Networking.HostName(hostIP);

                //lcu.AddLogging("The client is trying to connect to remote lcu at IP " + Definition.RemoteLcuPIAddress + "...");

                await streamSocket.ConnectAsync(hostName, Definition.RemoteLcuPortNumber); // Portnummer hårdkodat nu.

                //lcu.AddLogging("The client connected");

                // Send a request to the echo server.
                //var request = "Hello, World!";


                //As an alternative in the UWP Sample (Scenario3_Send.xaml.cs:
                //   writer = new DataWriter(socket.OutputStream);
                //   ...
                //   writer.WriteString(stringToSend);
                //   await writer.StoreAsync();



                using(var outputStream = streamSocket.OutputStream.AsStreamForWrite())
                {
                    using(var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(command);
                        await streamWriter.FlushAsync();
                    }
                }

                // Read data from the echo server.
                string response;
                using(var inputStream = streamSocket.InputStream.AsStreamForRead())
                {
                    using(var streamReader = new StreamReader(inputStream))
                    {
                        response = await streamReader.ReadLineAsync();
                    }
                }
                //lcu.AddLogging(string.Format("client received the response: \"{0}\" ", response));
                //lcu.AddLogging("The client closed its socket");
                return response;
            }
        }

        public bool HasIntrusionOccurred()
        {
            return RemoteLcuStatus.HasIntrusionOccurred;
        }

        public bool HasIntrusionOccurredRemotely()
        {
            return RemoteLcuStatus.HasIntrusionOccurredRemotely;
        }

        public bool IsDoorUnlocked()
        {
            return RemoteLcuStatus.IsDoorUnlocked;
        }

        public void SendStartUpMessage()
        {
            throw new NotImplementedException();
        }

        object IRemoteCentralUnitProxy.RemoteLcuStatusHasChanged
        {
            get => remoteLcuStatusHasChanged;
            set => remoteLcuStatusHasChanged = value;
        }

        public void ActivateCommunication()
        {
            StartListeningOnRemoteLcu();
        }

        //        // This "client" method sets up the socket each time. Maybe we could save it and only create socket first time or when needed.
        //        public async Task<string> SendCommandToRemoteLcu(string command)
        //        {
        //            try
        //            {
        //                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
        //                //var hostName = new Windows.Networking.HostName("localhost");
        //                //var hostName = new Windows.Networking.HostName(Definition.RemoteLcuPIAddress);

        //                await rcu.SendCommand(lcu, Definition.RemoteLcuPIAddress, command);

        //                #region movedCode
        //                /*
        //                // Create the StreamSocket and establish a connection to the echo server.
        //                using(var streamSocket = new Windows.Networking.Sockets.StreamSocket())
        //                {
        //                    //// The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
        //                    ////var hostName = new Windows.Networking.HostName("localhost");
        //                    //var hostName = new Windows.Networking.HostName(Definition.RemoteLcuPIAddress);

        //                    lcu.AddLogging("The client is trying to connect to remote lcu at IP " + Definition.RemoteLcuPIAddress + "...");

        //                    await streamSocket.ConnectAsync(hostName, Definition.RemoteLcuPortNumber); // Portnummer hårdkodat nu.

        //                    lcu.AddLogging("The client connected");

        //                    // Send a request to the echo server.
        //                    //var request = "Hello, World!";


        //                     //As an alternative in the UWP Sample (Scenario3_Send.xaml.cs:
        //                     //   writer = new DataWriter(socket.OutputStream);
        //                     //   ...
        //                     //   writer.WriteString(stringToSend);
        //                     //   await writer.StoreAsync();



        //                    using(var outputStream = streamSocket.OutputStream.AsStreamForWrite())
        //                    {
        //                        using(var streamWriter = new StreamWriter(outputStream))
        //                        {
        //                            await streamWriter.WriteLineAsync(command);
        //                            await streamWriter.FlushAsync();
        //                        }
        //                    }

        //                    // Read data from the echo server.
        //                    string response;
        //                    using(var inputStream = streamSocket.InputStream.AsStreamForRead())
        //                    {
        //                        using(var streamReader = new StreamReader(inputStream))
        //                        {
        //                            response = await streamReader.ReadLineAsync();
        //                        }
        //                    }
        //                    return response;
        //                    lcu.AddLogging(string.Format("client received the response: \"{0}\" ", response));

        //                }
        //                */
        //#endregion


        //            }
        //            catch(Exception ex)
        //            {
        //                var webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
        //                lcu.AddLogging(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //            }

        //            return "exception";
        //        }

        // This "server" method starts listening on port for calls from remote lcu.
        private async void StartListeningOnRemoteLcu()
        {
            try
            {
                Logger.Logg("StartLCU_Server_Communication");
                var streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currentlycommunicatio in use.
                await streamSocketListener.BindServiceNameAsync(Definition.OwnPortNumber);
                /*
                     Alternative from UWP Samples Scenario1.
                    // Try to bind to a specific address.
                    await listener.BindEndpointAsync(selectedLocalHost.LocalHost, ServiceNameForListener.Text);
                    rootPage.NotifyUser(

                   Yet another alternative:
                    // Try to limit traffic to the selected adapter.
                    // This option will be overridden by interfaces with weak-host or forwarding modes enabled.
                    NetworkAdapter selectedAdapter = selectedLocalHost.LocalHost.IPInformation.NetworkAdapter; 

                    // For demo purposes, ensure that we use the same adapter in the client connect scenario.
                    CoreApplication.Properties.Add("adapter", selectedAdapter);

                    await listener.BindServiceNameAsync(
                        ServiceNameForListener.Text, 
                        SocketProtectionLevel.PlainSocket,
                        selectedAdapter);
                 */

                //lcu.AddLogging("The server is listening on port " + Definition.OwnPortNumber + "...");
            }
            catch(Exception ex)
            {
                var webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                //lcu.AddLogging(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using(var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));
            //lcu.AddLogging(string.Format("The server received the request: \"{0}\"", request));

            string response = "";
            if(request.StartsWith(MessageLcuStarting))
            {
                // Echo the request back as the response with ACK: first.
                response = MessageACK + request;
            }
            else if(request.StartsWith(MessageHasIntrusionOccurred))
            {
                throw new Exception(); // response = AlarmHandler.GetInstance().HasIntrusionOccurred.ToString();
            }
            else if(request.StartsWith(MessageIsDoorUnlocked))
            {
                response = (!LocalCentralUnit.GetInstance().LcuDoorController.IsDoorLocked()).ToString();
            }
            else
            {
                response = "error: unknown command";
            }


            using(var outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using(var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(response);
                    await streamWriter.FlushAsync();
                }
            }
            // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
            // the text back to the UI thread as shown.


            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));
            //lcu.AddLogging(string.Format("The server sent back the response: \"{0}\"", response));
            sender.Dispose();

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add("server closed its socket"));
            //lcu.AddLogging("The server closed its socket");


        }


    }
}
