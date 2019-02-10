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
        public DoorController()
        {
                        
        }
        public IDoor Door { get; set; }

        public bool IsDoorOpen() 
        {
            return Door.IsOpen; 
        }
    }
}
