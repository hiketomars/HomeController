using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class RemoteLcuStatus
    {
        public bool HasIntrusionOccurred { get; set; }
        public bool IsDoorUnlocked { get; set; }

        // This property specifies if the remote control unit has received intrusion from another remote control unit.
        public bool HasIntrusionOccurredRemotely { get; set; }

    }
}
