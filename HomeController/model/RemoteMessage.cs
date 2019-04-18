using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;

namespace HomeController.model
{
    // A message looks like this for example:
    // "Msg;3412413414515;msgStateChanged;4;
    // Interpretation;
    // This is a message with id 3412413414515 and is about that the sending lcu has changed its state into EntranceOngoing
    public class RemoteMessage : ITransferObject
    {
        public const string MessageStartToken = "Msg";
        public const string MessagPartsDelimeter = ";";
        public const string MessageCurrentStatus = "msgCurrentStatus";


        public string Message { get; set; }
        public string Id { get; set; }

        public string TotalMessage => MessageStartToken
                                      + MessagPartsDelimeter
                                      + Id
                                      + MessagPartsDelimeter
                                      + Message
                                      + MessagPartsDelimeter;

        // Incoming status from a RCU.
        public static RemoteLcuStatus InterpretCurrentStatusMessage(string message, string id, List<string> parameters)
        {
            return new RemoteLcuStatus()
            {
                Message = message,
                Id = id,
                HasIntrusionOccurred = bool.Parse(parameters[0]),
                HasIntrusionOccurredRemotely = bool.Parse(parameters[1]),
                IsDoorLocked = bool.Parse(parameters[2]),
            };
        }

        // Our own outgoing status. 
        public static string CreateCurrentStatusMessage(object source)
        {
            string currentStatusCommand = MessageStartToken
                                          + MessagPartsDelimeter
                                          + MessageCurrentStatus
                                          + MessagPartsDelimeter
                                          + LocalCentralUnit.GetInstance().LcuAlarmHandler.HasIntrusionOccurred
                                          + MessagPartsDelimeter
                                          + LocalCentralUnit.GetInstance().LcuRemoteCentralUnitsController
                                              .HasIntrusionOccurred()
                                          + MessagPartsDelimeter
                                          + LocalCentralUnit.GetInstance().LcuDoorController.IsDoorLocked();



            return currentStatusCommand;
        }
    }


}
