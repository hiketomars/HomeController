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
    /// <summary>
    /// Represents a siren in the house and can be either on or off.
    /// Values are normally written to electronics via connection to the GPIO-pin.
    /// Can also be set to use virtual values (ie software values) instead for the on/off-property.
    /// </summary>
    public class Siren : GpioConnector, ISiren
    {
        public bool UseVirtualSirenSignal { get; set; }

        private bool VirtualSirenOn { get; set; }
        private readonly int sirenPinNumber;
        private GpioPin sirenPin;
        private GpioController gpio;

        public void TurnOn()
        {
            VirtualSirenOn = true;
            if(!UseVirtualSirenSignal)
            {
                sirenPin.Write(GpioPinValue.High);
            }
        }

        public void TurnOff()
        {
            VirtualSirenOn = false;
            if(!UseVirtualSirenSignal)
            {
                sirenPin.Write(GpioPinValue.Low);
            }
        }

        public bool IsOn()
        {
            if (UseVirtualSirenSignal)
            {
                return VirtualSirenOn;
            }
            return sirenPin.Read() == GpioPinValue.High;
        }

        public sealed override void InitGpio()
        {
            base.InitGpio();
            sirenPin = gpio.OpenPin(sirenPinNumber); ;
        }


    }
}
