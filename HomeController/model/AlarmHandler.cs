using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using HomeController.comm;
using Windows.UI.Xaml;

namespace HomeController.model
{
    public class AlarmHandler
    {
        public const int EntranceDelayDefaultMs = 6000;
        public const int SirenDuranceDefaultMs = 5 * 60 * 1000;
        public const int ActivationDelayDefaultMs = 30 * 1000;

        //public int ActivationDelayMs;

        //private DispatcherTimer EntranceTimer;
        private ThreadPoolTimer EntrancePoolTimer;
        //private DispatcherTimer SirenTimer;
        private ThreadPoolTimer SirenPoolTimer;
        //private DispatcherTimer ActivationTimer;
        private ThreadPoolTimer ActivationPoolTimer;
        private LocalCentralUnit lcu;

        public enum AlarmActivityStatus
        { Undefined, Off, Activating, Active, EntranceOngoing, Siren,
            SirenOff
        }

        public AlarmActivityStatus CurrentStatus { get; set; }
        public AlarmHandler(LocalCentralUnit lcu)
        {
            CurrentStatus = AlarmActivityStatus.Undefined;
            this.lcu = lcu;
            //EntranceTimer = new DispatcherTimer();
            //EntranceTimer.Interval = TimeSpan.FromMilliseconds(EntranceDelayMs);
            //EntranceTimer.Tick += EntranceTimer_Tick;

            //Task t3 = Task.Run(() => { WaitAndCheck(); });


            //SirenTimer = new DispatcherTimer();
            //SirenTimer.Interval = TimeSpan.FromMilliseconds(SirenDuranceMs);
            //SirenTimer.Tick += SirenDuranceTimer_Tick;

            //ActivationTimer = new DispatcherTimer();
            //ActivationTimer.Interval = TimeSpan.FromMilliseconds(ActivationDelayMs);
            //ActivationTimer.Tick += ActivationTimer_Tick;

            EntranceDelayMs = EntranceDelayDefaultMs;
            SirenDuranceMs = SirenDuranceDefaultMs;
            CurrentStatus = AlarmActivityStatus.Off;

        }

        public int EntranceDelayMs { get; set; }

        public int SirenDuranceMs { get; set; } 
        //private void WaitAndCheck()
        //{
        //    Task.Delay(1).Wait();

        //}

        //private void ActivationTimer_Tick(object sender, object e)
        //{
        //    IsAlarmActive = true;
        //}

        private void ActivationPoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            IsAlarmActive = true;
            CurrentStatus = AlarmActivityStatus.Active;
        }


        internal void DeactivateAlarm()
        {
            EntrancePoolTimer?.Cancel();
            SirenPoolTimer?.Cancel();

            IsEntrancePeriodOngoing = false;
            //HasIntrusionOccurred = false;
            IsAlarmActive = false;
            CurrentStatus = AlarmActivityStatus.Off;

        }

        internal void ActivateAlarm(int delayInMs)
        {
            //ActivationDelayMs = delayInMs;
            TimeSpan delay = new TimeSpan(0, 0, 0, 0, delayInMs);
            ActivationPoolTimer = ThreadPoolTimer.CreateTimer(ActivationPoolTimerElapsedHandler, delay);

            //ActivationPoolTimer.Period = delay;
            //ActivationTimer.Start();
            CurrentStatus = AlarmActivityStatus.Activating;

        }

        public bool IsAlarmActive { get; private set; }


        //private void SirenDuranceTimer_Tick(object sender, object e)
        //{
        //    SirenPoolTimer.Cancel();
        //    lcu.LcuSiren.TurnOff();
        //}

        private void SirenPoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            SirenPoolTimer.Cancel();
            lcu.LcuSirenController.TurnOff();
            CurrentStatus = AlarmActivityStatus.SirenOff;

        }


        private void EntrancePoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            IsEntrancePeriodOngoing = false;
            EntrancePoolTimer.Cancel();
            if(IsAlarmActive)
            {
                // Intrusion! Turn on siren. 
                HasIntrusionOccurred = true;
                lcu.LcuSirenController.TurnOn();
                SirenPoolTimer = ThreadPoolTimer.CreateTimer(SirenPoolTimerElapsedHandler,
                    TimeSpan.FromMilliseconds(SirenDuranceMs));
                CurrentStatus = AlarmActivityStatus.Siren;

            }
        }

        //private void EntranceTimer_Tick(object sender, object e)
        //{
        //    IsEntrancePeriodOngoing = false;
        //    EntranceTimer.Stop();
        //    if (IsAlarmActive)
        //    {
        //        // Intrusion! Turn on siren. 
        //        IsIntrusionOngoing = true;
        //        lcu.LcuSiren.TurnOn();
        //    }
        //}


        public bool HasIntrusionOccurred { get; set; }
        public bool IsEntrancePeriodOngoing { get; set; }

        public void CheckSituation()
        {
            if(lcu.DoorController.IsDoorOpen())
            {
                if(IsAlarmActive && CurrentStatus == AlarmActivityStatus.Active)
                {
                    IsEntrancePeriodOngoing = true;
                    EntrancePoolTimer = ThreadPoolTimer.CreateTimer(EntrancePoolTimerElapsedHandler, TimeSpan.FromMilliseconds(EntranceDelayMs));
                    CurrentStatus = AlarmActivityStatus.EntranceOngoing;

                    //EntranceTimer.Stop(); //todo kolla om det är en bra ide att stoppa den först i fall den redan var igång.
                    //EntranceTimer.Start();
                }
            }

            if (lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred())
            {
                //LcuRemoteCentralUnitsController.
                lcu.LcuSirenController.TurnOn();
            }
        }
    }
}
