﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HomeController.model;
using HomeController.utils;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeController
{
    public class FakeLcu : ILocalCentralUnit
    {
        public string Name {
            get => "FakeLcu";
        }

        public IDoor Door { get; }

        public string PortNumber { get; }
        public IAlarmHandler LcuAlarmHandler { get; }
        public IRemoteCentralUnitsController LcuRemoteCentralUnitsController { get; }
        public bool UseAnyMockedDoorProperty { get; set; }
        public bool UseVirtualDoorOpen { get; set; }
        public bool UseVirtualDoorFloating { get; set; }
        public bool UseVirtualDoorLocked { get; set; }

        public bool? IsSabotaged { get; }

        public void OnRcuReceivedMessage(IRemoteCentralUnitProxy rcu, Definition.MessageType messageType,
            string loggMessage)
        {
        }

        public void Action(string rcuName, string action)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAlarmHandler : IAlarmHandler {
        public void CheckSituation()
        {
            throw new NotImplementedException();
        }

        public void ActivateAlarm(int delayInMs)
        {
            throw new NotImplementedException();
        }

        public AlarmHandler.AlarmActivityStatus CurrentLocalStatus { get; set; }
        public bool HasIntrusionOccurredLocally { get; set; }
        public bool IsAlarmActive { get; }
        public int EntranceDelayMs { get; set; }
        public void DeactivateAlarm()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamSocketAndListenerPage : Page
    {
        public StreamSocketAndListenerPage()
        {
            this.InitializeComponent();
        }

        // Every protocol typically has a standard port number. For example, HTTP is typically 80, FTP is 20 and 21, etc.
        // For this example, we'll choose an arbitrary port number.
        static string PortNumber = "1337";

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Detta använder jag inte nu. Istället finns Clienten i LocalCentralUnit-klassen.
            // this.StartServer();
            // this.StartClient();

            // Nedan finns kod som inte heller används men som kan användas för att testa kommunikationsdelen separat.
            // Denna delen motsvarar att starta servern.
            var fakeLcu2 = new FakeLcu();
            var rcu2 = new RemoteCentralUnitProxy(fakeLcu2, "theRemoteLcu", 22, "localhost", PortNumber);
            //rcu2.StartListeningToRemoteLcu();

            // Denna delen motsvarar att starta klienten.
            var fakeLcu1 = new FakeLcu();
            var rcu1 = new RemoteCentralUnitProxy(fakeLcu1, "theRemoteLcu", 23, "localhost", PortNumber);

            var r = await rcu1.SendCommandSpecific("localhost","hi there");


        }

        //private async void StartServer()
        //{
        //    try
        //    {
        //        var streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

        //        // The ConnectionReceived event is raised when connections are received.
        //        streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;

        //        // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currentlycommunicatio in use.
        //        await streamSocketListener.BindServiceNameAsync(StreamSocketAndListenerPage.PortNumber);

        //        this.serverListBox.Items.Add("server is listening...");
        //    }
        //    catch(Exception ex)
        //    {
        //        Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
        //        this.serverListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }
        //}

        //private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        //{
        //    string request;
        //    using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
        //    {
        //        request = await streamReader.ReadLineAsync();
        //    }

        //    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server received the request: \"{0}\"", request)));

        //    // Echo the request back as the response.
        //    using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
        //    {
        //        using (var streamWriter = new StreamWriter(outputStream))
        //        {
        //            await streamWriter.WriteLineAsync(request);
        //            await streamWriter.FlushAsync();
        //        }
        //    }

        //    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));

        //    sender.Dispose();

        //    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.serverListBox.Items.Add("server closed its socket"));
        //}

        private async void StartClient()
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                using(var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {
                    // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                    var hostName = new Windows.Networking.HostName("localhost");

                    this.clientListBox.Items.Add("client is trying to connect...");

                    await streamSocket.ConnectAsync(hostName, StreamSocketAndListenerPage.PortNumber);

                    this.clientListBox.Items.Add("client connected");

                    // Send a request to the echo server.
                    string request = "Hello, World!";
                    using(Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using(var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

                    this.clientListBox.Items.Add(string.Format("client sent the request: \"{0}\"", request));

                    // Read data from the echo server.
                    string response;
                    using(Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using(StreamReader streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

                    this.clientListBox.Items.Add(string.Format("client received the response: \"{0}\" ", response));
                }

                this.clientListBox.Items.Add("client closed its socket");
            }
            catch(Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }
    }
}
