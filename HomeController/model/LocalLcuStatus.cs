using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class LocalLcuStatus : ILcuStatus
    {
        public bool IsDoorLocked { get; set; }
        public AlarmHandler.AlarmActivityStatus AlarmActivity { get; set; }
        public bool IsDoorOpen { get; set; }
        public override string ToString()
        {
            return "LocalLcuStatus: IsDoorOpen=" + IsDoorOpen + "; IsDoorLocked=" + IsDoorLocked + "AlarmActivity=" +
                   AlarmActivity;
        }
    }
}
