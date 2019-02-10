using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;
using Windows.UI.Xaml;

namespace HomeController.model
{
    public class AlarmHandler
    {
        public const int EntranceDelayMs = 6000;
        public const int SirenDuranceMs = 5 * 60 * 1000;
        public const int ActivationDelayDefaultMs = 30 * 1000;

        public int ActivationDelayMs;

        private DispatcherTimer EntranceTimer;
        private DispatcherTimer SirenTimer;
        private DispatcherTimer ActivationTimer;
        private LocalCentralUnit lcu;

        private enum AlarmActivityStatus
        { Off, Activating, Active, EntranceOngoing, Siren
        }

        public AlarmHandler(LocalCentralUnit lcu)
        {
            this.lcu = lcu;
            EntranceTimer = new DispatcherTimer();
            EntranceTimer.Interval = TimeSpan.FromMilliseconds(EntranceDelayMs);
            EntranceTimer.Tick += EntranceTimer_Tick;

            //Task t3 = Task.Run(() => { WaitAndCheck(); });

            SirenTimer = new DispatcherTimer();
            SirenTimer.Interval = TimeSpan.FromMilliseconds(SirenDuranceMs);
            SirenTimer.Tick += SirenDuranceTimer_Tick;

            ActivationTimer = new DispatcherTimer();
            ActivationTimer.Interval = TimeSpan.FromMilliseconds(ActivationDelayMs);
            ActivationTimer.Tick += ActivationTimer_Tick;

        }

        private void WaitAndCheck()
        {
            Task.Delay(1).Wait();

        }

        private void ActivationTimer_Tick(object sender, object e)
        {
            IsAlarmActive = true;
        }

        internal void DeactivateAlarm()
        {
            EntranceTimer.Stop();
            SirenTimer.Stop();

            IsEntrancePeriodOngoing = false;
            IsIntrusionOngoing = false;
            IsAlarmActive = false;
        }

        internal void ActivateAlarm(int delayInMs)
        {
            ActivationDelayMs = delayInMs;
            TimeSpan delay = new TimeSpan(0, 0, 0, 0, delayInMs);
            ActivationTimer.Interval = delay;
            ActivationTimer.Start();
        }

        public bool IsAlarmActive { get; private set; }


        private void SirenDuranceTimer_Tick(object sender, object e)
        {
            SirenTimer.Stop();
            lcu.LcuSiren.TurnOff();
        }

        private void EntranceTimer_Tick(object sender, object e)
        {
            IsEntrancePeriodOngoing = false;
            EntranceTimer.Stop();
            if (IsAlarmActive)
            {
                // Intrusion! Turn on siren. 
                IsIntrusionOngoing = true;
                lcu.LcuSiren.TurnOn();
            }
        }


        public bool IsIntrusionOngoing { get; set; }
        public bool IsEntrancePeriodOngoing { get; set; }

        public void CheckForEntrance()
        {
            if(lcu.DoorController.IsDoorOpen())
            {
                if(IsAlarmActive)
                {
                    IsEntrancePeriodOngoing = true;
                    EntranceTimer.Stop(); //todo kolla om det är en bra ide att stoppa den först i fall den redan var igång.
                    EntranceTimer.Start();
                }
            }
        }
    }
}
