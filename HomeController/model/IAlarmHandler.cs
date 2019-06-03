using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface IAlarmHandler
    {
        void CheckSituation();
        void ActivateAlarm(int delayInMs);
        AlarmHandler.AlarmActivityStatus CurrentLocalStatus { get; set; }
        bool HasIntrusionOccurredLocally { get; set; }
        bool IsAlarmActive { get; }
        int EntranceDelayMs { get; set; }
        void DeactivateAlarm();
    }
}
