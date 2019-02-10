using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using HomeController.comm;

namespace HomeController.model
{
    public class Siren : GpioConnector, ISiren
    {
        private readonly int sirenPinNumber;
        private GpioPin sirenPin;

        private GpioController gpio;
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
        public sealed override void InitGpio()
        {
            base.InitGpio();
            sirenPin = gpio.OpenPin(sirenPinNumber); ;
        }

    }
}
