
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

namespace HomeController
{
    public sealed partial class MainPage : Page
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
        private HouseController houseController; // Representerar andra RPi vid andra dörrar.

        public MainPage()
        {
            InitializeComponent();

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(100);
            //timer.Tick += Timer_Tick;
            //InitGPIO();
            //RgbLed.InitGPIO();
            InitModels();
            LED.Fill = redBrush;

            PerforStartSequence();
            //if (pinRedLED != null)
            //{
            //    timer.Start();
            //}        
        }

        public delegate void VisualizeLed(LEDGraphColor color, string message = "");
        public enum LEDGraphColor { Red, Green, Blue, Gray }
        public void VisualizeLedInColor(LEDGraphColor color, string message)
        {
            switch (color)
            {
                case LEDGraphColor.Red:
                    LED.Fill = redBrush;
                    break;

                case LEDGraphColor.Green:
                    LED.Fill = greenBrush;
                    break;

                case LEDGraphColor.Blue:
                    LED.Fill = blueBrush;
                    break;

                case LEDGraphColor.Gray:
                    LED.Fill = grayBrush;
                    break;

                default:
                    LED.Fill = yellowBrush;
                    break;

            }

            GpioStatus.Text = message;

        }


        private void InitModels()
        {
            door = new Door();
            houseController = new HouseController();
        }

        private void PerforStartSequence()
        {
            //LedFlashPattern ledFlashPattern = new LedFlashPattern(new List<RGBLEDPeriod>(){new RGBLEDPeriod)
            //LEDController ledController = new LEDController(new RgbLed(VisualizeLedInColor), new LedFlashPattern(
            //    new int[] {
            //                255, 0, 0, 500,
            //                0, 0, 0, 500,

            //                255, 0, 0, 500,
            //                0, 0, 0, 500,

            //                255, 0, 0, 500,
            //                0, 0, 0, 500,

            //                0, 255, 0, 1000,
            //                0, 0, 0, 1000,

            //                0, 255, 0, 1000,
            //                0, 0, 0, 1000,

            //                0, 255, 0, 1000,
            //                0, 0, 0, 1000,

            //                0, 0, 255, 200,
            //                0, 0, 0, 500,

            //                0, 0, 255, 200,
            //                0, 0, 0, 500,

            //                0, 0, 255, 200,
            //                0, 0, 0, 500,

            //                255, 0, 0, 500,
            //                0, 255, 0, 500,
            //                0, 0, 255, 500,

            //                0, 0, 0, 3000

            //                }),
            //                VisualizeLedInColor);

            LEDController ledController = new LEDController(new RgbLed(VisualizeLedInColor), new LedFlashPattern(
                new int[] {
                            // Three fast red flashes.
                            255, 0, 0, 200,
                            0, 0, 0, 200,

                            255, 0, 0, 200,
                            0, 0, 0, 200,

                            255, 0, 0, 200,
                            0, 0, 0, 200,

                            0, 0, 0, 500,


                            // Three fast green flashes.
                            0, 255, 0, 200,
                            0, 0, 0, 200,

                            0, 255, 0, 200,
                            0, 0, 0, 200,

                            0, 255, 0, 200,
                            0, 0, 0, 200,

                            0, 0, 0, 500,


                            // Three fast blue flashes.
                            0, 0, 255, 200,
                            0, 0, 0, 200,

                            0, 0, 255, 200,
                            0, 0, 0, 200,

                            0, 0, 255, 200,
                            0, 0, 0, 200,

                            0, 0, 0, 3000

                            }),
                            VisualizeLedInColor);
            ledController.StartLedPattern();
        }

        private void GpioStatus_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        //private void Sleep(int delay)
        //{
        //    var t = Task.Run(async delegate
        //    {
        //        await Task.Delay(delay);
        //        return 42;
        //    });
        //    t.Wait();
        //}

        //private void InitGPIO()
        //{
        //    GpioController gpio = GpioController.GetDefault();

        //    // Show an error if there is no GPIO controller
        //    if (gpio == null)
        //    {
        //        pinRedLED = null;
        //        GpioStatus.Text = "There is no GPIO controller on this device.";
        //        return;
        //    }

        //    pinRedLED = gpio.OpenPin(GPIO_PIN_RED_RGB_LED);
        //    pinValue = GpioPinValue.High;
        //    pinRedLED.Write(pinValue);
        //    pinRedLED.SetDriveMode(GpioPinDriveMode.Output);

        //    pinGreenLED = gpio.OpenPin(GPIO_PIN_GREEN_RGB_LED);
        //    pinValue = GpioPinValue.High;
        //    pinGreenLED.Write(pinValue);
        //    pinGreenLED.SetDriveMode(GpioPinDriveMode.Output);

        //    pinBlueLED = gpio.OpenPin(GPIO_PIN_BLUE_RGB_LED);
        //    pinValue = GpioPinValue.High;
        //    pinBlueLED.Write(pinValue);
        //    pinBlueLED.SetDriveMode(GpioPinDriveMode.Output);



        //    GpioStatus.Text = "GPIO pin initialized correctly.";

        //}


        // Called once every 1/10 of a second to check status and to act.
        //private void Timer_Tick(object sender, object e)
        //{
        //    if (!door.Closed)
        //    {
        //        // Door is open.
        //        if (door.IsDetectedOpenAtThisPoll())
        //        {
        //            if (houseController.AlarmIsActive)
        //            {
        //                houseController.StartEntrance(door);
        //            }else
        //            {
        //                houseController.RegisterEntrance(door);
        //            }
        //        }
        //    }

        //    door.SetAppropriteLedPattern(houseController);



        //    if (pinValue == GpioPinValue.High)
        //    {
        //        pinValue = GpioPinValue.Low;
        //        pinRedLED.Write(pinValue);
        //        LED.Fill = redBrush;
        //    }
        //    else
        //    {
        //        pinValue = GpioPinValue.High;
        //        pinRedLED.Write(pinValue);
        //        LED.Fill = grayBrush;
        //    }
        //}


    }
}

