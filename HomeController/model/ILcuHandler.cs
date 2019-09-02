using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;
using HomeController.utils;

namespace HomeController.model
{
    public interface ILcuHandler
    {
        void OnRcuReceivedMessage(ILocalCentralUnit lcu, IRemoteCentralUnitProxy rcu, Definition.MessageType messageType, string loggMessage);
        void OnLcuRelatedMessage(LocalCentralUnit localCentralUnit, Definition.MessageType logg, string message);
    }
}
