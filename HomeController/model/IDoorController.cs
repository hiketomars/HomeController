using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface IDoorController
    {
        IDoor Door { get; set; }
        bool IsDoorOpen();
        bool IsDoorLocked();
        void Reset();
    }
}
