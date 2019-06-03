using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture.Frames;
using Windows.System.Threading;
using Windows.UI.Core;
using HomeController.comm;
using Windows.UI.Xaml;
using HomeController.utils;

namespace HomeController.model
{
    public class AlarmHandler : IAlarmHandler
    {
        private readonly LocalCentralUnit lcu;
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

        public enum AlarmActivityStatus
        { Undefined, Off, Activating, Active, EntranceOngoing, Siren,
            SirenOff
        }

        private AlarmActivityStatus alarmActivityStatus;

        public AlarmActivityStatus CurrentLocalStatus
        {
            get { return alarmActivityStatus; }
            set
            {
                alarmActivityStatus = value;
                Logger.Logg(lcu.Name, Logger.LCU_Cat, "Status changed into " + alarmActivityStatus);
            }
        }

        private static AlarmHandler instance;

        //public static AlarmHandler GetInstance()
        //{
        //    if (instance == null)
        //    {
        //        instance = new AlarmHandler();
        //    }
        //    return instance;
        //}
    
        public AlarmHandler(LocalCentralUnit lcu)
        {
            this.lcu = lcu;
            CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Undefined;
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
            CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Off;

        }

        public int EntranceDelayMs { get; set; }

        public int SirenDuranceMs { get; set; } 

        private void ActivationPoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            Logger.Logg(lcu.Name, Logger.LCU_Cat, "Entering AH.ActivationPoolTimerElapsedHandler.");
            SetAlarmActive();
        }

        private void SetAlarmActive()
        {
            CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Active;

        }


        public void DeactivateAlarm()
        {
            Logger.Logg(lcu.Name, Logger.LCU_Cat, "Entering AH.DeactivateAlarm.");

            EntrancePoolTimer?.Cancel();
            SirenPoolTimer?.Cancel();

            IsEntrancePeriodOngoingLocally = false;
            //HasIntrusionOccurred = false;
            CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Off;

        }

        public void ActivateAlarm(int delayInMs)
        {
            Logger.Logg(lcu.Name, Logger.LCU_Cat, "Entering AH.ActivateAlarm.");

            //ActivationDelayMs = delayInMs;
            TimeSpan delay = new TimeSpan(0, 0, 0, 0, delayInMs);
            ActivationPoolTimer = ThreadPoolTimer.CreateTimer(ActivationPoolTimerElapsedHandler, delay);

            Logger.Logg(lcu.Name, Logger.LCU_Cat, "Timer start: ActivationPoolTimer.");

            //ActivationPoolTimer.Period = delay;
            //ActivationTimer.Start();
            CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Activating;
            Logger.Logg(lcu.Name, Logger.LCU_Cat, "Leaving AH.ActivateAlarm.");

        }

        //private void SetStatus(AlarmActivityStatus newStatus)
        //{
        //    //CurrentStatus = AlarmHandler.AlarmActivityStatus.Activating;
        //    CurrentStatus = newStatus;
        //    Logger.Logg(lcu.Name, Logger.LCU_Cat, "Status changed into "+CurrentStatus);

        //    /* Behöver inte tala om att status har ändrats. Remote LCU:er får fråga oss så svarar vi.
        //    // Tell other LCU:s about our new status.
        //    lcu.LcuRemoteCentralUnitsController.StatusHasChanged(CurrentStatus);
        //    */
        //}

        public bool IsAlarmActive
        {
            get { return CurrentLocalStatus == AlarmActivityStatus.Active; }
        }


        //private void SirenPoolTimerElapsedHandler(ThreadPoolTimer timer)
        //{
        //    SirenPoolTimer.Cancel();
        //    //SirenController.GetInstance().TurnOff();
        //    lcu.LcuSirenController.TurnOff();
        //    CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.SirenOff;

        //}

