﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.System.Threading;
using Castle.Components.DictionaryAdapter;
using HomeController.comm;
using HomeController.utils;

namespace HomeController.model
{
    
    /// <summary>
    /// This class is a proxy for a remote central unit.
    /// It regularly checks status for the real remote lcu using various communication methods.
    /// </summary>
    public class RemoteCentralUnitProxy : IRemoteCentralUnitProxy
    {
        
        


        private readonly ILocalCentralUnit lcu;
        public string NameOfRemoteLcu { get; set; }
        private readonly string ipAddress;
        private string portNumber;
        private ThreadPoolTimer periodicTimer;
        public string IpAddress { get; set; }
        //public string Name { get; set; }

        //private const string MessageLcuStarting = "msgLcuStarting";
        //private const string MessageIsDoorUnlocked = "msgIsDoorUnlocked";
        //private const string MessageHasIntrusionOccurred = "msgHasIntrusionOccurred";
        //private const string MessageStateChanged = "msgStateChanged";
        private const string MessageStartUp = "msgStartUp";
        public const string MessageACK = "ACK";


        public CurrentStatusMessage RcuCurrentStatusMessage { get; set; }
        private object remoteLcuStatusHasChanged;
        public event Definition.RemoteLcuStatusChangedEventHandler RemoteLcuStatusHasChanged;
        //public event Definition.RcuInfoEventHandler NewRcuInfo;
        

        public RemoteCentralUnitProxy(ILocalCentralUnit lcu, string nameOfRemoteLcu, int idOfRemoteLcu, string ipAddress, string portNumber)
        {
            this.lcu = lcu;
            this.NameOfRemoteLcu = nameOfRemoteLcu; // This is our name of the remote LCU which might not be exactly the same as it calls itself.
            this.ipAddress = ipAddress;
            this.portNumber = portNumber;
            //this.portNumber = portNumber;co            //this.respondPortNumber = respondPortNumber;
            RcuCurrentStatusMessage = new CurrentStatusMessage(null, null, null, null, AlarmHandler.AlarmActivityStatus.Undefined, "?"); // Initialize.
            //CreateAndStartPeriodicTimer();

            //if (lcu.Id < idOfRemoteLcu)
            //{
            //    this.portNumber = ActingLowIdPortNumber;
            //    this.respondPortNumber = ReactingPortNumber;
            //}
            //else if (lcu.Id > idOfRemoteLcu)
            //{
            //    this.portNumber = ReactingPortNumber;
            //    this.respondPortNumber = ActingLowIdPortNumber;
            //}
            //else
            //{
            //    throw new Exception("LCU:s have the same id " + lcu.Id);
            //}

            
                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Created RCU Proxy for " + nameOfRemoteLcu);
                //lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Created RCU Proxy for " + nameOfRemoteLcu); // todo Här använder jag ett event som är tänkt för något annat men kanske jag bara byta namn på eventet och ha det till lite mer generell kommunikation till presentern...

        }

        public RemoteCentralUnitProxy()
        {
        }

        /// <summary>
        /// Starts listener listen for commands from the RCU and also starts requesting status message from the RCU regularly.
        /// </summary>
        public void ActivateCommunication()
        {
            //StartListeningToRemoteLcu();
            StartPeriodicRequestForRcuStatusMessage();
        }

        /// <summary>
        /// Sends a message to the RCU indicating that this LCU has started.
        /// </summary>
        public void SendStartUpMessage()
        {
            lock (this)
            {
                var response = SendCommand(ipAddress, MessageStartUp);
                var interpretedResult = InterpretResponse(response);
                //interpretedResult.Response;
            }
        }

        /// <summary>
        /// Sends a ping to the RCU.
        /// </summary>
        /// <returns></returns>
        public bool SendPingMessage()
        {
            var response = SendCommand(ipAddress, (string)RemoteCentralUnitsController.MessagePing);
            var interpretedResult = InterpretResponse(response);
            return interpretedResult.Response;
        }

        /// <summary>
        /// Checks the intrusion status in the latest status message from the RCU.
        /// </summary>
        /// <returns></returns>
        public bool HasIntrusionOccurred()
        {
            return RcuCurrentStatusMessage.HasIntrusionOccurred;
        }

