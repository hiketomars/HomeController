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
        List<string> GetLoggings();
        void GetColorForBackdoorLED();
        void ConnectToLCU(string lcuName, string rcuName);
        void ListenToRCU(string lcuName, string rcuName);
        List<ILocalCentralUnit> GetLcuList();
        void ConnectToAllRCU(string lcuName);
        void ListenToAllRCU(string lcuName);
    }
}
