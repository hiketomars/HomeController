using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface IDoor
    {
        bool IsOpen { get; set; }
        bool IsLocked { get; set; }
        bool? IsFloating { get; }

        bool UseVirtualDoorClosedSignal { get; set; }
        bool UseVirtualDoorFloatingSignal { get; set; }
        bool UseVirtualDoorLockedSignal { get; set; }
        void UseVirtualIoSignals();
    }
}
