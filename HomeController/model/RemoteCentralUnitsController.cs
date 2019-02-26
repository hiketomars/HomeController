using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HomeController.utils;
using Windows.Data.Json;
using System.Net;
using System.Diagnostics;
using Windows.UI.Core;
using HomeController.model;
using Windows.UI.Xaml;
using System.Threading;
using Windows.System.Threading;
using HomeController.comm;
using HomeController.config;

namespace HomeController.model
{
    // This class controls the communication with other lcu's, so called remote lcu's.
    public class RemoteCentralUnitsController : IRemoteCentralUnitsController
    {
        private IConfigHandler configHandler;

        public RemoteCentralUnitsController()
        {
            Setup(new ConfigHandler());
        }

        public RemoteCentralUnitsController(IConfigHandler configHandler)
        {
            Setup(configHandler);
        }

        private void Setup(IConfigHandler configHandler)
        {
            this.configHandler = configHandler;
        }

        // Checks if intrusion has occurred in any of the remote central units.
        public bool HasIntrusionOccurred()
        {
            return false;
        }

        public bool IsAnyRemoteDoorUnlocked()
        {
            return false;//todo implementera
        }
    }

}
