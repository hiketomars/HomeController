﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Devices.Gpio;
using static HomeController.model.Door;
using Windows.Devices.Gpio;
using HomeController.comm;
using HomeController.utils;
using SocketComm;

namespace HomeController.model
{
    /// <summary>
    /// Represents a LED with the possibility to show three colors, red, green and blue.
    /// Currently there is no support for showing combinations of these colors at different intensity.
    /// That is, each of these three colors are either on or off at any given point in time.
    /// </summary>
    public sealed class RgbLed : GpioConnector, IRgbLed
    {
        public bool UseVirtualRgbLedSignal { get; set; }
        private bool VirtualRedLedSignal { get; set; }
        private bool VirtualGreenLedSignal { get; set; }
        private bool VirtualBlueLedSignal { get; set; }

        public event Definition.LEDChangedEventHandler LEDHasChanged;

        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPin bluePin;

        private DispatcherTimer timer;
        private GpioPin pin;
        private RGBValue[] patternLed;
        private readonly int redPinNumber;
        private readonly int greenPinNumber;
        private readonly int bluePinNumber;

        private GpioController gpioController;

        // This constructor uses default pins on the hardware to control the LED.
        // If the LED is connected on different pins the other constructor must be used.
        public RgbLed() : this(5, 6 , 13)
        {
        }

        public RgbLed(int redPinNumber, int greenPinNumber, int bluePinNumber)
        {
            this.redPinNumber = redPinNumber;
            this.greenPinNumber = greenPinNumber;
            this.bluePinNumber = bluePinNumber;
            //this.visualizeLed = visualizeLed;

            if (!UseVirtualRgbLedSignal && ExecutionHandler.OsHasGpioCapacity())
            {
                InitGpio();
            }

            SetRGBValue(0);
            //visualizeLed(MainPage.LEDGraphColor.Gray, initGpioResult);
        }

        public void OnLEDHasChanged(RGBValue rgbValue)
        {
            if (LEDHasChanged != null)
            {
                LEDHasChanged(rgbValue);
            }
        }

        //private MainPage.VisualizeLed visualizeLed;
        //internal void SetVisualizeLedDelegate(MainPage.VisualizeLed visualizeLed)
        //{
        //    this.visualizeLed = visualizeLed;
        //}

        // Sets red, green and blue parts to the same specified value.

        public void SetRGBValue(byte rgbValueAllLeds)
        {
            SetRGBValue(new RGBValue(rgbValueAllLeds, rgbValueAllLeds, rgbValueAllLeds));
        }

        // Sets red, green and blue parts to the specified values.
        // Sets the pins that is connected to the red, green and blue LED wires.
        // Currently any value for Red greater than 0 means that that pin is set high. The same for Green and Blue.
        public void SetRGBValue(RGBValue rgbValue)
        {
            bool atLeastOneColorTurnedOn = false;
            //var colorMix = GetColorMix(rgbValue);
            //List<MainPage.LEDGraphColor> colors = new List<MainPage.LEDGraphColor>();
            if (rgbValue.RedPart > 0)
            {
                SetHigh(redPin);
                //colors.Add(MainPage.LEDGraphColor.Red);
                // todo visualizeLed(MainPage.LEDGraphColor.Red, "Röd");
                atLeastOneColorTurnedOn = true;
            }
            else
            {
                SetLow(redPin);
            }

            if (rgbValue.GreenPart > 0)
            {
                SetHigh(greenPin);
                //colors.Add(MainPage.LEDGraphColor.Green);
                //todo visualizeLed(MainPage.LEDGraphColor.Green, "Grön");
                atLeastOneColorTurnedOn = true;
            }
            else
            {
                SetLow(greenPin);
            }

            if (rgbValue.BluePart > 0)
            {
                SetHigh(bluePin);
                //colors.Add(MainPage.LEDGraphColor.Blue);
                //todo visualizeLed(MainPage.LEDGraphColor.Blue, "Blå");
                atLeastOneColorTurnedOn = true;
            }
            else
            {
                SetLow(bluePin);
            }
            if (!atLeastOneColorTurnedOn)
            {
                //todo visualizeLed(MainPage.LEDGraphColor.Gray, Class1.ColorGray);
            }

            OnLEDHasChanged(rgbValue);
        }


        private void SetHigh(GpioPin aPin)
        {
            SetVirtualPin(aPin, true);
            if(aPin != null && !UseVirtualRgbLedSignal)
            {
                aPin.Write(GpioPinValue.High);
            }
        }

        private void SetLow(GpioPin aPin)
        {
            SetVirtualPin(aPin, false);

            if(aPin != null && !UseVirtualRgbLedSignal)
            {
                aPin.Write(GpioPinValue.Low);
            }
        }

