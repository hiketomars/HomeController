using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.config
{
    //public class ConfigHandler :  IConfigHandler
    //{
    //    private List<IRemoteCentralUnitProxy> remoteLcus;
    //    public ConfigHandler()
    //    {
    //        remoteLcus = new List<IRemoteCentralUnitProxy>()
    //        {
    //            new RemoteCentralUnitProxy("Framsidan", "192.168.11.1", "80"),
    //            new RemoteCentralUnitProxy("Baksidan", "192.168.11.2", "80"),
    //            new RemoteCentralUnitProxy("Altanen", "192.168.11.3", "80")
    //        };
    //    }
    //    public virtual List<IRemoteCentralUnitProxy> GetRemoteLcus()
    //    {
    //        return remoteLcus; 
    //    }
    //}
    public class ConfigHandler : IConfigHandler
    {
        private List<IRemoteCentralUnitProxy> remoteLcus;

        public ConfigHandler(List<IRemoteCentralUnitProxy> remoteLcus)
        {
            this.remoteLcus = remoteLcus;

        }
        public List<IRemoteCentralUnitProxy> GetRemoteLcus()
        {
            return remoteLcus;
        }
    }

}
