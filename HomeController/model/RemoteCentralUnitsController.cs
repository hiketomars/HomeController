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
using System.Globalization;
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
    // When created it checks the configuration for which remote ecu:s to expect to be found.
    public class RemoteCentralUnitsController : IRemoteCentralUnitsController
    {

        //private LocalCentralUnit lcu;
        private readonly IRemoteCentralUnitProxy rcu;

        
        private IConfigHandler configHandler;
        private List<IRemoteCentralUnitProxy> configuredRemoteCentralUnitProxys = new List<IRemoteCentralUnitProxy>();
        private List<IRemoteCentralUnitProxy> remoteCentralUnitProxies = new List<IRemoteCentralUnitProxy>();

        // This constructor is for real use.
        public RemoteCentralUnitsController()
        {
            configuredRemoteCentralUnitProxys = LocalCentralUnit.LcuConfigHandler.GetRemoteLcus();

            foreach(var configuredRemoteCentralUnit in configuredRemoteCentralUnitProxys)
            {
                IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(configuredRemoteCentralUnit.Name, configuredRemoteCentralUnit.IpAddress, configuredRemoteCentralUnit.PortNumber);
                //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
                remoteCentralUnitProxies.Add(remoteCentralUnit);

            }

        }

        // This constructor is for test use.
        public RemoteCentralUnitsController(List<IRemoteCentralUnitProxy> remoteCentralUnitProxies)
        {
            this.remoteCentralUnitProxies = remoteCentralUnitProxies;
        }

        public int RemoteCentralUnitsCount => remoteCentralUnitProxies.Count;


        public void ActivateRemoteCentralUnits()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.ActivateCommunication();
            }
        }

        // This constructor is for test cases.
        //public RemoteCentralUnitsController(LocalCentralUnit lcu, IConfigHandler configHandler)
        //{
        //    this.lcu = lcu;
        //    Setup(configHandler);
        //}


        public void Setup(LocalCentralUnit lcu)
        {
            //this.lcu = lcu;

            //this.configHandler = configHandler;


            foreach(var configuredRemoteCentralUnit in configuredRemoteCentralUnitProxys)
            {
                IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(configuredRemoteCentralUnit.Name, configuredRemoteCentralUnit.IpAddress, configuredRemoteCentralUnit.PortNumber);
                //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
                remoteCentralUnitProxies.Add(remoteCentralUnit);
                remoteCentralUnit.ActivateCommunication(); // StartListeningOnRemoteLcu();

            }

            //backdoorRemoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;

        }

        public bool VerifyContact()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                var contact = remoteCentralUnitProxy.SendPingMessage();
                if (!contact)
                {
                    Logger.Logg("VerifyContact: No contact with " + remoteCentralUnitProxy.Name);
                    return false;
                }
            }

            return true;
        }
/* Jag tror inte jag behöver tala om att status har ändrats. De får fråga så svarar jag.
        // Inform other LCU:s that our status has changed.
        public void StatusHasChanged(AlarmHandler.AlarmActivityStatus currentStatus)
        {
            foreach(var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.SendStateChangedMessage(currentStatus);
                Logger.Logg("Informed " + remoteCentralUnitProxy.Name + " that the state of "+LocalCentralUnit.GetInstance().Name+" has changed to "+currentStatus);
            }
        }
*/
        private void remoteCentralUnit_RemoteLcuStatusHasChanged(string todotype)
        {
            throw new NotImplementedException();
        }

        // Checks if intrusion has occurred in any of the remote central units.
        public bool HasIntrusionOccurred()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                if (remoteCentralUnitProxy.HasIntrusionOccurred())
                {
                    return true;
                }
                else
                {
                    int a = 0;
                }
            }

            return false;
            /*
            try
            {
                var task = SendCommandToRemoteLcu(MessageHasIntrusionOccurred);
                if(task.Result.StartsWith("exception"))
                {
                    return false;
                }

                return bool.Parse(task.Result);
            }
            catch
            {
                return false;
            }
            */
        }

        public bool IsAnyRemoteDoorUnlocked()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                if (remoteCentralUnitProxy.IsDoorUnlocked())
                {
                    return true;
                }
            }

            return false;
            //try
            //{
            //    var task = SendCommandToRemoteLcu(RemoteCentralUnitProxy.MessageIsDoorUnlocked);
            //    if (task.Result.StartsWith("exception"))
            //    {
            //        return true;
            //    }

            //    return bool.Parse(task.Result);
            //}
            //catch
            //{
            //    return true;
            //}
        }

        public void SendStartUpMessage()
        {
            foreach (var remoteCentralUnitProxy in remoteCentralUnitProxies)
            {
                remoteCentralUnitProxy.SendStartUpMessage();

            }
            //SendCommandToRemoteLcu(MessageLcuStarting);
        }
    }

}