        private void SetVirtualPin(GpioPin aPin, bool value)
        {
            if (aPin == redPin)
            {
                VirtualRedLedSignal = value;
            }else if (aPin == greenPin)
            {
                VirtualGreenLedSignal = value;
            }
            else if (aPin == bluePin)
            {
                VirtualBlueLedSignal = value;
            }
            else
            {
                throw new Exception("Unknown pin");
            }
        }


        // Need to be sealed in order to avoid inherit this class since that would cause
        // Virutal member call in constructor:
        // https://www.jetbrains.com/help/resharper/2018.3/VirtualMemberCallInConstructor.html
        public override void InitGpio()
        {
            base.InitGpio();

            redPin = gpioController.OpenPin(redPinNumber);;
            redPin.Write(GpioPinValue.High);
            redPin.SetDriveMode(GpioPinDriveMode.Output);

            greenPin = gpioController.OpenPin(greenPinNumber);
            greenPin.Write(GpioPinValue.High);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);

            bluePin = gpioController.OpenPin(bluePinNumber);
            bluePin.Write(GpioPinValue.High);
            bluePin.SetDriveMode(GpioPinDriveMode.Output);
        }



        //private LedPattern theLedPattern;
        //public LedPattern TheLedPattern
        //{
        //    get
        //    {
        //        return theLedPattern;
        //    }
        //    set
        //    {
        //        theLedPattern = value;

        //        patternLed = new RGBValue[updateRate];

        //        switch (theLedPattern) {
        //            case LedPattern.AlarmActiveAllDoorsLocked:
        //                //FillPattern(patternLed, 0, patternLed.Length - 1, new RGBValue(0b1, 0b0, 0b0)); // Red all the time.

        //                break;
        //            case LedPattern.AlarmActiveDoorNotLocked:           // Snabbt blinkande rött 50%, grönt 50%.
        //                //FillPattern(patternLed, new RGBValue(0b1, 0b0, 0b0), new RGBValue(0b0, 0b0, 0b0), new RGBValue(0b0, 0b1, 0b0), 100); // 100 ms i varje färg.
        //                break;
        //            case LedPattern.AlarmActiveDoorLockedButNotDoor1:   // 9 snabbt blinkande rött 50%. 10:e grönblink 50%.

        //                break;
        //            case LedPattern.AlarmActiveDoorLockedButNotDoor2:   // Lugnt blinkande rött 50%. Två korta grönblink 2%.
        //                break;
        //            case LedPattern.AlarmActiveDoorLockedButNotDoor3:   // Lugnt blinkande rött 50%. Tre korta grönblink 2%.
        //                break;
        //            case LedPattern.AlarmInactiveDoorNotLocked:         // Släckt
        //                break;
        //            case LedPattern.AlarmInactiveDoorLocked:            // Lyser rött
        //                break;
        //            case LedPattern.AlarmInactiveDoorLockedButNotDoor1: // Lyser rött med väldigt korta släckningar.
        //                break;
        //            case LedPattern.AlarmInactiveDoorLockedButNotDoor2: // Lyser rött med väldigt korta dubbla släckningar.
        //                break;
        //            case LedPattern.AlarmInactiveDoorLockedButNotDoor3:  // Lyser rött med väldigt korta trippelsläckningar.
        //                break;

        //        }
        //    }
        //}

        // Fills RGBValue[] with alternating values. For flashing LED.
        //public void FillPattern(RGBValue[] p, RGBValue color1, RGBValue color2, byte blockLengthMs)
        //{
        //    int blockLength = blockLengthMs / resolution;
        //    for (int i = 0; i < p.Length; i += blockLength)
        //    {
        //        FillPattern(p, i, i + blockLength-1, color1);
        //        FillPattern(p, i + blockLength, i + blockLength + blockLength-1, color2);
        //    }
        //}

        // Fills the RGBValue[] with values within specified interval.
        //public void FillPattern(RGBValue[]p, int start, int stop, RGBValue color)
        //{
        //    for (int i = start; i <= stop && i<p.Length; i++)
        //    {
        //        p[i] = color;
        //    }
        //}

        public void Red(bool offOn)
        {
            throw new NotImplementedException();
        }

        public void Green(bool offOn)
        {
            throw new NotImplementedException();
        }

        public void Blue(bool offOn)
        {
            throw new NotImplementedException();
        }

        public void AllColors(bool offOn)
        {
            throw new NotImplementedException();
        }

        public RGBValue GetColor()
        {
            throw new NotImplementedException();
        }
    }
}
