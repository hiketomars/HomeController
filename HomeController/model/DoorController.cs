using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace HomeController.model
{
    public class DoorController : IDoorController
    {
        public IDoor Door { get; set; }

        public bool IsDoorOpen
        {
            get { return Door.IsOpen; }
        }
    }
}
