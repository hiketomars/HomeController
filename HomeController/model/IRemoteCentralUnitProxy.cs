using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;

namespace HomeController.model
{
    public interface IRemoteCentralUnitProxy
    {
        Task<string> SendCommandSpecific(string hostName, string command);
        bool HasIntrusionOccurred();
        bool HasIntrusionOccurredRemotely();
        bool IsDoorUnlocked();
        void SendStartUpMessage();
        object RemoteLcuStatusHasChanged { get; set; }
        void ActivateCommunication();
        string Name { set; get; }
        string IpAddress { get; set; }
        //string PortNumber { get; set; }
        CurrentStatusMessage RcuCurrentStatusMessage { get; set; }
        bool SendPingMessage();
        void SendRequestRcuStatusMessage();
        //void SendCurrentStatusMessage(AlarmHandler.AlarmActivityStatus currentStatus);
        void StartReceiver();
        //void InformAboutNewAlarmStatus();
    }

    public interface IRemoteCentralUnitConfiguration
    {
        string Name { set; get; }
        int Id { set; get; }
        string IpAddress { get; set; }

        string InitiatorPortNumber { get; set; }

        string ResponderPortNumber { get; set; }
        //string ActingPortNumber { get; set; } // Used for acting part when sending etc.
        //string ReactingPortNumber { get; set; } // Used for reacting part when receiving etc.

    }
}
