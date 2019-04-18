using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.comm
{
    public class CommunicationResponse
    {
        public string Id { get; set; }
        public string AckedMessaged { get; set; }

        public string GetAckResponse()
        {
            return RemoteCentralUnitProxy.MessageACK
                   + RemoteMessage.MessagPartsDelimeter;
        }
    }

    public class PingResponse : CommunicationResponse
    {

    }
}
