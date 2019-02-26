using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    // This class represents a remote local central unit, a remote central unit.
    public class RemoteCentralUnit : IRemoteCentralUnit
    {
        private readonly string name;
        private readonly string ipadress;
        private readonly string portNumber;

        public RemoteCentralUnit(string name, string ipAddress, string portNumber)
        {
            this.name = name;
            this.ipadress = ipAddress;
            this.portNumber = portNumber;
        }
    }
}