        /// <summary>
        /// Checks the 'remotely intrusion' status in the latest status message from the RCU.
        /// This means that this LCU can find out if the RCU has registered the status that this LCU has.
        /// </summary>
        /// <returns></returns>
        public bool HasIntrusionOccurredRemotely()
        {
            return RcuCurrentStatusMessage.HasIntrusionOccurredRemotely;
        }

        public bool? IsDoorUnlocked()
        {
            return RcuCurrentStatusMessage.IsDoorLocked;
        }

        object IRemoteCentralUnitProxy.RemoteLcuStatusHasChanged
        {
            get => remoteLcuStatusHasChanged;
            set => remoteLcuStatusHasChanged = value;
        }
        public CurrentStatusMessage GetRcuCurrentStatusMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // For debug purpose.
        public void ConnectToRcu()
        {
            lock (this)
            {

                ConnectToRCU("localhost"); //todo
            }
        }

        public void RequestStatusFromRcu()
        {
            lock (this)
            {

                //Task<string> t = SendCommandSpecific(ipAddress, "hejsan");
                //Task<string> t = SendCommand(ipAddress, MessageGetStatus);
                SendRequestOfRcuStatusMessage();
                //Task<string> t = SendCommand(ipAddress, MessageGetStatus);
            }
        }

        private List<StatusBaseMessage> incomingQueue = new List<StatusBaseMessage>();
        public void QueueIncomingMessage(StatusBaseMessage statusBaseMessage)
        {
            //incomingQueue.Add(statusBaseMessage);
            // Currently no que is used.
            HandleMessageFromQueue(statusBaseMessage);
        }


        private StatusBaseMessage lastReceivedRcuStatus;
        public async void HandleMessageFromQueue(StatusBaseMessage statusBaseMessage)
        {
                Task.Delay(1000).Wait();

                switch (statusBaseMessage)
                {
                    case CurrentStatusMessage rcuStatusMessage:
                        // Got the status for the RCU.
                        lastReceivedRcuStatus = statusBaseMessage;
                        lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg,
                            "RCU " + NameOfRemoteLcu + " has status " + rcuStatusMessage.AlarmStatus + ", DO:" + rcuStatusMessage.IsDoorOpen + ", DF:"+rcuStatusMessage.IsDoorFloating + ", DL:"+rcuStatusMessage.IsDoorLocked+", DS:"+rcuStatusMessage.IsSabotaged);
                        lcu.OnRcuReceivedMessage(this, Definition.MessageType.RcuStatus,
                            rcuStatusMessage.StatusAsReadableString);
                        break;

                    case RequestRcuStatusMessage message:
                    // Our status is requested, send it to the RCU.
                    lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg,
                        "RCU " + NameOfRemoteLcu + " requests our status.");

                    await SendCurrentStatusToRcu();
                        break;

                    case UnknownMessage message:
                        break;

