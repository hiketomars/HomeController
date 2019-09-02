using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    // This class represents our request for the current status from an RCU.
    // The other RCU will typically respond with the ITransferObject CurrentStatusMessage.
    // However such a response is a new message of its own and not a direct response on the same session.
    // The port is also typically different.
    public class RequestRcuStatusMessage : StatusBaseMessage
    {
        public override string MessageType { get; } // Not used
        public string Id { get; set; }
        public override StatusBaseMessage Clone()
        {
            var clone = new RequestRcuStatusMessage(Id, SendingLcuName);
            return clone;
        }

        public override string CompleteMessageStringToSend { get; }
        public RequestRcuStatusMessage(string id, string sendingLcuName) :base(sendingLcuName)
        {
            Id = id;
            CompleteMessageStringToSend = RemoteCentralUnitsController.MessageStartToken // [0]
                                          + RemoteCentralUnitsController.MessagPartsDelimeter
                                          + id // [1]
                                          + RemoteCentralUnitsController.MessagPartsDelimeter
                                          + SendingLcuName // [2]
                                          + RemoteCentralUnitsController.MessagPartsDelimeter
                                          + RemoteCentralUnitsController.MessageGetStatus // [3]
                                          + RemoteCentralUnitsController.MessagPartsDelimeter;


        }



        public string ToString()
        {
            return CompleteMessageStringToSend;
        }
    }

    // todo move to own file.
    // A messages that does not have the correct syntax will be take wrapped by this class.
    public class UnknownMessage : StatusBaseMessage
    {
        public UnknownMessage(string completeString, string sendingLcuName):base(sendingLcuName)
        {
            CompleteMessageStringToSend = completeString;
        }
        public override string MessageType
        {
            get => RemoteCentralUnitsController.MessageUnknown;
        }
        public string Id { get; set; }
        public override StatusBaseMessage Clone()
        {
            var clone = new UnknownMessage(CompleteMessageStringToSend, SendingLcuName);
            return clone;
        }

        public override string CompleteMessageStringToSend { get; }
    }
}
