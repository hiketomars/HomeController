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
        void AddLoggItem(string text);
        void SetLoggItems(List<string> loggings);
        void SetColorForBackdoorLED(RGBValue rgbValue);
        void SetLcus(List<ILocalCentralUnit> lcus);
        void AddLoggText(string lcuName, string text);
        void AddLoggText(string lcuName, string rcuName, string text);
    }
}
