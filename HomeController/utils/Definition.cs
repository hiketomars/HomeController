using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.utils
{
    // Holds some global definitions. Perhaps "Utils" and global definitions "BOD" should be kept separate...
    public class Definition
    {
        public delegate void LoggInGui(string text);
        public const string StandardDateTimeFormat = "yyyy-MM-dd HH.mm.ss";
        public delegate void VoidEventHandler();
        public delegate void LEDChangedEventHandler(RGBValue rgbValue);
        public enum LEDGraphColor { Red, Green, Blue, Gray }
        public const string PortNumber = "1337";
    }
}
