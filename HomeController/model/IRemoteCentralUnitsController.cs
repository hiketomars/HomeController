using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;

namespace HomeController.model
{
    public interface IRemoteCentralUnitsController
    {
        // Checks if intrusion has occurred in any of the remote central units.
        bool HasIntrusionOccurred();
        bool IsAnyRemoteDoorUnlocked();
        void SendStartUpMessage();
        void Setup(LocalCentralUnit localCentralUnit);
        bool VerifyContact();
        //void StatusHasChanged(AlarmHandler.AlarmActivityStatus currentStatus);
        int RemoteCentralUnitsCount { get; }
        LocalCentralUnit Lcu { get; set; }
        void ActivateCommunicationOnAllProxys();
        CompoundStatus GetCompoundStatus();
        List<IRemoteCentralUnitProxy> RcuList { get; }
        // FOr Debug puspose
        void ConnectToOnlyRcu();
        void ListenToRcu(string rcuName);

        void ListenToAllRcus();
        void ConnectToAllRcus();
    }
}
