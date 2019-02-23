using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public void SetLed_IntrusionHasOccurred()
        {
            SetAndStartFlash(RGBValue.Red, RGBValue.Blue, 1000);
        }

        // Patterns for Inactive Alarms.

        public void Reset()
        {
            timer.Stop();
            rgbLed.SetRGBValue(RGBValue.Black);
        }

        public void SetLed_RemoteIntrusionHasOccurred()
        {
        //    ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreenWithSpecialIndication();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidRed();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreenWithSpecialIndication();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsOpenButAllOtherDoorsAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateFlash1Second50percentGreen50PercentBlack();
            StartLedPattern();
        }

        public void SetLed_AlarmIsInactiveAndDoorIsOpenAndNotAllOtherDoorsAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsActiveAndDoorIsOpenAndLocked_StatusError()
        {
            ledFlashPattern = LedPatternFactory.CreateFlash100MS50percentBlue50PercentBlack();
            StartLedPattern();
        }

        // Patterns for Active Alarms.
        public void SetLed_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void SetLed_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell()
        {
            ledFlashPattern = LedPatternFactory.CreateSolidGreen();
            StartLedPattern();
        }

        public void StopLedPattern()
        {
            timer.Stop();
        }

        public IRgbLed ControlledRgbLed => rgbLed;

        public void SetTotalColor(RGBValue color)
        {
            rgbLed.SetRGBValue(color);
        }

        public RGBValue GetLedColor()
        {
            return rgbLed.GetColor();
        }

        // One color flash.
        private void SetAndStartFlash(RGBValue rgbValue, int timeMs)
        {
            ledFlashPattern = new LedFlashPattern(
                new int[] {
                    rgbValue.RedPart, rgbValue.GreenPart, rgbValue.BluePart, timeMs,
                    0, 0, 0, timeMs
                }, -1);
            StartLedPattern();
        }

        // Two color flash.
        private void SetAndStartFlash(RGBValue rgbValue1, RGBValue rgbValue2, int timeMs)
        {
            ledFlashPattern = new LedFlashPattern(
                new int[] {
                    rgbValue1.RedPart, rgbValue1.GreenPart, rgbValue1.BluePart, timeMs,
                    rgbValue2.RedPart, rgbValue2.GreenPart, rgbValue2.BluePart, timeMs
                }, -1);
            StartLedPattern();
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
