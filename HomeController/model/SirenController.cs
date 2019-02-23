using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class SirenController : ISirenController
    {
        public SirenController(ISiren siren)
        {
            Siren = siren;
        }

        public ISiren Siren { get; }

        public void TurnOn()
        {
            Siren.TurnOn();
        }

        public void TurnOff()
        {
            Siren.TurnOff();
        }

        public bool IsOn => Siren.IsOn();
        public void Reset()
        {
            TurnOff();
        }
    }
}
