using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.config
{
    public class ConfigHandler :  IConfigHandler
    {
        public List<IRemoteCentralUnitProxy> GetRemoteLcus()
        {
            return null; // todo
        }
    }
}
