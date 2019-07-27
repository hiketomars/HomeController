using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface ILocalCentralUnit
    {
        string Name { get; }
        IAlarmHandler LcuAlarmHandler { get; }
        IRemoteCentralUnitsController LcuRemoteCentralUnitsController { get; }
}
}
