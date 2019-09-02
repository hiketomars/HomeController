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
using System.Globalization;
using Windows.UI.Core;
using HomeController.model;
using Windows.UI.Xaml;
using System.Threading;
using Windows.Networking.Sockets;
using Windows.System.Threading;
using Castle.Components.DictionaryAdapter;
using HomeController.comm;
using HomeController.config;

namespace HomeController.model
{
    // Each LCU has exactly one object of this class.
    // This object controls the communication with other lcu's, so called remote lcu's.
    // When created it checks the configuration for which remote ecu:s to expect to be found.
    // The object is the one that listens to the port for incoming messages from RCU:s. 
    // 190819
    public class RemoteCentralUnitsController : IRemoteCentralUnitsController
    {
        public const char MessagPartsDelimeter = ';';
        public const string MessageStartToken = "Msg";

        public const string MessageCurrentStatus = "msgCurrentStatus"; // Delivery of current status from RCU.
        public const string MessagePing = "msgPing";
        public const string MessageGetStatus = "msgGetStatus"; // A request for the LCU:s status.
        public const string MessageUnknown = "msgUnknown"; // A request that was not recognized as a valid message.


        //private LocalCentralUnit lcu;
        private readonly IRemoteCentralUnitProxy rcu;

        
        private IConfigHandler configHandler;
        //private List<IRemoteCentralUnitProxy> configuredRemoteCentralUnitProxys = new List<IRemoteCentralUnitProxy>();
        private List<IRemoteCentralUnitProxy> remoteCentralUnitProxies = new List<IRemoteCentralUnitProxy>();

        //// This constructor is for real use.
        //public RemoteCentralUnitsController(LocalCentralUnit lcu)
        //{
        //    configuredRemoteCentralUnitProxys = lcu.LcuConfigHandler.GetRemoteLcus();

        //    foreach(var configuredRemoteCentralUnit in configuredRemoteCentralUnitProxys)
        //    {
        //        IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(lcu, configuredRemoteCentralUnit.Name, configuredRemoteCentralUnit.IpAddress, configuredRemoteCentralUnit.PortNumber);
        //        //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
        //        remoteCentralUnitProxies.Add(remoteCentralUnit);

        //    }

        //}


        // For debug purpose.
        public void RequestStatusFromRcu()
        {
            if (remoteCentralUnitProxies.Count != 1)
            {
                throw new Exception("RequestStatusFromRcu only available when you have a single RCU.");
            }

            remoteCentralUnitProxies[0].RequestStatusFromRcu();
        }

        //public void ListenToTheOnlyRcu()
        //{
        //    if(remoteCentralUnitProxies.Count != 1)
        //    {
        //        throw new Exception("ListenToOnlyRcu only available when you have a single RCU.");
        //    }

        //    remoteCentralUnitProxies[0].StartListeningToRemoteLcu();
        //}

        //public void ListenToRcu(string rcuName)
        //{
        //    var rcu = remoteCentralUnitProxies.Find(e=>e.NameOfRemoteLcu == rcuName);
        //    rcu.StartListeningToRemoteLcu();
        //}

        public void ConnectToRcu(string rcuName)
        {
            var rcu = remoteCentralUnitProxies.Find(e => e.NameOfRemoteLcu == rcuName);
            rcu.ConnectToRcu();
        }

        public void ListenToAllRcus()
        {
            StartLcuListeningOnPort(Lcu.PortNumber);
            //foreach (var rcu in remoteCentralUnitProxies)
            //{
            //    rcu.StartListeningToRemoteLcu();
            //}
        }

        private static StreamSocketListener streamSocketListener; // Eventuellt spara undan med CoreApplication.Properties.Add("listener"+xxx, listener);

