using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
