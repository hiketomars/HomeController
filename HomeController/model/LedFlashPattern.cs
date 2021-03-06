﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    // LedFlashPattern håller en lista med RGBPeriod med vars hjälp det går att bestämma hur LED:en ska blinka.
    public class LedFlashPattern
    {
        public readonly List<RGBLEDPeriod> RGBLEDPeriods;

        public int Cycles { get; }
        public bool EternalCycles
        {
            get {
                return Cycles == -1;
            }
        }

        public LedFlashPattern()
        {
            // Empty list.
            this.RGBLEDPeriods = new List<RGBLEDPeriod>();
        }
        public LedFlashPattern(int [] rgbAntPeriodArray, int cycles = -1)
        {
            int pos = 0;
            this.RGBLEDPeriods = new List<RGBLEDPeriod>();
            for (pos = 0; pos + 3 < rgbAntPeriodArray.Length; pos += 4)
            {
                RGBLEDPeriods.Add(new RGBLEDPeriod(new RGBValue((byte)rgbAntPeriodArray[pos], (byte)rgbAntPeriodArray[pos + 1], (byte)rgbAntPeriodArray[pos + 2]), rgbAntPeriodArray[pos + 3]));
            }

            Cycles = cycles;
        }
        public LedFlashPattern(List<RGBLEDPeriod> rgbLEDPeriods)
        {
            this.RGBLEDPeriods = rgbLEDPeriods;
        }
    }

    public static class LedPatternFactory
    {
        public static LedFlashPattern CreateSolidRed()
        {
            return new LedFlashPattern(new int[] { 255, 0, 0, -1 });
        }
        public static LedFlashPattern CreateSolidGreen()
        {
            return new LedFlashPattern(new int[] { 0, 255, 0, -1 });
        }
        public static LedFlashPattern CreateSolidBlue()
        {
            return new LedFlashPattern(new int[] { 0, 0, 255, -1 });
        }
        public static LedFlashPattern CreateFlash1Second50percentRed50PercentBlack()
        {
            return new LedFlashPattern(new int[] {  255, 0, 0, 1000,
                                                    0, 0, 0, 1000, });
        }

        public static LedFlashPattern CreateFlash1Second50percentGreen50PercentBlack()
        {
            return new LedFlashPattern(new int[] {  0, 255, 0, 1000,
                0, 0, 0, 1000, });
        }
        public static LedFlashPattern CreateFlash1Second50percentGreen50PercentRed()
        {
            return new LedFlashPattern(new int[] {  0, 255, 0, 1000,
                255, 0, 0, 1000, });
        }

        public static LedFlashPattern CreateSolidRedWithSpecialIndication()
        {
            return new LedFlashPattern(new int[] {
                255, 0, 0, 480,
                0, 0, 0, 40, // Flickering
                255, 0, 0, 480,
                0, 0, 0, 1000, });
        }


        public static LedFlashPattern CreateSolidGreenWithSpecialIndication()
        {
            return new LedFlashPattern(new int[] {
                0, 255, 0, 480,
                0, 0, 0, 40, // Flickering
                0, 255, 0, 480,
                0, 0, 0, 1000, });
        }
        public static LedFlashPattern CreateFlash1Second50percentGreen50PercentRedWithSpecialIndication()
        {
            return new LedFlashPattern(new int[] {
                0, 255, 0, 480,
                0, 0, 0, 40, // Flickering
                0, 255, 0, 480,
                255, 0, 0, 480,
                0, 0, 0, 40, // Flickering
                255, 0, 0, 480,
            });
        }

        public static LedFlashPattern CreateFlash100MS50percentBlue50PercentBlack()
        {
            return new LedFlashPattern(new int[] {  0, 0, 255, 50,
                0, 0, 0, 50 });
        }
    }
}
