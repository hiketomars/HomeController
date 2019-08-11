using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;

namespace HomeController.model
{
    public interface ILocalCentralUnit
    {
        
        string Name { get; }
        IAlarmHandler LcuAlarmHandler { get; }
        IRemoteCentralUnitsController LcuRemoteCentralUnitsController { get; }

        void OnRcuReceivedMessage(IRemoteCentralUnitProxy rcu, Definition.MessageType messageType, string loggMessage); // Called by RCU:s when they receive a message.
    }
}
