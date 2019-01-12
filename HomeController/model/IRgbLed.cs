using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface IRgbLed
    {
        void Red(bool offOn);
        void Green(bool offOn);
        void Blue(bool offOn);
        void AllColors(bool offOn);
    }
}
