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
        Task<string> SendCommand(string hostName, string command);
        bool HasIntrusionOccurred();
        bool HasIntrusionOccurredRemotely();
        bool IsDoorUnlocked();
        void SendStartUpMessage();
        object RemoteLcuStatusHasChanged { get; set; }
        void ActivateCommunication();
        string Name { set; get; }
        string IpAddress { get; set; }
        string PortNumber { get; set; }
        bool SendPingMessage();
        void SendStateChangedMessage(AlarmHandler.AlarmActivityStatus currentStatus);

    }
}
