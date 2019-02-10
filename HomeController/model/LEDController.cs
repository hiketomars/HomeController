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
    public class LEDController : ILEDController
    {
        private readonly IRgbLed rgbLed;
        private LedFlashPattern ledFlashPattern;
        private DispatcherTimer timer;


        public LEDController(IRgbLed regbLed)
        {
            this.rgbLed = regbLed;
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

        public void PerformStartUpLedFlash()
        {
            ledFlashPattern = new LedFlashPattern(
                new int[] {
                    // Three fast red flashes.
                    255, 0, 0, 200,
                    0, 0, 0, 200,

                    255, 0, 0, 200,
                    0, 0, 0, 200,

                    255, 0, 0, 200,
                    0, 0, 0, 200,

                    // Three fast green flashes.
                    0, 255, 0, 200,
                    0, 0, 0, 200,

                    0, 255, 0, 200,
                    0, 0, 0, 200,

                    0, 255, 0, 200,
                    0, 0, 0, 200,

                    // Three fast blue flashes.
                    0, 0, 255, 200,
                    0, 0, 0, 200,

                    0, 0, 255, 200,
                    0, 0, 0, 200,

                    0, 0, 255, 200,
                    0, 0, 0, 200,

                }, 1);
            StartLedPattern();
        }

        public void StopLedPattern()
        {
            timer.Stop();
        }

        public IRgbLed ControlledRgbLed => rgbLed;

        public void SetTotalColor(RGBValue green)
        {
            
        }

        public RGBValue GetLedColor()
        {
            return RGBValue.Red;//todo
        }

        private int currentPos;
        private int currentCycle;
        private void Timer_Tick(object sender, object e)
        {
            if(currentPos >= ledFlashPattern.RGBLEDPeriods.Count)
            {
                // Time to start from the beginning in the pattern.
                currentPos = 0;
                currentCycle++;
                // Continue for ever if it is an eternal cycle, otherwise just do the correct amount of cycles.
                if((currentCycle >= ledFlashPattern.Cycles) && !ledFlashPattern.EternalCycles)
                {
                    timer.Stop();
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
                //Logger.Logg("Soon leaving Timer_tick. Interval set to " + timer.Interval + " ms.");
            }
        }
    }
}
