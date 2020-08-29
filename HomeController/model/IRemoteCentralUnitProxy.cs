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
        void QueueIncomingMessage(StatusBaseMessage statusBaseMessage);
        Task<string> SendCommandSpecific(string hostName, string command);
        bool HasIntrusionOccurred();
        bool HasIntrusionOccurredRemotely();
        bool? IsDoorUnlocked();
        void SendStartUpMessage();
        object RemoteLcuStatusHasChanged { get; set; }
        void ActivateCommunication();
        //string Name { set; get; }
        string NameOfRemoteLcu { set; get; }
        string IpAddress { get; set; }
        CurrentStatusMessage RcuCurrentStatusMessage { get; set; }
        bool SendPingMessage();
        //void SendRequestOfRcuStatusMessage();

        // For debugging.
        //void StartListeningToRemoteLcu();
        void ConnectToRcu();
        void RequestStatusFromRcu();
        void HandleMessageFromQueue(StatusBaseMessage statusBaseMessage);
        void HandleFirstMessageInQueue();
        void Action(string action);
    }

    public interface IRemoteCentralUnitConfiguration
    {
        string Name { set; get; }
        int Id { set; get; }
        string IpAddress { get; set; }

        string PortNumber { get; set; }

    }
}