                    default:
                        break;
                }
            
        }


        private static StreamSocketListener streamSocketListener; // Eventuellt spara undan med CoreApplication.Properties.Add("listener"+xxx, listener);

        // For the GUI that supports several LCU:s.
        // This "server" method starts listening on port for calls from remote lcu.
        // The lcuName is used to get the correct port to listen to.
        // In production the port could be the same for all lcu/rcu:s but since the multi-LCU-GUI
        // is run on a single PC the ports needs to be different for each rcu.
        //public async void StartListeningToRemoteLcu()
        //{
        //    try
        //    {
                
        //        string message = lcu.Name + " now listen on port " + portNumber+ " expecting messages from " + NameOfRemoteLcu;
        //        Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, message);
        //        streamSocketListener = new StreamSocketListener();
        //        //CoreApplication.Properties.Add("listener", listener);


        //        // The ConnectionReceived event is raised when connections are received.
        //        streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
        //        //streamSocketListener.Control.KeepAlive = false;

        //        // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
        //        await streamSocketListener.BindServiceNameAsync(portNumber);

        //        lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, message);


        //        #region alternatives

        //        /*
        //             Alternative from UWP Samples Scenario1.
        //            // Try to bind to a specific address.
        //            await listener.BindEndpointAsync(selectedLocalHost.LocalHost, ServiceNameForListener.Text);
        //            rootPage.NotifyUser(

        //           Yet another alternative:
        //            // Try to limit traffic to the selected adapter.
        //            // This option will be overridden by interfaces with weak-host or forwarding modes enabled.
        //            NetworkAdapter selectedAdapter = selectedLocalHost.LocalHost.IPInformation.NetworkAdapter; 

        //            // For demo purposes, ensure that we use the same adapter in the client connect scenario.
        //            CoreApplication.Properties.Add("adapter", selectedAdapter);

        //            await listener.BindServiceNameAsync(
        //                ServiceNameForListener.Text, 
        //                SocketProtectionLevel.PlainSocket,
        //                selectedAdapter);
        //         */

        //        #endregion

        //        //lcu.AddLogging("The server is listening on port " + Definition.OwnPortNumber + "...");
        //    }
        //    catch(Exception ex)
        //    {
        //        var webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
        //        Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Exception in RCUP.StartListeningOnRemoteLcu " + ex);

        //        //lcu.AddLogging(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }
        //}

        //private void OnNewRcuInfo(string info)
        //{
        //    if(NewRcuInfo != null)
        //    {
        //        NewRcuInfo(lcu.Name, NameOfRemoteLcu, info);
        //    }
        //}



        public async void ConnectToRCU(string hostIp)
        {


                // Create the StreamSocket and establish a connection to the RCU.
                _streamSockets[portNumber] = new StreamSocket();


                //// The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                ////var hostName = new Windows.Networking.HostName("localhost");
                var hostName = new HostName(hostIp);

                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "ConnectToRCU: Pos20");
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Pos20");

                //lcu.AddLogging("The client is trying to connect to remote lcu at IP " + Definition.RemoteLcuPIAddress + "...");

                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat,
                    "ConnectToRCU: Connecting async " + hostName + " " + portNumber);
                await _streamSockets[portNumber].ConnectAsync(hostName, portNumber);

                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "ConnectToRCU: Pos30");
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Pos30");


                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat,
                    "ConnectToRCU: Connected async to " + hostName + " " + portNumber);
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg,
                    "Connected async to " + hostName + " " + portNumber);
            
        }

        //private static StreamSocket streamSocket;
        private static Dictionary<string, StreamSocket> _streamSockets = new Dictionary<string, StreamSocket>();


        private static Stream outputStream;
        private static Dictionary<string, Stream> _outputStreams = new Dictionary<string, Stream>();

        private int sendCounter;


        // Sends exactly the specified command without adding anything.
        // Parameter port only needs to be specified when called from a debug method since client and server are on the same machine then.
        public async Task<string> SendCommandSpecific(string hostIp, string exactCommand)
        {

            try
            {
                sendCounter++;

                //if (streamSocket == null)
                //{
                //    ConnectToRCU(hostIp);
                //    lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Connected to RCU on the fly");
                //}

                //// Create the StreamSocket and establish a connection to the echo server.


                //    streamSocket = new StreamSocket();


                ////// The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                //////var hostName = new Windows.Networking.HostName("localhost");
                //var hostName = new HostName(hostIp);

                //Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "SendCommandSpecific: Pos20");
                //lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Pos20");

                ////lcu.AddLogging("The client is trying to connect to remote lcu at IP " + Definition.RemoteLcuPIAddress + "...");


                //Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "SendCommandSpecific: Pos30");
                //lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Pos30");




                //lcu.AddLogging("The client connected");

                // Send a request to the echo server.
                //var request = "Hello, World!";


                //As an alternative in the UWP Sample (Scenario3_Send.xaml.cs:
                //   writer = new DataWriter(socket.OutputStream);
                //   ...
                //   writer.WriteString(stringToSend);
                //   await writer.StoreAsync();

                var hostName = new HostName(hostIp);


                //if (streamSocket == null)
                //{
                //    streamSocket = new StreamSocket();
                //}
                if (!_streamSockets.ContainsKey(portNumber))
                {
                    _streamSockets.Add(portNumber, new StreamSocket());
                }



                //if(outputStream == null)
                //{
                //    outputStream = streamSocket.OutputStream.AsStreamForWrite();
                //    //CoreApplication.Properties.Add("clientStreamOutputStream", outputStream);
                //    //outValueOutputStream = outputStream;
                //}

                if (!_outputStreams.ContainsKey(portNumber))
                {
                    _outputStreams[portNumber] = _streamSockets[portNumber].OutputStream.AsStreamForWrite();
                    //CoreApplication.Properties.Add("clientStreamOutputStream", outputStream);
                    //outValueOutputStream = outputStream;
                }


                //using(var outputStream = streamSocket.OutputStream.AsStreamForWrite())
                //{
                var streamWriter = new StreamWriter(_outputStreams[portNumber]);

                //using(var streamWriter = new StreamWriter(outputStream))
                //{
                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat,
                    "SendCommandSpecific: Writing to port " + portNumber + " on " + hostName);

                await streamWriter.WriteLineAsync(exactCommand);
                await streamWriter.FlushAsync();
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.SendCounter, sendCounter.ToString());
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg,
                    lcu.Name + " writing to port " + portNumber + " on " + hostName + " (via its proxy for RCU" +
                    NameOfRemoteLcu + ") " + ":\r\n   " + exactCommand);
                //}
                //}

                // Read data from the echo server.
                //string response = "FAKE_ACC";
