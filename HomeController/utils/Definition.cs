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
        public const string RemoteLcuPIAddress = "192.168.11.2";
        public const string OwnPortNumber = "1337";
        public const string RemoteLcuPortNumber = "1338";


        public delegate void LoggInGui(string text);

        public const string StandardDateTimeFormat = "yyyy-MM-dd HH.mm.ss";

        public delegate void VoidEventHandler();

        public delegate void LEDChangedEventHandler(RGBValue rgbValue);

        public delegate void RemoteLcuStatusChangedEventHandler(string todoType);

        public enum LEDGraphColor { Red, Green, Blue, Gray }
    }
}
