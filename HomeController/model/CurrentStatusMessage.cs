using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    // This class represents the information to send to an RCU when we want to share our current status.
    // Example "Msg;3412413414515;msgCurrentStatus;2;" 
    public class CurrentStatusMessage : ITransferObject
    {
        private readonly AlarmHandler.AlarmActivityStatus alarmActivityStatus;

        public CurrentStatusMessage(AlarmHandler.AlarmActivityStatus alarmActivityStatus)
        {
            this.alarmActivityStatus = alarmActivityStatus;
        }

        public string MessageType
        {
            get { return RemoteCentralUnitProxy.MessageCurrentStatus; }
        }

        public string Id { get; set; }

        // 190529
        public string CompleteMessageStringToSend
        {
            get
            {
                var completeMessage = RemoteCentralUnitProxy.MessageStartToken // [0]
                                + RemoteCentralUnitProxy.MessagPartsDelimeter
                                + Id // [1]
                                + RemoteCentralUnitProxy.MessagPartsDelimeter
                                + MessageType // [2]
                                + RemoteCentralUnitProxy.MessagPartsDelimeter
                                + int.Parse(alarmActivityStatus.ToString()) // [3]
                                + RemoteCentralUnitProxy.MessagPartsDelimeter;

                return completeMessage;
            }
        }

        public AlarmHandler.AlarmActivityStatus AlarmStatus { get; set; }
        public AlarmHandler.OverallAlarmState ReceivedOverallAlarmState { get; set; }
        public bool HasIntrusionOccurred { get; set; }

        // This property specifies if the remote control unit has received intrusion from another remote control unit.
        public bool HasIntrusionOccurredRemotely { get; set; }
        public bool IsDoorLocked { get; set; }
        public DateTime StatusTime { get; set; }
    }
}