        private async void StartLcuListeningOnPort(string portNumber)
        {
            try
            {

                string message = Lcu.Name + " now listen on port " + portNumber + " expecting messages from RCU:s.";
                Logger.Logg(Lcu.Name, Logger.RCUProxy_Cat, message);
                streamSocketListener = new StreamSocketListener();
                //CoreApplication.Properties.Add("listener", listener);


                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
                //streamSocketListener.Control.KeepAlive = false;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListener.BindServiceNameAsync(portNumber);

                Lcu.OnLcuRelatedMessage(Definition.MessageType.Logg, message);


                #region alternatives

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

                #endregion

                //lcu.AddLogging("The server is listening on port " + Definition.OwnPortNumber + "...");
            }
            catch(Exception ex)
            {
                var webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Logger.Logg(Lcu.Name, Logger.RCUProxy_Cat, "Exception in RCUC.StartLcuListeningOnPort " + ex);
            }
        }

        private int receiveCounter;
        private List<ITransferObject> receivedObjects = new EditableList<ITransferObject>();
        private async void StreamSocketListener_ConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {

            receiveCounter++;
            Logger.Logg(Lcu.Name, Logger.RCUProxy_Cat, " received something!");
            //lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, " received something!");

            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                while (true)
                {
                    request = await streamReader.ReadLineAsync();


                    var transferObject = BuildTransferObjectFromPortStringMessage(request);
                    string loggMessage = Lcu.Name + " got message " + transferObject.MessageType + "with id " + transferObject.Id +
                                         " on port " + Lcu.LcuConfigHandler.LcuPortNumber;
                    Logger.Logg(Lcu.Name, Logger.RCUProxy_Cat, loggMessage);
                    receivedObjects.Add(transferObject);

                    //lcu.OnRcuReceivedMessage(this, Definition.MessageType.ReceiveCounter, receiveCounter.ToString());
                    //lcu.OnRcuReceivedMessage(this, Definition.MessageType.Logg, loggMessage);
                    Lcu.OnLcuRelatedMessage(Definition.MessageType.Logg, loggMessage);


                    //var communicationResponse = CreateCommunicationResponseObject(transferObject);


                    // Get the correct proxy from the rcu list based on the name of the rcu in the transferObject.
                    var rcu = RcuList.Find(r => r.NameOfRemoteLcu == transferObject.SendingLcuName);
                    if (rcu != null)
                    {
                        rcu.QueueIncomingMessage(transferObject); //HandleIncomingMessage();
                        Lcu.OnLcuRelatedMessage(Definition.MessageType.Logg, "LCU " + Lcu.Name + " queued incoming message from rcu proxy with name " + transferObject.SendingLcuName);
                    }
                    else
                    {
                        Lcu.OnLcuRelatedMessage(Definition.MessageType.Logg, "LCU "+Lcu.Name + " could not find rcu proxy with name "+transferObject.SendingLcuName + " in its list");
                    }
                }

                //if (communicationResponse == null)
                //{
                //    Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Will not send ACK to this command");
                //    return;
                //}

                /*
                string ackResponse = communicationResponse.GetAckResponse();

                using (var outputStream = args.Socket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(ackResponse);
                        await streamWriter.FlushAsync();
                    }
                }

                Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Sent ack to message with id " + transferObject.Id);

                // Ack has been sent. Now time to interpret the message.
                */

                /* RIGHT NOW WE DO NOT DO ANY ACTION
                if (transferObject.MessageType == MessagePing)
                {
                    SendPingResponse(transferObject);

                }
                else if (transferObject.MessageType == MessageGetStatus)
                {
                    // A remote lcu has requested to get our status
                    await SendCurrentStatusAsync();
                }
                else if (transferObject.MessageType == MessageCurrentStatus)
                {
                    // We've got the status of the RCU, store it here for future use.
                    RcuCurrentStatusMessage = (CurrentStatusMessage) transferObject;
                }
                else if (transferObject.MessageType == MessageUnknown)
                {
                    Logger.Logg(lcu.Name, Logger.RCUProxy_Cat, "Could not interpret unknown message");
                }

    */
                // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                // the text back to the UI thread as shown.

                // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
                //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));
                //lcu.AddLogging(string.Format("The server sent back the response: \"{0}\"", response));
                sender.Dispose();

                // todo Hur ska jag göra detta på ett bra och asynkront sätt? Nu låter jag MVP sköta detta som vanligt.
                //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add("server closed its socket"));

            }

        }


