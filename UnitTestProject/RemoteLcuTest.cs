using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications.Management;
using HomeController.comm;
using HomeController.config;
using HomeController.model;
using HomeController.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    //makl 19028 [TestClass]
    public class RemoteLcuTest
    {
        private Mock<IDoor> doorMock;
        private Mock<IDoorController> doorControllerMock;
        private Mock<IRgbLed> rgbLedMock;
        private Mock<ISiren> sirenMock;
        private Mock<ISirenController> sirenControllerMock;
        private Mock<ILEDController> ledControllerMock;
        private Mock<IRemoteCentralUnitsController> remoteCentralUnitsControllerMock;
        private LocalCentralUnit lcu;
        private RemoteCentralUnitProxy rcu;
        private Mock<IRemoteCentralUnitProxy> remoteCentralUnitProxy1;

        // Test LCU.
        [TestInitialize]
        public void DoSetup()
        {
            // Construct mocks.
            doorMock = new Mock<IDoor>();
            doorMock.SetupAllProperties();

            doorControllerMock = new Mock<IDoorController>();
            doorControllerMock.SetupAllProperties();

            rgbLedMock = new Mock<IRgbLed>();
            rgbLedMock.SetupAllProperties();

            ledControllerMock = new Mock<ILEDController>();
            ledControllerMock.SetupAllProperties();

            sirenMock = new Mock<ISiren>();
            sirenMock.SetupAllProperties();

            sirenControllerMock = new Mock<ISirenController>();
            sirenControllerMock.SetupAllProperties();

            remoteCentralUnitsControllerMock = new Mock<IRemoteCentralUnitsController>();
            remoteCentralUnitsControllerMock.SetupAllProperties();

            // Inject properties.
            //ledControllerMock.Object.ControlledRgbLed = rgbLedMock.Object;

            // Create objects
            //doorController = new Mock<IDoorController>().Object;
            //doorController.Door = door;

            //lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object, doorControllerMock.Object, remoteCentralUnitsControllerMock.Object, sirenMock.Object, sirenControllerMock.Object);
            //rcu = new RemoteCentralUnitProxy(lcu, "Backdoor", "192.168.11.2", "1011");

        }


        [TestCleanup]
        public void DoClean()
        {
            lcu?.Reset();
        }


        [TestMethod]
        public void
            R1_ReadingHasIntrusionOccurredFromRemoteControlUnits_WhenNoIntrusionHasOccurredAtAnyDoor_Expect_CorrectResponse()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            // Remote Central Units Controller
            
            //var remoteCentralUnitsController = new RemoteCentralUnitsController(remoteCentralUnits);
            //LocalCentralUnit.xxxxxSetInstance(remoteCentralUnitsController);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                null, sirenMock.Object, sirenControllerMock.Object);

            lcu.LcuDoorController = doorControllerMock.Object;
            lcu.LcuRemoteCentralUnitsController = new RemoteCentralUnitsController(remoteCentralUnits);
            RemoteMockService.UseMocks = true;
            lcu.StartSurveillance();

            //remoteCentralUnitsController.Setup(lcu);
            lcu.ActivateAlarm(0);
            Task.Delay(4000).Wait();

            // All remote central units will be checked for intrusion since none of them return true.
            rcuFrontDoor.Verify(f => f.HasIntrusionOccurred(), Times.AtLeastOnce);
            //rcuBackDoor.Verify(f => f.HasIntrusionOccurred(), Times.AtLeastOnce);
            //rcuAltanenDoor.Verify(f => f.HasIntrusionOccurred(), Times.AtLeastOnce);

            Assert.IsFalse(lcu.LcuAlarmHandler.HasIntrusionOccurredLocally,
                "No intrusion at any door but controller does not think so.");
        }

        [TestMethod]
        public void
            R2_ReadingHasIntrusionOccurredFromRemoteControlUnits_WhenIntrusionHasOccurredAtOneDoor_Expect_CorrectResponse()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurred()).Returns(true);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);

            // Setup Config handler so that remote central unit controller uses these remote lcu proxies.
            //var configHandlerMock = new Mock<IConfigHandler>();
            //configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(remoteCentralUnits);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenControllerMock.Object);

            //LocalCentralUnit.LcuConfigHandler = configHandlerMock.Object;
            //remoteCentralUnitsController.Setup(lcu);
            lcu.StartSurveillance();
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Since front door is first in list it will be asked first and the other doors will not be checked for intrusion.
            rcuFrontDoor.Verify(f => f.HasIntrusionOccurred(), Times.AtLeastOnce);
            rcuBackDoor.Verify(f => f.HasIntrusionOccurred(), Times.Never);
            rcuAltanenDoor.Verify(f => f.HasIntrusionOccurred(), Times.Never);

            Assert.IsTrue(lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "Intrusion at one door but controller does not think so.");
        }


        [TestMethod]
        public void
            R3_ReadingHasIntrusionOccurredFromRemoteControlUnits_WhenIntrusionHasOccurredAtAllDoors_Expect_CorrectResponse()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurred()).Returns(true);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(true);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurred()).Returns(true);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenControllerMock.Object);

            //remoteCentralUnitsController.Setup(lcu);
            lcu.StartSurveillance();
            lcu.ActivateAlarm(0);
            Task.Delay(3000).Wait();

            // All remote central units will be checked for intrusion since none of them return true.
            rcuFrontDoor.Verify(f => f.HasIntrusionOccurred(), Times.AtLeastOnce);
            rcuBackDoor.Verify(f => f.HasIntrusionOccurred(), Times.Never);
            rcuAltanenDoor.Verify(f => f.HasIntrusionOccurred(), Times.Never);

            Assert.IsTrue(lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "Intrusion at all doors but controller thinks no intrusion at all.");
        }

        [TestMethod]
        public void
            R4_ReadingIsDoorUnlockedFromRemoteControlUnits_WhenAllRemoteDoorsAreLocked_Expect_CorrectResponse()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            // Setup Config handler so that remote central unit controller uses these remote lcu proxies.
            //var configHandlerMock = new Mock<IConfigHandler>();
            //configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(remoteCentralUnits);

            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenControllerMock.Object);

            //LocalCentralUnit.LcuConfigHandler = configHandlerMock.Object;
            //remoteCentralUnitsController.Setup(lcu);
            lcu.StartSurveillance();
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Since all doors are locked all will be asked for IsDoorUnlocked.
            rcuFrontDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce,
                "All doors need to be called for this variant");
            rcuBackDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce,
                "All doors need to be called for this variant");
            rcuAltanenDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce,
                "All doors need to be called for this variant");

            Assert.IsFalse(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked(),
                "All other doors are locked but controller does not think so.");
        }


        [TestMethod]
        public void
            R5_ReadingIsDoorUnlockedFromRemoteControlUnits_WhenAllButOneRemoteDoorsAreLocked_Expect_CorrectResponse()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.IsDoorUnlocked()).Returns(true);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            // Setup Config handler so that remote central unit controller uses these remote lcu proxies.
            //var configHandlerMock = new Mock<IConfigHandler>();
            //configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(remoteCentralUnits);

            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenControllerMock.Object);

            //LocalCentralUnit.LcuConfigHandler = configHandlerMock.Object;
            //remoteCentralUnitsController.Setup(lcu);
            lcu.StartSurveillance();
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Since front door is first in list it will be asked first and then the second one but the last one will not be checked.
            rcuFrontDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            rcuBackDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            rcuAltanenDoor.Verify(f => f.IsDoorUnlocked(), Times.Never);

            Assert.IsTrue(lcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked(),
                "All other doors except for one are locked but controller thinks all are locked.");
        }


        [TestMethod]
        public void R20_IntrusionOccurrsAtOneRemoteLcu_When_AlarmIsActive_Expect_SirenTurnedOnAtLocalCentralUnit()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.IsDoorUnlocked()).Returns(true);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            // Setup Config handler so that remote central unit controller uses these remote lcu proxies.
            //var configHandlerMock = new Mock<IConfigHandler>();
            //configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(remoteCentralUnits);
            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenControllerMock.Object);

            //SirenController.SetInstance(sirenControllerMock.Object);
            lcu.LcuSirenController = sirenControllerMock.Object;
            lcu.StartSurveillance();

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            Assert.IsFalse(lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "No intrusion at any door but controller does not think so.");
            Task.Delay(2000).Wait();

            // Setup intrusion.
            // todo Sätt inpasseringstid till 0
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(true);
            Task.Delay(2000).Wait();

            // Verify that siren is turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce); // Will be called many times...

        }

        [TestMethod]
        public void R21_IntrusionOccurrsAtLocalLcu_When_AlarmIsActive_Expect_SirenTurnedOnAtAllRemoteCentralUnits()
        {
            string lcuFrontdoorName = "Ytterdörren";
            string lcuBackdoorName = "Baksidan";
            string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            string lcuAltanenIp = "192.168.11.3";

            // Mocked remote central unit proxys.
            var rcuFrontDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuFrontDoor.Setup(f => f.SendCommandSpecific(lcuFrontdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuFrontDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuFrontdoorName);
            rcuFrontDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuBackDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuBackDoor.Setup(f => f.SendCommandSpecific(lcuBackdoorIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuBackDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuBackdoorName);
            rcuBackDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var rcuAltanenDoor = new Mock<IRemoteCentralUnitProxy>();
            rcuAltanenDoor.Setup(f => f.SendCommandSpecific(lcuAltanenIp, It.IsAny<string>()))
                .ReturnsAsync((string ip, string cmd) => RemoteCentralUnitProxy.MessageACK + cmd);
            rcuAltanenDoor.Setup(f => f.NameOfRemoteLcu).Returns(lcuAltanenName);
            rcuAltanenDoor.Setup(f => f.IsDoorUnlocked()).Returns(false);
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurred()).Returns(false);

            var remoteCentralUnits = new List<IRemoteCentralUnitProxy>
            {
                rcuFrontDoor.Object, rcuBackDoor.Object, rcuAltanenDoor.Object
            };

            doorMock.SetupAllProperties();
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Setup Config handler so that remote central unit controller uses these remote lcu proxies.
            //var configHandlerMock = new Mock<IConfigHandler>();
            //configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(remoteCentralUnits);
            var rcuController = new RemoteCentralUnitsController(remoteCentralUnits);
            var sirenController = new SirenController(null);
            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object,
                rcuController, sirenMock.Object, sirenController);
            sirenController.lcu = lcu;
            //SirenController.SetInstance(sirenControllerMock.Object);
            lcu.LcuSirenController.Siren = sirenMock.Object;
            lcu.LcuDoorController.Door = doorMock.Object;

            lcu.StartSurveillance();

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            Assert.IsFalse(lcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "No intrusion at any door but controller does not think so.");

            // Setup intrusion. No entrance delay.
            lcu.LcuAlarmHandler.EntranceDelayMs = 0;
            //doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);
            doorMock.Setup(f => f.IsOpen).Returns(true);

            Task.Delay(3000).Wait();

            //doorControllerMock.Verify(f=>f.IsDoorOpen()).Re;
            // Verify that siren is turned on.
            //sirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce); // Will be called many times...
            doorMock.Verify(f=>f.IsOpen, Times.AtLeastOnce);
            sirenMock.Verify(f => f.TurnOn(), Times.AtLeastOnce);

            // todo Verify that all other rcu:s has turned on their sirens.
            // 
            rcuAltanenDoor.Setup(f => f.HasIntrusionOccurredRemotely()).Returns(true);
            rcuBackDoor.Setup(f => f.HasIntrusionOccurredRemotely()).Returns(true);
            rcuFrontDoor.Setup(f => f.HasIntrusionOccurredRemotely()).Returns(true);
        }

        [TestMethod]
        public void S1_SirenIsOnAfterAnIntrusion_When_AlarmIsActive_Expect_AllSirenTurnedOffAfterElapsedTime()
        {

        }

        [TestMethod]
        public void S2_IntrusionLocally_When_AlarmIsActive_Expect_AllSirenTurnedOn()
        {

        }
    }
}
