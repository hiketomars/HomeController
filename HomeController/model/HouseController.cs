using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class HouseController
    {
        public bool AlarmIsActive { get; set; }
        public bool AlarmIsAlarming { get; set; }

        internal void StartEntrance(Door door)
        {
            throw new NotImplementedException();
        }

        internal void RegisterEntrance(Door door)
        {
            throw new NotImplementedException();
        }
    }
}
