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
        IDoor Door { get; }
        string PortNumber { get; }
        IAlarmHandler LcuAlarmHandler { get; }
        IRemoteCentralUnitsController LcuRemoteCentralUnitsController { get; }
        bool UseAnyMockedDoorProperty { get; }
        bool UseVirtualDoorOpen { get; set; }
        bool UseVirtualDoorFloating { get; set; }
        bool UseVirtualDoorLocked { get; set; }
        bool? IsSabotaged { get; }

        void OnRcuReceivedMessage(IRemoteCentralUnitProxy rcu, Definition.MessageType messageType, string loggMessage); // Called by RCU:s when they receive a message.
        void Action(string rcuName, string action);
    }
}
