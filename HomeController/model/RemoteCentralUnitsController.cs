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
        //private List<IRemoteCentralUnitProxy> configuredRemoteCentralUnitProxys = new List<IRemoteCentralUnitProxy>();
        private List<IRemoteCentralUnitProxy> remoteCentralUnitProxies = new List<IRemoteCentralUnitProxy>();

        //// This constructor is for real use.
        //public RemoteCentralUnitsController(LocalCentralUnit lcu)
        //{
        //    configuredRemoteCentralUnitProxys = lcu.LcuConfigHandler.GetRemoteLcus();

        //    foreach(var configuredRemoteCentralUnit in configuredRemoteCentralUnitProxys)
        //    {
        //        IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(lcu, configuredRemoteCentralUnit.Name, configuredRemoteCentralUnit.IpAddress, configuredRemoteCentralUnit.PortNumber);
        //        //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
        //        remoteCentralUnitProxies.Add(remoteCentralUnit);

        //    }

        //}


        // For debug purpose.
        public void RequestStatusFromRcu()
        {
            if (remoteCentralUnitProxies.Count != 1)
            {
                throw new Exception("RequestStatusFromRcu only available when you have a single RCU.");
            }

            remoteCentralUnitProxies[0].RequestStatusFromRcu();
        }

        //public void ListenToTheOnlyRcu()
        //{
        //    if(remoteCentralUnitProxies.Count != 1)
        //    {
        //        throw new Exception("ListenToOnlyRcu only available when you have a single RCU.");
        //    }

        //    remoteCentralUnitProxies[0].StartListeningToRemoteLcu();
        //}

        public void ListenToRcu(string rcuName)
        {
            var rcu = remoteCentralUnitProxies.Find(e=>e.NameOfRemoteLcu == rcuName);
            rcu.StartListeningToRemoteLcu();
        }

        public void ListenToAllRcus()
        {
            foreach (var rcu in remoteCentralUnitProxies)
            {
                rcu.StartListeningToRemoteLcu();
            }
        }

        public void ConnectToAllRcus()
        {
            foreach(var rcu in remoteCentralUnitProxies)
            {
                rcu.RequestStatusFromRcu();
            }
        }


        public void ActivateCommunicationOnAllProxys()
        {
            foreach (var configuredRemoteCentralUnit in remoteCentralUnitProxies)
            {
                configuredRemoteCentralUnit.ActivateCommunication();
            }
        }

        // Collect the statuses from the LCU:s and finds out which one has most recently changed its alarm status.
        public CompoundStatus GetCompoundStatus()
        {
            DateTime newestAlarmStatus = new DateTime(1970, 1, 1, 0, 0, 0);
            //RemoteLcuStatus newestLcuStatus = configuredRemoteCentralUnitProxys[0].RemoteLcuStatus; 
            IRemoteCentralUnitProxy mostRecentChangedLcu = remoteCentralUnitProxies[0]; // Initiate //todo Kan det vara så att vi inte har några RCU:er?

            var compoundStatus = new CompoundStatus();
            foreach(var configuredRemoteCentralUnit in remoteCentralUnitProxies)
            {
                compoundStatus.LcuStatuses.Add(configuredRemoteCentralUnit.RcuCurrentStatusMessage);

                if (configuredRemoteCentralUnit.RcuCurrentStatusMessage.StatusTime > mostRecentChangedLcu.RcuCurrentStatusMessage.StatusTime)
                {
                    mostRecentChangedLcu = configuredRemoteCentralUnit;
                }
            }

            compoundStatus.MostRecentChangedLcu = mostRecentChangedLcu;
            return compoundStatus;
        }

        public List<IRemoteCentralUnitProxy> RcuList
        {
            get
            {
                return remoteCentralUnitProxies;
            }
        }
        public LocalCentralUnit Lcu { get; set; }
        private List<IRemoteCentralUnitConfiguration> remoteCentralUnitConfigurations;

        // This constructor is used by unit tests.
        public RemoteCentralUnitsController(List<IRemoteCentralUnitProxy> remoteCentralUnitProxies)
        {
            this.remoteCentralUnitProxies = remoteCentralUnitProxies;
        }
        
        // This constructor is for test use.
        public RemoteCentralUnitsController(LocalCentralUnit lcu, List<IRemoteCentralUnitConfiguration> remoteCentralUnitConfigurations)
        {
            Lcu = lcu;
            this.remoteCentralUnitConfigurations = remoteCentralUnitConfigurations;
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


        // If this RemoteCentralUnitsController has been created from a list of RemoteCentralUnitConfigurations then
        // this Setup has to be called to create the actual list of CentralUnitProxies.
        public void Setup(LocalCentralUnit lcu)
        {
            foreach(var remoteCentralUnitConfiguration in remoteCentralUnitConfigurations)
            {
                IRemoteCentralUnitProxy remoteCentralUnit = new RemoteCentralUnitProxy(lcu, remoteCentralUnitConfiguration.Name, remoteCentralUnitConfiguration.Id, remoteCentralUnitConfiguration.IpAddress, remoteCentralUnitConfiguration.InitiatorPortNumber, remoteCentralUnitConfiguration.ResponderPortNumber);
                //remoteCentralUnit.RemoteLcuStatusHasChanged += remoteCentralUnit_RemoteLcuStatusHasChanged;
                remoteCentralUnitProxies.Add(remoteCentralUnit);
                //remoteCentralUnit.ActivateCommunication(); // StartListeningOnRemoteLcu();

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
                    Logger.Logg(Lcu.Name, Logger.RCUCtrl_Cat, "VerifyContact: No contact with " + remoteCentralUnitProxy.NameOfRemoteLcu);
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
        public static string RCUCtrl_Cat;
                Logger.Logg(lcu.Name, "Informed " + remoteCentralUnitProxy.Name + " that the state of "+LocalCentralUnit.GetInstance().Name+" has changed to "+currentStatus);
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

        //public void InformAboutNewAlarmStatus(AlarmHandler.AlarmActivityStatus status)
        //{
        //    foreach(var remoteCentralUnitProxy in remoteCentralUnitProxies)
        //    {
        //        remoteCentralUnitProxy.InformAboutNewAlarmStatus();

        //    }
        //    //SendCommandToRemoteLcu(MessageLcuStarting);
        //}

    }

}
