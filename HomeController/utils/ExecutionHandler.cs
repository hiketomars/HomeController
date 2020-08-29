using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace HomeController.utils
{
    public class ExecutionHandler
    {
        public static bool OsHasGpioCapacity()
        {
            string os = AnalyticsInfo.VersionInfo.DeviceFamily;
            return !os.ToLower().Contains("windows");
        }

    }
}
