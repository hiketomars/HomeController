using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.config
{
    public interface IConfigHandler
    {
        List<RemoteCentralUnit> GetRemoteLcus();

    }
}
