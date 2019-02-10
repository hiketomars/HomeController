using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace HomeController.comm
{
    public class GpioConnector
    {
        private GpioController gpioController;

        public virtual void InitGpio()
        {
            gpioController = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpioController == null)
            {
                //GpioStatus.Text = "There is no GPIO controller on this device.";
                throw new Exception("There is no GPIO controller on this device.");
            }
        }
    }
}
