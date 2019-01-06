using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using HomeController;
using HomeController.utils;

namespace HomeController.model
{
    // En LEDController kan sköta en LED. Bland annat kan den få den att blinka enligt ett visst mönster.
    public class LEDController
    {
        //public delegate void ChangeLedDelegate();
        //private ChangeLedDelegate ChangeLed;

        private readonly RgbLed rgbLed;
        private LedFlashPattern ledFlashPattern;
        private DispatcherTimer timer;

        public LEDController(RgbLed regbLed) : this(regbLed, null) { }

        public LEDController(RgbLed regbLed, LedFlashPattern ledFlashPattern)
        {
            this.rgbLed = regbLed;
            this.ledFlashPattern = ledFlashPattern;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += Timer_Tick;
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
        private int currentCycle;

        private void Timer_Tick(object sender, object e)
        {
            //ChangeLed();
            if(currentPos >= ledFlashPattern.RGBLEDPeriods.Count)
            {
                // Time to start from the beginning in the pattern.
                currentPos = 0;
                currentCycle++;
                // Coontinue for ever if it is an eternal cycle, otherwise just do the correct amount of cycles.
                if((currentCycle >= ledFlashPattern.Cycles) && !ledFlashPattern.EternalCycles)
                {
                    timer.Stop();
                    //visualizeLed()
                }
            }
            RGBLEDPeriod rgbLedPeriod = ledFlashPattern.RGBLEDPeriods[currentPos++];
            RGBValue rgbValue = rgbLedPeriod.RGBValue;
            rgbLed.SetRGBValue(rgbValue);

            if (rgbLedPeriod.HoldValueMs < 0)
            {
                // This RGBLEDPeriod specifies that the HoldValueMs is for eternity.
                timer.Stop();
                Logger.Logg("Timer in LEDController stopped.");
            }
            else
            {
                timer.Interval = TimeSpan.FromMilliseconds(rgbLedPeriod.HoldValueMs);
                Logger.Logg("Soon leaving Timer_tick. Interval set to " + timer.Interval + " ms.");
            }
        }
    }
}
