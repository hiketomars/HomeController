using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface IRemoteCentralUnitsController
    {
        // Checks if intrusion has occurred in any of the remote central units.
        bool HasIntrusionOccurred();
        bool IsAnyRemoteDoorUnlocked();
    }
}
