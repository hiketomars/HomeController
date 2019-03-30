using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class SirenController : ISirenController
    {
        public SirenController()
        {
        }

        //public void SetSiren(ISiren siren)
        //{
        //    Siren = siren;
        //}

        public ISiren Siren { get; set; }

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

        //private static ISirenController instance;
        //public static ISirenController GetInstance()
        //{
        //    if (instance == null)
        //    {
        //        instance = new SirenController();
        //    }

        //    return instance;
        //}

        //public static void SetInstance(ISirenController sirenController)
        //{
        //    instance = sirenController;
        //}
    }
}
