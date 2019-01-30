using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class Siren : ISiren
    {
        public void TurnOn()
        {
            // todo set pin to activate siren.
            isOn = true;
        }

        public void TurnOff()
        {
            // todo set pin to deactivate siren.
            isOn = false;
        }

        public bool IsOn()
        {
            // todo read the pin that activates the siren.
            return isOn;
        }

        private bool isOn;
    }
}
