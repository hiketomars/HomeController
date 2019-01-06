using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    // Represents a specific RGB Value (color) for a specified amont of time, e.g. as part of a flash pattern.
    public class RGBLEDPeriod
    {
        //public RGBLEDPeriod(RGBValue rgbValue)
        //{
        //    RGBValue = rgbValue;
        //}
        private int holdValueMs;
        public RGBLEDPeriod(RGBValue rgbValue, int holdValueMs)
        {
            RGBValue = rgbValue;
            this.holdValueMs = holdValueMs;
        }
        private RGBLEDPeriod() { }

        public RGBValue RGBValue { get; set; }

        private int timePeriod10milliSecond = 10;

        // Bestämmer hur lång tidsperiod denna RGBPeriod;s RGBValue gäller.
        public int HoldValueMs
        {
            get
            {
                return holdValueMs;
            }
        }
    }
}