        private void EntrancePoolTimerElapsedHandler(ThreadPoolTimer timer)
        {
            IsEntrancePeriodOngoingLocally = false;
            EntrancePoolTimer.Cancel();
            if(IsAlarmActive)
            {
                // Intrusion! Turn on siren. 
                HasIntrusionOccurredLocally = true;
                //SirenController.GetInstance().TurnOn();
                lcu.LcuSirenController.TurnOn(SirenDuranceDefaultMs);
                //SirenPoolTimer = ThreadPoolTimer.CreateTimer(SirenPoolTimerElapsedHandler,
                //    TimeSpan.FromMilliseconds(SirenDuranceMs));
                CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.Siren;

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


        // Local intrusion.
        public bool HasIntrusionOccurredLocally { get; set; }

        public bool IsEntrancePeriodOngoingLocally { get; set; }

        public DateTime OverallAlarmStateTime { get; set; }
        public enum OverallAlarmState
        {
            AlarmDeactivated,
            AlarmActivating,
            AlarmActivated,
            Siren
        }

        public OverallAlarmState CurrentOverallAlarmState;


        // This method will set the status based on the local and remote conditions.
        public void CheckSituation()
        {
            if(lcu.LcuDoorController.IsDoorOpen())
            {
                if(IsAlarmActive /*&& CurrentStatus == AlarmActivityStatus.Active*/)
                {
                    IsEntrancePeriodOngoingLocally = true;
                    EntrancePoolTimer = ThreadPoolTimer.CreateTimer(EntrancePoolTimerElapsedHandler, TimeSpan.FromMilliseconds(EntranceDelayMs));
                    CurrentLocalStatus = AlarmHandler.AlarmActivityStatus.EntranceOngoing;

                    //EntranceTimer.Stop(); //todo kolla om det är en bra ide att stoppa den först i fall den redan var igång.
                    //EntranceTimer.Start();
                }
                else
                {
                    int a = 0;
                }
            }

            //todo Kanske inte bästa sättet att göra detta på; Ovan sätter jag i vissa situationer statusen till
            // ett nytt värde men nedan kommer jag kanske direkt sätta om det igen pga att en remote lcu har ett nyare status...

            var mostRecentRcuStatus = lcu.LatestCompoundStatus.MostRecentChangedLcu.RcuCurrentStatusMessage.ReceivedOverallAlarmState;
            switch (mostRecentRcuStatus)
            {
                case OverallAlarmState.AlarmDeactivated:
                    DeactivateAlarm();
                    CurrentOverallAlarmState = OverallAlarmState.AlarmDeactivated;
                    break;

                case OverallAlarmState.AlarmActivating:
                    ActivateAlarm(0);
                    CurrentOverallAlarmState = OverallAlarmState.AlarmActivating;
                    break;

                case OverallAlarmState.AlarmActivated:
                    SetAlarmActive();
                    CurrentOverallAlarmState = OverallAlarmState.AlarmActivated;
                    break;
                case OverallAlarmState.Siren:
                    CurrentOverallAlarmState = OverallAlarmState.Siren;

                    if(!lcu.LcuSirenController.IsOn)
                    {
                        lcu.LcuSirenController.TurnOn(SirenDuranceMs);
                    }

                    break;
                default:
                    break;

            }

            //if(lcu.LatestCompoundStatus.AlarmStatus == AlarmActivityStatus.Active)
            //{
            //    lcu.LcuAlarmHandler.SetAlarmActive();
            //}
            //else
            //{
            //    lcu.LcuSirenController.TurnOff();
            //}


            //if(totalStatus == AlarmActivityStatus.Siren)
            //{
            //    lcu.LcuSirenController.TurnOn();
            //}
            //else
            //{
            //    lcu.LcuSirenController.TurnOff();
            //}


            //if(lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred())
            //{
            //    //LcuRemoteCentralUnitsController.
            //    //SirenController.GetInstance().TurnOn();
            //    lcu.LcuSirenController.TurnOn();
            //}


        }
    }
}
