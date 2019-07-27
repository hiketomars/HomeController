using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class RemoteCentralUnitConfiguration : IRemoteCentralUnitConfiguration
    {
        // initiatorPortNumber is the port that the LCU will use to write to the RCU.
        // responderPortNumber is the port that the LCU will listen to for incoming messages from the RCU.
        public RemoteCentralUnitConfiguration(string name, string id, string ip, string initiatorPortNumber, string responderPortNumber)
        {
            Name = name;
            Id = int.Parse(id);
            IpAddress = ip;
            InitiatorPortNumber = initiatorPortNumber;
            ResponderPortNumber = responderPortNumber;
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public string IpAddress { get; set; }

        public string InitiatorPortNumber { get; set; }

        public string ResponderPortNumber { get; set; }
        //public string ActingPortNumber { get; set; }
        //public string ReactingPortNumber { get; set; }

    }
}
