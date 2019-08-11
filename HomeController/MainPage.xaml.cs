
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

// Copyright (c) Microsoft. All rights reserved.

using HomeController.model;
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI.Core;
using HomeController.userControl;
using HomeController.utils;
using HomeController.view;

namespace HomeController
{
    public sealed partial class MainPage : Page, IMainView
    {
        // The number of the GPIO pin on Raspberry Pi 3 model B.

        private const int GPIO_PIN_SIREN = 22;

        private const int GPIO_PIN_DOOR_CLOSED = 12;
        private const int GPIO_PIN_DOOR_LATCHED = 16;
        private const int GPIO_PIN_DOOR_LOCKED = 20;
        private const int GPIO_PIN_DOOR_INTACT = 21; // This means not sabotaged.


        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush yellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private Door door;
        private LcuHandler houseHandler; // Representerar andra RPi vid andra dörrar.
        private MainPresenter mainPresenter;





        public MainPage()
        {
            InitializeComponent();
            mainPresenter = new MainPresenter(this);
            mainPresenter.ModelEventHandler_LcuInstancesHasChanged(); // Update/Initialize.
        }

        private void GpioStatus_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            mainPresenter.InfoBtn_Click(sender, e);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            mainPresenter.StopApplication();
        }

        private void ListenOnPort_Checked(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>        
        /// Loggs text in the house part of the GUI.
        /// </summary>
        /// <param name="text"></param>
        public void AddHouseLoggText(string text)
        {
            this.HouseTextBox.Text += text;
        }

        //// Called from the presenter to set all logg items in this view.
        //// From IMainView.
        //public async void AddHouseLoggText(List<string> loggings)
        //{
        //    // Since this method might be called from another thread other than the GUI-thread we need to use the Dispatcher.
        //    //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.loggListBox.Items.Clear());

        //    //this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.loggListBox.Items.Clear()).GetResults();
        //    //foreach (var logging in loggings)
        //    //{
        //    //    //this.loggListBox.Items.Add(logging);
        //    //    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.loggListBox.Items.Add(logging)).GetResults();
        //    //}
       
        //        //this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ClearAndUpdateLoggItems(loggings))
        //        //    .GetResults();

        //        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ClearAndUpdateLoggItems(loggings));

            

        //}

        // Keeps clear and add of items together in a synchronous call.
        //private void ClearAndUpdateLoggItems(List<string> loggings)
        //{
        //    lock (this) // Don't know if this is needed...
        //    {
        //        this.HouseTextBox.Items.Clear();
        //        foreach (var logging in loggings)
        //        {
        //            this.HouseTextBox.Items.Add(logging);
        //        }
        //    }
        //}

        // Called from presenter to set the GUI color of the backdoor LED.
        // From IMainView.
        public void SetColorForBackdoorLED(RGBValue rgbValue)
        {
            if (rgbValue.HasRedPart)
            {
                LED.Fill = redBrush;
            }else if (rgbValue.HasGreenPart)
            {
                LED.Fill = greenBrush;
            }
            else if (rgbValue.HasBluePart)
            {
                LED.Fill = blueBrush;
            }
            else
            {
                LED.Fill = grayBrush;
            }
        }

        //private List<ILocalCentralUnit> lcus = new List<ILocalCentralUnit>();
        private List<LcuUserControl> lcuUserControls = new List<LcuUserControl>();

        // ---------------------------- LCU ------------------------------------------------
        // Adds text to rcu log.

        public void AddLcuLoggText(string lcuName, string text)
        {
            var lcuUserControl = lcuUserControls.Find(e => e.LcuName == lcuName);
            lcuUserControl.AddTextToOutput(text);
        }

        public void SetLcus(List<ILocalCentralUnit> lcus)
        {
            foreach(var lcu in lcus)
            {
                var lcuUserControl = new LcuUserControl(mainPresenter);
                lcuUserControl.LcuName = lcu.Name;
                lcuUserControl.NameText = lcu.Name;
                lcuUserControl.AddTextToOutput("Lcu " + lcu.Name + " created.");
                foreach(var rcu in lcu.LcuRemoteCentralUnitsController.RcuList)
                {
                    lcuUserControl.AddRcu(rcu.NameOfRemoteLcu);
                }

                LcuStackPanel.Children.Add(lcuUserControl);
                //this.lcus.Add(lcu);
                lcuUserControls.Add(lcuUserControl);
            }
        }

        // ---------------------------- RCU ------------------------------------------------
        // Adds text to rcu logg (within an lcu).
        public void AddRcuLoggText(string lcuName, string rcuName, string text)
        {
            var lcuUserControl = lcuUserControls.Find(e => e.LcuName == lcuName);
            var rcuUserControl = lcuUserControl.GetRcuUserControls().Find(e => e.RcuName == rcuName);
            rcuUserControl.AddTextToOutput(text);
        }

        public void AddRcuSendCounterText(string lcuName, string rcuName, string text)
        {
            var lcuUserControl = lcuUserControls.Find(e => e.LcuName == lcuName);
            var rcuUserControl = lcuUserControl.GetRcuUserControls().Find(e => e.RcuName == rcuName);
            rcuUserControl.SetSendCounterText(text);
        }

        public void AddRcuReceiveCounterText(string lcuName, string rcuName, string text)
        {
            var lcuUserControl = lcuUserControls.Find(e => e.LcuName == lcuName);
            var rcuUserControl = lcuUserControl.GetRcuUserControls().Find(e => e.RcuName == rcuName);
            rcuUserControl.SetReceiveCounterText(text);
        }

        public void ClearRcuText(string lcuName, string rcuName)
        {
            var lcuUserControl = lcuUserControls.Find(e => e.LcuName == lcuName);
            var rcuUserControl = lcuUserControl.GetRcuUserControls().Find(e => e.RcuName == rcuName);
            rcuUserControl.ClearOutput();
        }

        // Not used right now since I'm having another method that takes RGBValue as an argument instead.
        // Called from presenter to set the GUI color of the backdoor LED.
        // From IMainView.
        public void SetColorForBackdoorLED(Definition.LEDGraphColor color)
        {
            switch (color)
            {
                case Definition.LEDGraphColor.Red:
                    LED.Fill = redBrush;
                    break;

                case Definition.LEDGraphColor.Green:
                    LED.Fill = greenBrush;
                    break;

                case Definition.LEDGraphColor.Blue:
                    LED.Fill = blueBrush;
                    break;

                case Definition.LEDGraphColor.Gray:
                    LED.Fill = grayBrush;
                    break;

                default:
                    LED.Fill = yellowBrush;
                    break;

            }
        }

        // When the user clicks this button the LCU makes a connection to the other LCU.
        private void ConnectBtn_OnClickBtn_Click(object sender, RoutedEventArgs e)
        {
            //mainPresenter.ConnectBtn_Click("Inget namn satt");
        }

        private void ListenBtn_OnClickBtn_OnClickBtn_Click(object sender, RoutedEventArgs e)
        {
            //mainPresenter.ListenBtn_Click("Inget LCU-namn satt", "Inget RCU-namn satt");
        }
    }
}

