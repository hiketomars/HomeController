using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class RemoteLcuStatus : ITransferObject
    {
        public bool HasIntrusionOccurred { get; set; }
        public bool IsDoorLocked { get; set; }

        // This property specifies if the remote control unit has received intrusion from another remote control unit.
        public bool HasIntrusionOccurredRemotely { get; set; }

        public string Message { get; set; }
        public string Id { get; set; }
    }
}
