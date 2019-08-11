using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;
using HomeController.utils;

namespace UnitTestProject
{
    class TestLcuHandler : ILcuHandler
    {
        public void OnRcuReceivedMessage(ILocalCentralUnit lcu, IRemoteCentralUnitProxy rcu,
            Definition.MessageType messageType, string loggMessage)
        {
        }
    }
}
