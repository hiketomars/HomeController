using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace HomeController.model
{
    public class RemoteLcuResponse
    {
        public bool Response { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string ResponderName { get; set; }
    }
}
