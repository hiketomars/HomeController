using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using HomeController;

namespace HomeController.model
{
    // En LEDController kan sköta en LED. Bland annat kan den få den att blinka enligt ett visst mönster.
    public class LEDController
    {
        //public delegate void ChangeLedDelegate();
        //private ChangeLedDelegate ChangeLed;

        private readonly RgbLed rgbLed;
        private LedFlashPattern ledFlashPattern;
        private readonly MainPage.VisualizeLed drawLed;
        private DispatcherTimer timer;

        public LEDController(RgbLed regbLed, MainPage.VisualizeLed drawLed) : this(regbLed, null, drawLed) { }

        public LEDController(RgbLed regbLed, LedFlashPattern ledFlashPattern, MainPage.VisualizeLed drawLed)
        {
            this.rgbLed = regbLed;
            this.ledFlashPattern = ledFlashPattern;
            this.drawLed = drawLed;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += Timer_Tick;

            rgbLed.SetVisualizeLedDelegate(drawLed);
        }

        public void StartLedPattern(LedFlashPattern ledFlashPattern)
        {
            this.ledFlashPattern = ledFlashPattern;
            StartLedPattern();
        }

        public void StartLedPattern()
        {
            if(ledFlashPattern == null)
            {
                throw new Exception("Must set a pattern");
            }
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Start();
        }

        public void StopLedPattern()
        {
            timer.Stop();
        }

        private int currentPos;
        private void Timer_Tick(object sender, object e)
        {
            //ChangeLed();
            if(currentPos >= ledFlashPattern.RGBLEDPeriods.Count)
            {
                // Time to start from the beginning in the pattern.
                currentPos = 0;
            }
            RGBLEDPeriod rgbLedPeriod = ledFlashPattern.RGBLEDPeriods[currentPos++];
            RGBValue rgbValue = rgbLedPeriod.RGBValue;
            rgbLed.SetRGBValue(rgbValue);

            if (rgbLedPeriod.HoldValueMs < 0)
            {
                // This RGBLEDPeriod specifies that the HoldValueMs is for eternity.
                timer.Stop();
            }
            else
            {
                timer.Interval = TimeSpan.FromMilliseconds(rgbLedPeriod.HoldValueMs);
            }
        }
    }
}
