using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    // This class represents our request for the current status from an RCU.
    // The other RCU will typically respond with the ITransferObject CurrentStatusMessage.
    // However such a response is a new message of its own and not a direct response on the same session.
    // The port is also typically different.
    public class RequestRcuStatusMessage : ITransferObject
    {
        public string MessageType { get; set; } // Not used
        public string Id { get; set; }
        public string CompleteMessageStringToSend { get; }
        public RequestRcuStatusMessage(string id)
        {
            Id = id;
            CompleteMessageStringToSend = RemoteCentralUnitProxy.MessageStartToken // [0]
                                          + RemoteCentralUnitProxy.MessagPartsDelimeter
                                          + id // [1]
                                          + RemoteCentralUnitProxy.MessagPartsDelimeter
                                          + RemoteCentralUnitProxy.MessageGetStatus // [2]
                                          + RemoteCentralUnitProxy.MessagPartsDelimeter;


        }

        public string ToString()
        {
            return CompleteMessageStringToSend;
        }
    }

    // todo move to own file.
    // A messages that does not have the correct syntax will be take wrapped by this class.
    public class UnknownMessage : ITransferObject
    {
        public UnknownMessage(string completeString)
        {
            CompleteMessageStringToSend = completeString;
        }
        public string MessageType
        {
            get => RemoteCentralUnitProxy.MessageUnknown;
        }
        public string Id { get; set; }
        public string CompleteMessageStringToSend { get; }
    }
}