        // Static method that translates the specified read data from a port into the correct ITransferObject.
        // Returns null if the string could not be interpreted as a known ITransferObject.
        // Example can be found in class StatusBaseMessage.
        // todo Lägg in den här koden i konstruktorn istället. /190818
        private static StatusBaseMessage BuildTransferObjectFromPortStringMessage(string readPortData)
        {
            // "Msg;3412413414515;frontDoor;msgCurrentStatus;2;
            //   0       1            2           3          4
            var parts = readPortData.Split(MessagPartsDelimeter);
            if(parts.Length < 4)
            {
                throw new Exception("Unknown message syntax. Command much contain \"Msg\";<RCU>;<Id>;<Msg> at least!");
            }
            var sendingLcuName = parts[2];

            if(parts[0] != MessageStartToken)
            {
                Logger.Logg("", Logger.RCUProxy_Cat, "Illegal start of command: " + parts[0]);

                var unknownMessage = new UnknownMessage(readPortData, sendingLcuName);
                return unknownMessage;
            }

            if(parts[3] == MessageGetStatus)
            {
                // Request for our status.
                return new RequestRcuStatusMessage(parts[1], sendingLcuName);
            }

            if(parts[3] == MessageCurrentStatus)
            {
                string statusParam = parts[4];
                // Received RCU status.
                Enum.TryParse(statusParam, out AlarmHandler.AlarmActivityStatus currentAlarmStatusForRcu);
                var rcuCurrentStatus = new CurrentStatusMessage(currentAlarmStatusForRcu, sendingLcuName);
                rcuCurrentStatus.Id = parts[1];
                return rcuCurrentStatus;
            }
            throw new Exception("Unknown message: " + parts[2]);
        }

        public void ConnectToAllRcus()
        {
            foreach(var rcu in remoteCentralUnitProxies)
            {
                rcu.RequestStatusFromRcu();
            }
        }


        public void ActivateCommunicationOnAllProxys()
        {
            foreach (var configuredRemoteCentralUnit in remoteCentralUnitProxies)
            {
                configuredRemoteCentralUnit.ActivateCommunication();
            }
        }

        // Collect the statuses from the LCU:s and finds out which one has most recently changed its alarm status.
        public CompoundStatus GetCompoundStatus()
        {
            DateTime newestAlarmStatus = new DateTime(1970, 1, 1, 0, 0, 0);
            //RemoteLcuStatus newestLcuStatus = configuredRemoteCentralUnitProxys[0].RemoteLcuStatus; 
            IRemoteCentralUnitProxy mostRecentChangedLcu = remoteCentralUnitProxies[0]; // Initiate //todo Kan det vara så att vi inte har några RCU:er?

            var compoundStatus = new CompoundStatus();
            foreach(var configuredRemoteCentralUnit in remoteCentralUnitProxies)
            {
                compoundStatus.LcuStatuses.Add(configuredRemoteCentralUnit.RcuCurrentStatusMessage);

                if (configuredRemoteCentralUnit.RcuCurrentStatusMessage.StatusTime > mostRecentChangedLcu.RcuCurrentStatusMessage.StatusTime)
                {
                    mostRecentChangedLcu = configuredRemoteCentralUnit;
                }
            }

            compoundStatus.MostRecentChangedLcu = mostRecentChangedLcu;
            return compoundStatus;
        }

        public List<IRemoteCentralUnitProxy> RcuList
        {
            get
            {
                return remoteCentralUnitProxies;
            }
        }
        public LocalCentralUnit Lcu { get; set; }
        private List<IRemoteCentralUnitConfiguration> remoteCentralUnitConfigurations;

        // This constructor is used by unit tests.
        public RemoteCentralUnitsController(List<IRemoteCentralUnitProxy> remoteCentralUnitProxies)
        {
            this.remoteCentralUnitProxies = remoteCentralUnitProxies;
        }
        
