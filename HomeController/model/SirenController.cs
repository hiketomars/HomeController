using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using HomeController.comm;

namespace HomeController.model
{
    public class SirenController : ISirenController
    {
        public LocalCentralUnit lcu;

        public SirenController(LocalCentralUnit lcu)
        {
            this.lcu = lcu;
        }
        private ThreadPoolTimer SirenPoolTimer;

        //public void SetSiren(ISiren siren)
        //{
        //    Siren = siren;
        //}

        public ISiren Siren { get; set; }

        public void TurnOn(int sirenDuranceMs)
        {
            SirenPoolTimer = ThreadPoolTimer.CreateTimer(SirenPoolTimerElapsedHandler,
                TimeSpan.FromMilliseconds(sirenDuranceMs));

            Siren.TurnOn();
        }

        public void TurnOn()
        {
            TurnOn(AlarmHandler.SirenDuranceDefaultMs);
        }

        private void SirenPoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            SirenPoolTimer.Cancel();
            //SirenController.GetInstance().TurnOff();
            lcu.LcuSirenController.TurnOff();
            lcu.LcuAlarmHandler.CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.SirenOff;

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
