using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class RemoteCentralUnitConfiguration : IRemoteCentralUnitConfiguration
    {
        public RemoteCentralUnitConfiguration(string name, string id, string ip, string portNumber)
        {
            Name = name;
            Id = int.Parse(id);
            IpAddress = ip;
            PortNumber = portNumber;
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public string IpAddress { get; set; }

        public string PortNumber { get; set; }


    }
}
