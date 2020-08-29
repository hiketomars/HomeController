using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;

namespace HomeController.model
{
    // Interface for a remote central unit.
    // All calls are synchronous and implementations should respond immediately.
    public interface IRemoteCentralUnitProxy
    {
        // -----------------------------------------------------
        // Requests for alarm specific status:
        // -----------------------------------------------------
        bool HasIntrusionOccurred();
        bool HasIntrusionOccurredRemotely();
        bool? IsDoorUnlocked();
        CurrentStatusMessage GetRcuCurrentStatusMessage { get; set; }

        // -----------------------------------------------------
        // Request for other information:
        // -----------------------------------------------------
        string NameOfRemoteLcu { set; get; }


        // -----------------------------------------------------
        // Sending of information:
        // -----------------------------------------------------
        



        // -----------------------------------------------------
        // Calls that should be moved to other interfaces (lower levels in some cases).
        // -----------------------------------------------------
        bool SendPingMessage();
        void RequestStatusFromRcu();

        void QueueIncomingMessage(StatusBaseMessage statusBaseMessage);
        Task<string> SendCommandSpecific(string hostName, string command);
        void SendStartUpMessage();
        object RemoteLcuStatusHasChanged { get; set; }
        void ActivateCommunication();
        //string Name { set; get; }
        string IpAddress { get; set; }
        //void SendRequestOfRcuStatusMessage();

        // For debugging.
        //void StartListeningToRemoteLcu();
        void ConnectToRcu();
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
