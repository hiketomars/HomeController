using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    public interface ILEDController
    {
        IRgbLed ControlledRgbLed { get; }
        void SetTotalColor(RGBValue green);
        RGBValue GetLedColor();
        void PerformStartUpLedFlash();
    }
}
