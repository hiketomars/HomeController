using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;
using HomeController.utils;

namespace HomeController.view
{
    /// <summary>
    /// This is a View interface in the MVP-architecture.
    /// </summary>
    public interface IMainView : IView
    {
        void AddHouseLoggText(string text);
        void SetLcuInfoText(string lcuName, string text);
        void SetColorForBackdoorLED(RGBValue rgbValue);
        void SetLcus(List<ILocalCentralUnit> lcus);

        void SetLcuLoggText(string lcuName, string text);
        void AddLcuLoggText(string lcuName, string text);

        void AddRcuLoggText(string lcuName, string rcuName, string text);
        void AddRcuSendCounterText(string lcuName, string rcuName, string text);
        void AddRcuReceiveCounterText(string lcuName, string rcuName, string text);
        void ClearRcuText(string lcuName, string rcuName);
    }
}
