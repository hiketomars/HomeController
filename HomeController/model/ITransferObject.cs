using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public interface ITransferObject : ILcuStatus
    {
        string MessageType { get; }
        string Id { get; set; }
        string CompleteMessageStringToSend { get; }
        string SendingLcuName { get; }
        string ToString();
    }
}
