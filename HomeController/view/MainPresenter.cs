using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HomeController.model;
using HomeController.utils;

namespace HomeController.view
{
    /// <summary>
    /// This is a presenter in the MVP-architecture.
    /// </summary>
    public class MainPresenter
    {
        private IMainView mainView;
        private IHouseModel houseModel;
        public MainPresenter(IMainView mainView)
        {
            Logger.Logg("Main", Logger.MainPresenter_Cat, "=============================================");
            Logger.Logg("Main", Logger.MainPresenter_Cat, "MainPresenter");
            this.mainView = mainView;
            houseModel = HouseModelFactory.GetHouseModel();
            houseModel.ModelHasChanged += new Definition.VoidEventHandler(ModelEventHandler_ModelHasChanged);
            houseModel.LCULedHasChanged += new Definition.LEDChangedEventHandler(ModelEventHandler_LCULedHasChanged);
            houseModel.LcuInstancesHasChanged += new Definition.VoidEventHandler(ModelEventHandler_LcuInstancesHasChanged);
            houseModel.RcuReceivedMessage += new Definition.RcuMessageReceivedEventHandler(ModelEventHandler_RcuReceivedMessage);
            houseModel.HomeReceivedMessage += new Definition.HomeMessageReceivedEventHandler(ModelEventHandler_HomeReceivedMessage);
            houseModel.LcuRelatedMessage += new Definition.LcuRelatedMessageEventHandler(ModelEventHandler_LcuRelatedMessage);

            
        }

        private async void ModelEventHandler_LcuRelatedMessage(ILocalCentralUnit lcu, Definition.MessageType messageType, string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Your UI update code goes here!
                    switch(messageType)
                    {
                        case Definition.MessageType.Logg:
                            mainView.AddLcuLoggText(lcu.Name, message + "\r\n");
                            break;
                        case Definition.MessageType.StaticInfo:
                            mainView.SetLcuInfoText(lcu.Name, message);
                            break;
                        default:
                            mainView.AddLcuLoggText(lcu.Name, message + "\r\n");
                            break;
                    }

                });
        }

        // Event handler for messages concerning the Home Controller as a hole.
        private async void ModelEventHandler_HomeReceivedMessage(Definition.MessageType messageType, string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Your UI update code goes here!
                    switch(messageType)
                    {
                        case Definition.MessageType.Logg:
                            mainView.AddHouseLoggText(message + "\r\n");
                            break;
                        default:
                            mainView.AddHouseLoggText(message + "\r\n");
                            break;
                    }

                });
        }

        // Event handler for messages received by RCU.
        private async void ModelEventHandler_RcuReceivedMessage(ILocalCentralUnit lcu, IRemoteCentralUnitProxy rcu, Definition.MessageType messageType, string message)
        {
            // To not get the error 
            // "The application called an interface that was marshalled for a different thread."
            // when updating the GUI from a non-thread ( in this case the method is called from the thread that reads the socket)
            // I need to do this trick. The await is perhaps not needed but if used I need to make the method async.
            // https://stackoverflow.com/questions/16477190/correct-way-to-get-the-coredispatcher-in-a-windows-store-app/18485317#18485317
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Your UI update code goes here!
                    switch (messageType)
                    {
                        case Definition.MessageType.Logg:
                            mainView.AddRcuLoggText(lcu.Name, rcu.NameOfRemoteLcu, message + "\r\n");
                            break;
                        case Definition.MessageType.SendCounter:
                            mainView.AddRcuSendCounterText(lcu.Name, rcu.NameOfRemoteLcu, message);
                            break;
                        case Definition.MessageType.ReceiveCounter:
                            mainView.AddRcuReceiveCounterText(lcu.Name, rcu.NameOfRemoteLcu, message);
                            break;
                        default:
                            mainView.AddRcuLoggText(lcu.Name, rcu.NameOfRemoteLcu, message + "\r\n");
                            break;
                    }

                });
        }


        // The number of Lcu:s that the model handles has changed.
        // Normally it is 1 but can be many if the application is run in that mode.
        public void ModelEventHandler_LcuInstancesHasChanged()
        {
            var lcus = houseModel.GetLcuList();
            mainView.SetLcus(lcus);
            foreach (var lcu in lcus)
            {
                mainView.SetLcuInfoText(lcu.Name,"Port: "+lcu.PortNumber);
            }
        }

        // This is the handler method for the event ModelHasChanged that comes from the model.
        public void ModelEventHandler_ModelHasChanged()
        {
            var loggings = houseModel.GetLoggings();
            var totalString = new StringBuilder();
            foreach (string logging in loggings)
            {
                totalString.Append(logging);
            }

            mainView.AddHouseLoggText(totalString.ToString());
        }

        public void ModelEventHandler_LCULedHasChanged(RGBValue rgbValue)
        {
            //No need to read model here since the value is supplied in the event.
            mainView.SetColorForBackdoorLED(rgbValue);
        }

        internal void StopApplication()
        {
            mainView.AddHouseLoggText("\r\nStop clicked.\r\n");
            Application.Current.Exit();
        }

        internal void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            mainView.AddHouseLoggText("\r\nInfo button clicked.\r\n");
            mainView.AddHouseLoggText("Log path " + Logger.LastUsedLogPath);

        }



        //public void ListenBtn_Click(string lcuName, string rcuName)
        //{
        //    mainView.AddRcuLoggText(lcuName, rcuName, "User initiated " + lcuName + " to listen to " + rcuName + "\r\n");
        //    houseModel.ListenToRCU(lcuName, rcuName);
        //}

        public void ConnectBtn_Click(string lcuName, string rcuName)
        {
            mainView.AddRcuLoggText(lcuName, rcuName, "User initiated " + lcuName + " to connect to " + rcuName + "\r\n");
            houseModel.ConnectToRCU(lcuName, rcuName);
        }

        public void RequestStatusBtn_Click(string lcuName, string rcuName)
        {
            mainView.AddRcuLoggText(lcuName, rcuName, "User initiated "+ lcuName + " to req status from " + rcuName + "\r\n");
            houseModel.RequestStatusFromRCU(lcuName, rcuName);
        }



        public void ConnectAllBtn_Click(string lcuName)
        {
            mainView.AddLcuLoggText(lcuName, "User initiated " + lcuName + " to connected to all rcu:s\r\n");
            houseModel.ConnectToAllRCU(lcuName);
        }

        public void ListenAllBtn_Click(string lcuName)
        {
            mainView.AddLcuLoggText(lcuName, "User initiated " + lcuName + " to listen to all rcu:s\r\n");
            houseModel.ListenToAllRCU(lcuName);
        }

        public void ClearAllBtn_Click(string lcuName)
        {
            mainView.SetLcuLoggText(lcuName, "");
        }

        public void ClearBtn_Click(string lcuName, string rcuName)
        {
            mainView.ClearRcuText(lcuName, rcuName);
        }

        public void ActionSelector_OnSelectionChanged(string lcuName, string selectionChangedEventArgs, object sender, SelectionChangedEventArgs selectionChangedEventArgs1)
        {
        }

        public void ActionBtn_Click(string lcuName, string rcuName, string actionSelectorSelectValue)
        {
            houseModel.ActionBtn(lcuName, rcuName, actionSelectorSelectValue);
        }
    }
}
