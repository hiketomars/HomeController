using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    // This class is used to hold together the status for all the remote lcu:s and the local lcu as one combined (compound) status.
    public class CompoundStatus
    {
        public List<ILcuStatus> LcuStatuses;
        private readonly AlarmHandler.AlarmActivityStatus alarmStatus;

        //public AlarmHandler.AlarmActivityStatus AlarmStatus => alarmStatus;

        public CompoundStatus()
        {
            LcuStatuses = new List<ILcuStatus>();
        }

        // MostRecentChangedLcu is the RCU that has the RemoteLcuStatus with the latest StatusTime,
        // this is, the LCU that most recently has changed its alarm state. 
        public IRemoteCentralUnitProxy MostRecentChangedLcu { get; set; }



        public void AddLocalStatus(LocalLcuStatus localLcuStatus)
        {
            // Lets the local lcu status be in the same list as the remote lcu:s statuses.
            LcuStatuses.Add(localLcuStatus);
        }

        public AlarmHandler.AlarmActivityStatus GetNewestAlarmStatus()
        {
            DateTime newestAlarmStatus = new DateTime(1970, 1, 1, 0, 0, 0);

            foreach (var lcuStatuse in LcuStatuses)
            {
            }

            return AlarmHandler.AlarmActivityStatus.Undefined;
        }
    }
}