/*
            using(var inputStream = streamSocket.InputStream.AsStreamForRead())
            {
                using(var streamReader = new StreamReader(inputStream))
                {
                    Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "SendCommand: Waiting for Ack...");
                    lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Waiting for Ack...");
                    response = await streamReader.ReadLineAsync();
                }
            }
            //lcu.AddLogging(string.Format("client received the response: \"{0}\" ", response));
            //lcu.AddLogging("The client closed its socket");
            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "SendCommand: Received Ack: " + response);
            lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Received Ack: " + response);
*/
                return "xxxresponse";
            }
            catch (InvalidOperationException ex)
            {
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, "Probably not connected to "+NameOfRemoteLcu);
                return "Exception " + ex.Message;

            }
            catch (Exception ex)
            {
                return "Exception " + ex.Message;
            }

        }


        public void HandleFirstMessageInQueue()
        {
            HandleIncomingMessageTimerElapsedHandler(null);
        }

        public void Action(string action)
        {
            switch(action)
            {
                case "PopFirstMessageInQueue":
                    HandleFirstMessageInQueue();
                    break;
            }

        }


        #region private_methods

        /* *********************************************************************************************
         * Private methods
         ********************************************************************************************* */


        private void StartPeriodicRequestForRcuStatusMessage()
        {
            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "StartReceiver for "+NameOfRemoteLcu);
            int period = 1000;
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(timerElapsedHandler, TimeSpan.FromMilliseconds(period));
        }

        private void timerElapsedHandler(ThreadPoolTimer timer)
        {
            // Ask for status of remote lcu. The response will be handled elsewhere. 
            // Maybe I need to check if I get any answer....?
            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Sending GetStatus to " + NameOfRemoteLcu);

            SendRequestOfRcuStatusMessage();
        }

        private void StartPeriodicHandlingOfIncomingMessages()
        {
            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "StartPeriodicHandlingOfIncomingMessages for " + NameOfRemoteLcu);
            int period = 1000;
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(HandleIncomingMessageTimerElapsedHandler, TimeSpan.FromMilliseconds(period));
        }

        private void HandleIncomingMessageTimerElapsedHandler(ThreadPoolTimer timer)
        {
            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "HandleIncomingMessageTimerElapsedHandler for " + NameOfRemoteLcu);

            if (incomingQueue.Count > 0)
            {
                var item = incomingQueue.FindLast(i=>true);

                var clone = item.Clone();
                incomingQueue.Remove(item);
                HandleMessageFromQueue(clone);
                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, lcu.Name + " handled queued message. "+incomingQueue.Count + " messages in queue now.");
            }
        }



        //public void SendCurrentStatusMessage(AlarmHandler.AlarmActivityStatus currentStatus)
        //{
        //    var remoteMessage = new RemoteMessage() { Id = GetNewMessageId(), Message = MessageStartUp + RemoteMessage.MessagPartsDelimeter + currentStatus + RemoteMessage.MessagPartsDelimeter };

        //    var response = SendCommand(ipAddress, remoteMessage.TotalMessage);
        //    var interpretedResult = InterpretResponse(response);
        //    // todo Jag måste väl kolla att jag får ett acc här?
        //}

        // Sends a request to the RCU to get its status.
        // The answer is send as a new message and not as a direct response.
        private void SendRequestOfRcuStatusMessage()
        {
            var getRcuStatusMessage = new RequestRcuStatusMessage(StatusBaseMessage.GetNewMessageId(), lcu.Name);
            var response = SendCommand(ipAddress, getRcuStatusMessage);
        }

        private async Task<string> SendCommand(string hostIp, string command)
        {
            return await SendCommandSpecific(hostIp, RemoteCentralUnitsController.MessageStartToken + RemoteCentralUnitsController.MessagPartsDelimeter + command);
        }



        //private RemoteLcuPingResponse InterpretResponse(Task<string> response)

        private RemoteLcuResponse InterpretResponse(Task<string> response)
        {
            //todo investigate response.Result and create a  RemoteLcuPingResponse from that data. Hardcoded test value below.
            return new RemoteLcuResponse(){ResponderName = "Some ecu", Response = true, ResponseTime = new TimeSpan(0,0, 1)};
        }

        // Sends the specific transfer command object to the rcu.
        private async Task<string> SendCommand(string hostIp, ITransferObject command)
        {
            return await SendCommandSpecific(hostIp, command.CompleteMessageStringToSend);
        }

        // Sends our status the the RCU. This could typically be the response to a request for the current status.
        private async Task<string> SendCurrentStatusToRcu()
        {
            var ourCurrentStatus = new CurrentStatusMessage(lcu.Door.IsOpen, lcu.Door.IsFloating, lcu.Door.IsLocked, lcu.IsSabotaged, lcu.LcuAlarmHandler.CurrentLocalStatus, lcu.Name);

            return await SendCommand("localhost", ourCurrentStatus);
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




        //    private int receiveCounter;
        //    private async void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        //    {
        //        receiveCounter++;
        //        Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, " received something!");
        //        lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, " received something!");

        //        string request;
        //        using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
        //        {
        //            while (true)
        //            {
        //                request = await streamReader.ReadLineAsync();


        //                var transferObject = BuildTransferObjectFromPortStringMessage(request);
        //                string loggMessage = "Got message " + transferObject.MessageType + "with id " + transferObject.Id +
        //                                     " on rpn " + portNumber + " probably from " + NameOfRemoteLcu;
        //                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, loggMessage);
        //                //receivedObjects.Add(transferObject);

        //                lcu.OnRcuReceivedMessage(this, Definition.MessageType.ReceiveCounter, receiveCounter.ToString());
        //                lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, loggMessage);



        //                var communicationResponse = CreateCommunicationResponseObject(transferObject);
        //            }

        //            //if (communicationResponse == null)
        //            //{
        //            //    Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Will not send ACK to this command");
        //            //    return;
        //            //}

        //            /*
        //            string ackResponse = communicationResponse.GetAckResponse();

        //            using (var outputStream = args.Socket.OutputStream.AsStreamForWrite())
        //            {
        //                using (var streamWriter = new StreamWriter(outputStream))
        //                {
        //                    await streamWriter.WriteLineAsync(ackResponse);
        //                    await streamWriter.FlushAsync();
        //                }
        //            }

        //            Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Sent ack to message with id " + transferObject.Id);

        //            // Ack has been sent. Now time to interpret the message.
        //            */

        //            /* RIGHT NOW WE DO NOT DO ANY ACTION
        //            if (transferObject.MessageType == MessagePing)
        //            {
        //                SendPingResponse(transferObject);

        //            }
        //            else if (transferObject.MessageType == MessageGetStatus)
        //            {
        //                // A remote lcu has requested to get our status
        //                await SendCurrentStatusAsync();
        //            }
        //            else if (transferObject.MessageType == MessageCurrentStatus)
        //            {
        //                // We've got the status of the RCU, store it here for future use.
        //                RcuCurrentStatusMessage = (CurrentStatusMessage) transferObject;
        //            }
        //            else if (transferObject.MessageType == MessageUnknown)
        //            {
        //                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Could not interpret unknown message");
        //            }

        //*/
        //            // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
        //            // the text back to the UI thread as shown.

        //            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
        //            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));
        //            //lcu.AddLogging(string.Format("The server sent back the response: \"{0}\"", response));
        //            sender.Dispose();

        //            // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
        //            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add("server closed its socket"));

        //        }
        //    }

        // Static method that translates the specified read data from a port into the correct ITransferObject.
        // Returns null if the string could not be interpreted as a known ITransferObject.
        //private static ITransferObject BuildTransferObjectFromPortStringMessage(string readPortData)
        //{
        //    var parts = readPortData.Split(RemoteCentralUnitsController.MessagPartsDelimeter[0]);
        //    if (parts.Length < 3)
        //    {
        //        throw new Exception("Unknown message syntax. Command much contain \"Msg\";<Id>;<Msg> at least!");
        //    }
        //    if(parts[0] != RemoteCentralUnitsController.MessageStartToken)
        //    {
        //        Logger.Logg("", Logger.RCUProxy_Cat, "Illegal start of command: " + parts[0]);

        //        var unknownMessage = new UnknownMessage(readPortData);
        //        return unknownMessage;
        //    }

        //    if(parts[2] == RemoteCentralUnitsController.MessageGetStatus)
        //    {
        //        return new RequestRcuStatusMessage(parts[1]);
        //    }
        //    else if(parts[2] == RemoteCentralUnitsController.MessageCurrentStatus)
        //    {
        //        var currentAlarmStatusForRcu = (AlarmHandler.AlarmActivityStatus)Int32.Parse(parts[3]);
        //        var rcuCurrentStatus = new CurrentStatusMessage(currentAlarmStatusForRcu);
        //        rcuCurrentStatus.Id = parts[1];
        //        return rcuCurrentStatus;
        //    }
        //    throw new Exception("Unknown message: " + parts[2]);
        //}


        //private async Task SendCurrentStatusAsync()
        //{
        //    Logger.Logg(lcu.Name, Logger.RCUProxy_Cat,
        //        "Sending message CurrentStatus as a response on earlier received request.");

        //    //var currentStatusCommand = CreateCurrentStatusMessage(lcu); //todo source
        //    var currentStatusMessage = new CurrentStatusMessage(lcu.LcuAlarmHandler.CurrentLocalStatus);

        //    //var currentStatusCommandTransferObject = RemoteMessage.CreateCurrentStatusMessageTransferObject(lcu); //todo source
        //    await SendCommand(ipAddress, currentStatusMessage);
        //    //await SendCommandSpecific(ipAddress, currentStatusCommand); //todo ip
        //}

        //public static string CreateCurrentStatusMessage(LocalCentralUnit lcu)
        //{
        //    string currentStatusCommand = RemoteCentralUnitProxy.MessageStartToken
        //                                  + RemoteCentralUnitProxy.MessagPartsDelimeter
        //                                  + RemoteCentralUnitProxy.MessageCurrentStatus
        //                                  + RemoteCentralUnitProxy.MessagPartsDelimeter
        //                                  + lcu.LcuAlarmHandler.HasIntrusionOccurredLocally
        //                                  + RemoteCentralUnitProxy.MessagPartsDelimeter
        //                                  + lcu.LcuRemoteCentralUnitsController
        //                                      .HasIntrusionOccurred()
        //                                  + RemoteCentralUnitProxy.MessagPartsDelimeter
        //                                  + lcu.LcuDoorController.IsDoorLocked();



        //    return currentStatusCommand;
        //}



        private void SendPingResponse(ITransferObject message)
        {
            // Ping does not need a response, the ack that already has been sent is enough.
        }

        private CommunicationResponse CreateCommunicationResponseObject(ITransferObject receivedMessage)
        {
            return new CommunicationResponse(){AckedMessaged = receivedMessage.MessageType, Id = receivedMessage.Id};
        }
        #endregion private_methods
    }
}
