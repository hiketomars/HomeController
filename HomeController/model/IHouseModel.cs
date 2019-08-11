using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.utils;
namespace HomeController.model
{
    /// <summary>
    /// The model interface in the MVP-architecture.
    /// </summary>
    public interface IHouseModel
    {
        event Definition.VoidEventHandler ModelHasChanged;
        event Definition.VoidEventHandler LcuInstancesHasChanged;
        event Definition.LEDChangedEventHandler LCULedHasChanged;
        event Definition.RcuMessageReceivedEventHandler RcuReceivedMessage;
        event Definition.HomeMessageReceivedEventHandler HomeReceivedMessage; // Event about a message that concerns the hole Home Controller application, ie not a specific LCU.


        List<string> GetLoggings();
        void GetColorForBackdoorLED();
        void RequestStatusFromRCU(string lcuName, string rcuName);
        void ListenToRCU(string lcuName, string rcuName);
        List<ILocalCentralUnit> GetLcuList();
        void ConnectToAllRCU(string lcuName);
        void ListenToAllRCU(string lcuName);
    }
}
