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

namespace HomeController.model
{
    public class RemoteCentralUnitsController : IRemoteCentralUnitsController
    {
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
