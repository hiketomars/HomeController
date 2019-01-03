﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.view
{
    public interface IMainView : IView
    {
        void Logg(string text);
        void SetLoggingItems(List<string> loggings);
        void SetColorForBackdoorLED(Definition.LEDGraphColor color);
    }
}