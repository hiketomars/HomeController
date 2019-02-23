using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface ISirenController
    {
        void TurnOn();
        void TurnOff();
        bool IsOn { get; }
        void Reset();
    }
}