        // This constructor is for test use.
        public RemoteCentralUnitsController(LocalCentralUnit lcu, List<IRemoteCentralUnitConfiguration> remoteCentralUnitConfigurations)
        {
            Lcu = lcu;
            this.remoteCentralUnitConfigurations = remoteCentralUnitConfigurations;
        }

        public int RemoteCentralUnitsCount => remoteCentralUnitProxies.Count;


        public void ActivateRemoteCentralUnits()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.ActivateCommunication();
            }
        }

        // This constructor is for test cases.
        //public RemoteCentralUnitsController(LocalCentralUnit lcu, IConfigHandler configHandler)
        //{
        //    this.lcu = lcu;
        //    Setup(configHandler);
        //}


        // If this RemoteCentralUnitsController has been created from a list of RemoteCentralUnitConfigurations then
        // this Setup has to be called to create the actual list of CentralUnitProxies.
        public void Setup(LocalCentralUnit lcu)
        {
            foreach(var remoteCentralUnitConfiguration in remoteCentralUnitConfigurations)
            {
                IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(lcu, remoteCentralUnitConfiguration.Name, remoteCentralUnitConfiguration.Id, remoteCentralUnitConfiguration.IpAddress, remoteCentralUnitConfiguration.PortNumber);
                //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
                remoteCentralUnitProxies.Add(remoteCentralUnit);
                //remoteCentralUnit.ActivateCommunication(); // StartListeningOnRemoteLcu();

            }

            //backdoorRemoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
        }


        public bool VerifyContact()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                var contact = remoteCentralUnitProxy.SendPingMessage();
                if (!contact)
                {
                    Logger.Logg(Lcu.Name, Logger.RCUCtrl_Cat, "VerifyContact: No contact with " + remoteCentralUnitProxy.NameOfRemoteLcu);
                    return false;
                }
            }

            return true;
        }
/* Jag tror inte jag behöver tala om att status har ändrats. De får fråga så svarar jag.
        // Inform other LCU:s that our status has changed.
        public void StatusHasChanged(AlarmHandler.AlarmActivityStatus currentStatus)
        {
            foreach(var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.SendStateChangedMessage(currentStatus);
        public static string RCUCtrl_Cat;
                Logger.Logg(lcu.Name, "Informed " + remoteCentralUnitProxy.Name + " that the state of "+LocalCentralUnit.GetInstance().Name+" has changed to "+currentStatus);
            }
        }
*/
        private void remoteCentralUnit_RemoteLcuStatusHasChanged(string todotype)
        {
            throw new NotImplementedException();
        }

        // Checks if intrusion has occurred in any of the remote central units.
        public bool HasIntrusionOccurred()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                if (remoteCentralUnitProxy.HasIntrusionOccurred())
                {
                    return true;
                }
                else
                {
                    int a = 0;
                }
            }

            return false;
            /*
            try
            {
                var task = SendCommandToRemoteLcu(MessageHasIntrusionOccurred);
                if(task.Result.StartsWith("exception"))
                {
                    return false;
                }

                return bool.Parse(task.Result);
            }
            catch
            {
                return false;
            }
            */
        }

        public bool IsAnyRemoteDoorUnlocked()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                if (remoteCentralUnitProxy.IsDoorUnlocked())
                {
                    return true;
                }
            }

            return false;
            //try
            //{
            //    var task = SendCommandToRemoteLcu(RemoteCentralUnitProxy.MessageIsDoorUnlocked);
            //    if (task.Result.StartsWith("exception"))
            //    {
            //        return true;
            //    }

            //    return bool.Parse(task.Result);
            //}
            //catch
            //{
            //    return true;
            //}
        }

        public void SendStartUpMessage()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.SendStartUpMessage();

            }
            //SendCommandToRemoteLcu(MessageLcuStarting);

        }

        //public void InformAboutNewAlarmStatus(AlarmHandler.AlarmActivityStatus status)
        //{
        //    foreach(var remoteCentralUnitProxy in remoteCentralUnitProxies)
        //    {
        //        remoteCentralUnitProxy.InformAboutNewAlarmStatus();

        //    }
        //    //SendCommandToRemoteLcu(MessageLcuStarting);
        //}
    }

}
