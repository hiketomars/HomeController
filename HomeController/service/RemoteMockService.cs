using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;

namespace HomeController.service
{
    public static class RemoteMockService
    {

        public static List<IRemoteCentralUnitProxy> RemoteCentralUnitProxyMocks { get; set; }
        public static bool UseMocks { get; set; }
    }
}
