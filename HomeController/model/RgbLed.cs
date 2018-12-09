using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Devices.Gpio;
using static HomeController.model.Door;
using Windows.Devices.Gpio;

namespace HomeController.model
{
    public class RgbLed
    {
        //public const int GPIO_PIN_RED_RGB_LED = 5;
        //public const int GPIO_PIN_GREEN_RGB_LED = 6;
        //public const int GPIO_PIN_BLUE_RGB_LED = 13;



        private GpioPin redPin;
        private GpioPin greenPin;
        private GpioPin bluePin;

        //private static GpioPin pinRedLED;
        //private static GpioPin pinGreenLED;
        //private static GpioPin pinBlueLED;

        private DispatcherTimer timer;
        private GpioPin pin;
        private RGBValue[] patternLed;
        private readonly int redPinNumber;
        private readonly int greenPinNumber;
        private readonly int bluePinNumber;

        private GpioController gpio;


        public RgbLed(MainPage.VisualizeLed visualizeLed) : this(visualizeLed, 5, 6 , 13)
        {

        }

        public RgbLed(MainPage.VisualizeLed visualizeLed, int redPinNumber, int greenPinNumber, int bluePinNumber)
        {
            this.redPinNumber = redPinNumber;
            this.greenPinNumber = greenPinNumber;
            this.bluePinNumber = bluePinNumber;
            this.visualizeLed = visualizeLed;
            string initGpioResult = InitGPIO(); 

            SetRGBValue(0);
            visualizeLed(MainPage.LEDGraphColor.Gray, initGpioResult);
        }

        private MainPage.VisualizeLed visualizeLed;
        internal void SetVisualizeLedDelegate(MainPage.VisualizeLed visualizeLed)
        {
            this.visualizeLed = visualizeLed;
        }

        // Sets red, green and blue parts to the same specified value.
        public void SetRGBValue(byte rgbValueAllLeds) {
            SetRGBValue(new RGBValue(rgbValueAllLeds, rgbValueAllLeds, rgbValueAllLeds));
        }

        private void SetHigh(GpioPin aPin) {
            if(aPin != null)
            {
                aPin.Write(GpioPinValue.High);
            }
        }

        private void SetLow(GpioPin aPin)
        {
            if (aPin != null)
            {
                aPin.Write(GpioPinValue.Low);
            }
        }

        // Sets red, green and blue parts to the specified values.
        public void SetRGBValue(RGBValue rgbValue)
        {
            bool colorHasBeenSet = false;
            //var colorMix = GetColorMix(rgbValue);
            //List<MainPage.LEDGraphColor> colors = new List<MainPage.LEDGraphColor>();
            if (rgbValue.RedPart > 0)
            {
                SetHigh(redPin);
                //colors.Add(MainPage.LEDGraphColor.Red);
                visualizeLed(MainPage.LEDGraphColor.Red, "Röd");
                colorHasBeenSet = true;
            }
            else
            {
                SetLow(redPin);
            }

            if (rgbValue.GreenPart > 0)
            {
                SetHigh(greenPin);
                //colors.Add(MainPage.LEDGraphColor.Green);
                visualizeLed(MainPage.LEDGraphColor.Green, "Grön");
                colorHasBeenSet = true;
            }
            else
            {
                SetLow(greenPin);
            }

            if (rgbValue.BluePart > 0)
            {
                SetHigh(bluePin);
                //colors.Add(MainPage.LEDGraphColor.Blue);
                visualizeLed(MainPage.LEDGraphColor.Blue, "Blå");
                colorHasBeenSet = true;
            }
            else
            {
                SetLow(bluePin);
            }
            if (!colorHasBeenSet)
            {
                visualizeLed(MainPage.LEDGraphColor.Gray, "Grå");
            }
        }


        public string InitGPIO()
        {
            gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                //GpioStatus.Text = "There is no GPIO controller on this device.";
                return "There is no GPIO controller on this device.";
            }

            redPin = gpio.OpenPin(redPinNumber);;
            redPin.Write(GpioPinValue.High);
            redPin.SetDriveMode(GpioPinDriveMode.Output);

            greenPin = gpio.OpenPin(greenPinNumber);
            greenPin.Write(GpioPinValue.High);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);

            bluePin = gpio.OpenPin(bluePinNumber);
            bluePin.Write(GpioPinValue.High);
            bluePin.SetDriveMode(GpioPinDriveMode.Output);
            return "GPIO initialized";
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

    }
}
