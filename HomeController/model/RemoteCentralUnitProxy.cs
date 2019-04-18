using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Devices;
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
        private readonly string nameOfRemoteLcu;
        private readonly string ipAddress;
        private readonly string portNumber;
        private ThreadPoolTimer periodicTimer;

        private const string MessageLcuStarting = "msgLcuStarting";
        private const string MessageIsDoorUnlocked = "msgIsDoorUnlocked";
        private const string MessageHasIntrusionOccurred = "msgHasIntrusionOccurred";
        private const string MessageGetStatus = "msgGetStatus";
        private const string MessagePing = "msgPing";
        private const string MessageCurrentStatus = "msgCurrentStatus";
        private const string MessageStateChanged = "msgStateChanged";
        private const string MessageStartUp = "msgStartUp";
        public const string MessageACK = "ACK";


        private RemoteLcuStatus RemoteLcuStatus { get; set; }
        private object remoteLcuStatusHasChanged;
        public event Definition.RemoteLcuStatusChangedEventHandler RemoteLcuStatusHasChanged;

        public RemoteCentralUnitProxy(/*LocalCentralUnit lcu,*/ string nameOfRemoteLcu, string ipAddress, string portNumber)
        {
            //this.lcu = lcu;
            this.nameOfRemoteLcu = nameOfRemoteLcu; // This is our name of the remote LCU which might not be exactly the same as it calls itself.
            this.ipAddress = ipAddress;
            this.portNumber = portNumber;
            RemoteLcuStatus = new RemoteLcuStatus(); // Initialize.
            CreateAndStartPeriodicTimer();
            Logger.Logg(Logger.RCUProxy, "Created RCU Proxy for " + nameOfRemoteLcu);
        }

        private void CreateAndStartPeriodicTimer()
        {
            int period = 1000;
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(timerElapsedHandler, TimeSpan.FromMilliseconds(period));
        }

        private void timerElapsedHandler(ThreadPoolTimer timer)
        {
            // Ask for status of remote lcu. The response will be handled elsewhere. 
            // Maybe I need to check if I get any answer....?
            Logger.Logg(Logger.RCUProxy, "Sending GetStatus to " + nameOfRemoteLcu);

            var response = SendCommand(ipAddress, MessageGetStatus);
            //RemoteLcuStatus = GetRemoteLcuStatus(response);
        }

        //private RemoteLcuStatus GetRemoteLcuStatus(Task<string> response)
        //{
        //    return new RemoteLcuStatus(); //todo här ska jag kommunicera med remote lcu.
        //}

        //private RemoteLcuPingResponse PingRemoteLcu(Task<string> response)
        //{
        //    return new RemoteLcuPingResponse(); //todo här ska jag kommunicera med remote lcu.
        //}

        public string Name { get; set; }
        public string PortNumber { get; set; }

        public bool SendPingMessage()
        {
            var response = SendCommand(ipAddress, MessagePing);
            var interpretedResult = InterpretResponse(response);
            return interpretedResult.Response;
        }

        public void SendStateChangedMessage(AlarmHandler.AlarmActivityStatus currentStatus)
        {
            var remoteMessage = new RemoteMessage() { Id = GetNewMessageId(), Message = MessageStartUp + RemoteMessage.MessagPartsDelimeter + currentStatus + RemoteMessage.MessagPartsDelimeter };

            var response = SendCommand(ipAddress, remoteMessage.TotalMessage);
            var interpretedResult = InterpretResponse(response);
            // todo Jag måste väl kolla att jag får ett acc här?
        }

        public void SendStartUpMessage()
        {
            var response = SendCommand(ipAddress, MessageStartUp);
            var interpretedResult = InterpretResponse(response);
            //interpretedResult.Response;
        }


        private string GetNewMessageId()
        {
            return DateTime.Now.Ticks.ToString();
        }

        //private RemoteLcuPingResponse InterpretResponse(Task<string> response)

        private RemoteLcuResponse InterpretResponse(Task<string> response)
        {
            //todo investigate response.Result and create a  RemoteLcuPingResponse from that data. Hardcoded test value below.
            return new RemoteLcuResponse(){ResponderName = "Some ecu", Response = true, ResponseTime = new TimeSpan(0,0, 1)};
        }

        public string IpAddress { get; set; }

        public async Task<string> SendCommand(string hostIP, string command)
        {
            // Create the StreamSocket and establish a connection to the echo server.
            using(var streamSocket = new Windows.Networking.Sockets.StreamSocket())
            {
                //// The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                ////var hostName = new Windows.Networking.HostName("localhost");
                var hostName = new Windows.Networking.HostName(hostIP);

                //lcu.AddLogging("The client is trying to connect to remote lcu at IP " + Definition.RemoteLcuPIAddress + "...");

                Logger.Logg(Logger.RCUProxy, "SendCommand: Connecting "+hostName + " " + Definition.RemoteLcuPortNumber);
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
                        Logger.Logg(Logger.RCUProxy, "SendCommand: Writing to " + hostName + " " + Definition.RemoteLcuPortNumber);

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
                        Logger.Logg(Logger.RCUProxy, "SendCommand: Waiting for response...");

                        response = await streamReader.ReadLineAsync();
                    }
                }
                //lcu.AddLogging(string.Format("client received the response: \"{0}\" ", response));
                //lcu.AddLogging("The client closed its socket");
                Logger.Logg(Logger.RCUProxy, "SendCommand: Received response: " +response);

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
            return RemoteLcuStatus.IsDoorLocked;
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
            Logger.Logg(Logger.RCUProxy, "Received something!");

            string request;
            using(var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));
            //lcu.AddLogging(string.Format("The server received the request: \"{0}\"", request));


            var message = GetRemoteMessage(request);
            Logger.Logg(Logger.RCUProxy,"Got message " + message.Message+ "with id "+message.Id + " from " + nameOfRemoteLcu);

            var communicationResponse = CreateCommunicationResponseObject(message);
            if (communicationResponse == null)
            {    
                Logger.Logg("Will not send ACK to this command");
                return;
            }
        
            string ackResponse = communicationResponse.GetAckResponse();

            using (var outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(ackResponse);
                    await streamWriter.FlushAsync();
                }
            }

            Logger.Logg("Sent ack to message with id " + message.Id);

            // Ack has been sent. Now time to interpret the message.
            if(message.Message == MessagePing)
            {
                SendPingResponse(message);
                
            }else if (message.Message == MessageGetStatus)
            {
                // A remote lcu has requested to get our status
                SendCurrentStatus();
            }
            else if (message.Message == MessageCurrentStatus)
            {
                // We've got the status of this RCU, store it here for future use.
                RemoteLcuStatus = (RemoteLcuStatus)message;
            }
            //else if (message.Message == MessageStateChanged)
            //{
            //    HandleStateChangedOnRemoteLcu(nameOfRemoteLcu, message);
            //}





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

        private void SendCurrentStatus()
        {

            Logger.Logg(Logger.RCUProxy, "Sending message CurrentStatus as a response on earlier received request.");

            var currentStatusCommand = RemoteMessage.CreateCurrentStatusMessage(null);//todo source
            SendCommand(ipAddress, currentStatusCommand); //todo ip

        }

        // This LCU got a StateChanged message from a remote lcu. This method takes care of what to need to be done locally and possibly sends a response.
        private void HandleStateChangedOnRemoteLcu(string remoteLcuName, ITransferObject message)
        {
            // StateChanged does not need a response, the ack that already has been sent is enough.
            
        }

        private void SendPingResponse(ITransferObject message)
        {
            // Ping does not need a response, the ack that already has been sent is enough.
        }

        public ITransferObject GetRemoteMessage(string request)
        {
            var parts = request.Split(RemoteMessage.MessagPartsDelimeter[0]);
            var messageStartToken = parts[0].Trim().ToUpper();
            var id = parts[1].Trim();
            var message = parts[2].Trim();
            int i = 3;
            var parameters = new List<string>(); 
            while (parts.Length > i)
            {
                parameters.Add(parts[i++]);
            }
            switch(messageStartToken)
            {
                case MessagePing:
                    return new RemoteMessage() { Message = MessagePing, Id = id};

                case MessageCurrentStatus:
                    // Deserializing an object would be nicer here.
                    return RemoteMessage.InterpretCurrentStatusMessage(message, id, parameters);
                    //return new RemoteLcuStatus() {
                    //    Message = message,
                    //    Id = id,
                    //    IsDoorLocked = bool.Parse(parameters[0]),
                    //    HasIntrusionOccurred = bool.Parse(parameters[1]),
                    //    HasIntrusionOccurredRemotely = bool.Parse(parameters[2]),
                    //};

                /* För närvarande känner jag int att jag behöver ett sådant här meddelande.
                 Local lcu kommer fråga efter status regelbundet så jag behöver inga meddelanden om att det skett någon förändring, eller...?

                case MessageStateChanged:
                    return new StateChangedRemoteMessage()
                    {
                        Message = MessageStateChanged, Id = id,
                        NewState = (AlarmHandler.AlarmActivityStatus) int.Parse(parameters[0])
                    };
                    */


                default:
                    int s = 0;
                    break;
            }

            Logger.Logg("Received unknown message " + messageStartToken);
            return null;
        }

        // An alternative to this approach would be to serialize an entire object.
        // Commands:
        // ACK; 2324; Ping;  
        //
        // Response is an object of type CommunicationResponse. 
        //
        private CommunicationResponse CreateCommunicationResponseObject(ITransferObject receivedMessage)
        {
            return new CommunicationResponse(){AckedMessaged = receivedMessage.Message, Id = receivedMessage.Id};
        }
    }
}
