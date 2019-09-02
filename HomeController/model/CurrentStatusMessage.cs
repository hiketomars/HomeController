using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    // This class represents the information to send to an RCU when we want to share our current status.
    public class CurrentStatusMessage : StatusBaseMessage
    {
        private readonly AlarmHandler.AlarmActivityStatus alarmActivityStatus;

        public CurrentStatusMessage(AlarmHandler.AlarmActivityStatus alarmActivityStatus, string sendingLcuName) : base(sendingLcuName)
        {
            this.alarmActivityStatus = alarmActivityStatus;
        }

        public override string MessageType
        {
            get { return RemoteCentralUnitsController.MessageCurrentStatus; }
        }



        // 190529
        public override string CompleteMessageStringToSend
        {
            get
            {
                var completeMessage = RemoteCentralUnitsController.MessageStartToken // [0]
                                + RemoteCentralUnitsController.MessagPartsDelimeter
                                + Id // [1]
                                + RemoteCentralUnitsController.MessagPartsDelimeter
                                + SendingLcuName // [2]
                                + RemoteCentralUnitsController.MessagPartsDelimeter
                                + MessageType // [3]
                                + RemoteCentralUnitsController.MessagPartsDelimeter
                                + alarmActivityStatus.ToString() // [4]
                                + RemoteCentralUnitsController.MessagPartsDelimeter;

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
        public override StatusBaseMessage Clone()
        {
            var clone = new CurrentStatusMessage(alarmActivityStatus, SendingLcuName);
            return clone;
        }
    }




    // Example 1:
    // "Msg;3412413414515;frontDoor;msgGetStatus;
    // Interpretation;
    // This is a message with id 3412413414515 and is about that the sending lcu, frontDoor, wants to know the status of the RCU.
    //
    // Example 2:
    // "Msg;3412413414516;backDoor;msgCurrentStatus;Off;
    // Interpretation;
    // This is a message with id 3412413414516 and is the alarm status of the RCU with the name backDoor. 2 is an enum value meaning 'Activating'.
    // 190530
    public abstract class StatusBaseMessage : ITransferObject
    {
        protected StatusBaseMessage(string sendingLcuName)
        {
            SendingLcuName = sendingLcuName;
            Id = GetNewMessageId();
        }
        public string SendingLcuName { get; }

        public string Id { get; set; }

        public abstract string MessageType { get; }

        public static string GetNewMessageId()
        {
            //return DateTime.Now.Ticks.ToString();
            string longId = DateTime.Now.Ticks.ToString();
            const int diseredIdLength = 4; // Using short Id:s during development for easier log readings, eg 4.
            return longId.Substring(longId.Length - diseredIdLength);
        }

        public abstract string CompleteMessageStringToSend { get; }
        public abstract StatusBaseMessage Clone();
    }
}
