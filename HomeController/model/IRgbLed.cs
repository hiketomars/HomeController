using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    public interface IRgbLed
    {
        void Red(bool offOn);
        void Green(bool offOn);
        void Blue(bool offOn);
        void AllColors(bool offOn);
        RGBValue GetColor();        
        void SetRGBValue(RGBValue rgbValue);

        // Modernare alternativ??? event Action<RGBValue> LEDHasChanged;
        event Definition.LEDChangedEventHandler LEDHasChanged;
    }
}
