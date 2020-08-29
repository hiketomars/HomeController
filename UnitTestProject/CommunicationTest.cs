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
using HomeController.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class CommunicationTest
    {
        private Mock<IDoor> frontDoorMock;
        private Mock<IDoorController> frontDoorControllerMock;
        private Mock<IRgbLed> frontDoorRgbLedMock;
        private Mock<ISiren> frontDoorSirenMock;
        private Mock<ISirenController> frontDoorSirenControllerMock;
        private Mock<ILEDController> frontDoorLedControllerMock;
        private LocalCentralUnit frontDoorLcu;

        private Mock<IDoor> backDoorMock;
        private Mock<IDoorController> backDoorControllerMock;
        private Mock<IRgbLed> backDoorRgbLedMock;
        private Mock<ISiren> backDoorSirenMock;
        private Mock<ISirenController> backDoorSirenControllerMock;
        private Mock<ILEDController> backDoorLedControllerMock;
        private LocalCentralUnit backDoorLcu;

        private Mock<IDoor> altanenDoorMock;
        private Mock<IDoorController> altanenDoorControllerMock;
        private Mock<IRgbLed> altanenDoorRgbLedMock;
        private Mock<ISiren> altanenDoorSirenMock;
        private Mock<ISirenController> altanenDoorSirenControllerMock;
        private Mock<ILEDController> altanenDoorLedControllerMock;
        private LocalCentralUnit altanenDoorLcu;
        private string testName = "?";
        private TestLcuHandler testLcuHandler;

        // Test LCU.
        [TestInitialize]
        public void DoSetup()
        {
            testName = "-";
            // Front Door
            // Construct mocks.
            frontDoorMock = new Mock<IDoor>();
            frontDoorMock.SetupAllProperties();

            frontDoorControllerMock = new Mock<IDoorController>();
            frontDoorControllerMock.SetupAllProperties();

            frontDoorRgbLedMock = new Mock<IRgbLed>();
            frontDoorRgbLedMock.SetupAllProperties();

            frontDoorLedControllerMock = new Mock<ILEDController>();
            frontDoorLedControllerMock.SetupAllProperties();

            frontDoorSirenMock = new Mock<ISiren>();
            frontDoorSirenMock.SetupAllProperties();

            frontDoorSirenControllerMock = new Mock<ISirenController>();
            frontDoorSirenControllerMock.SetupAllProperties();

            //
            backDoorMock = new Mock<IDoor>();
            backDoorMock.SetupAllProperties();

            backDoorControllerMock = new Mock<IDoorController>();
            backDoorControllerMock.SetupAllProperties();
            
            backDoorRgbLedMock = new Mock<IRgbLed>();
            backDoorRgbLedMock.SetupAllProperties();
            
            backDoorLedControllerMock = new Mock<ILEDController>();
            backDoorLedControllerMock.SetupAllProperties();
            
            backDoorSirenMock = new Mock<ISiren>();
            backDoorSirenMock.SetupAllProperties();
            
            backDoorSirenControllerMock = new Mock<ISirenController>();
            backDoorSirenControllerMock.SetupAllProperties();


            //
            altanenDoorMock = new Mock<IDoor>();
            altanenDoorMock.SetupAllProperties();

            altanenDoorControllerMock = new Mock<IDoorController>();
            altanenDoorControllerMock.SetupAllProperties();

            altanenDoorRgbLedMock = new Mock<IRgbLed>();
            altanenDoorRgbLedMock.SetupAllProperties();

            altanenDoorLedControllerMock = new Mock<ILEDController>();
            altanenDoorLedControllerMock.SetupAllProperties();

            altanenDoorSirenMock = new Mock<ISiren>();
            altanenDoorSirenMock.SetupAllProperties();

            altanenDoorSirenControllerMock = new Mock<ISirenController>();
            altanenDoorSirenControllerMock.SetupAllProperties();

            testLcuHandler = new TestLcuHandler();
        }


        [TestCleanup]
        public void DoClean()
        {
            frontDoorLcu?.Reset();
            backDoorLcu?.Reset();
            altanenDoorLcu?.Reset();
        }

        //makl 19028 [TestMethod]
        public void C0_CommunicatingProxies()
        {
            string ip = "127.0.0.1";
            var lcuMock1 = new Mock<ILocalCentralUnit>();
            var ah1 = new Mock<IAlarmHandler>();
            lcuMock1.Setup(f => f.LcuAlarmHandler).Returns(ah1.Object);
            var remoteCentralUnitProxy1A = new RemoteCentralUnitProxy(lcuMock1.Object, "remoteLcuB", 2, ip, "1331");

            var lcuMock2 = new Mock<ILocalCentralUnit>();
            var ah2 = new Mock<IAlarmHandler>();
            lcuMock2.Setup(f => f.LcuAlarmHandler).Returns(ah2.Object);
            ah2.Setup(f => f.CurrentLocalStatus).Returns(AlarmHandler.AlarmActivityStatus.EntranceOngoing);

            var remoteCentralUnitProxy2B = new RemoteCentralUnitProxy(lcuMock2.Object, "remoteLcuA", 1, ip,"1330");

            remoteCentralUnitProxy1A.ActivateCommunication();
            remoteCentralUnitProxy2B.ActivateCommunication();

            Task.Delay(3000).Wait();

            Assert.IsTrue(remoteCentralUnitProxy1A.GetRcuCurrentStatusMessage.AlarmStatus ==
                          AlarmHandler.AlarmActivityStatus.EntranceOngoing);
        }

        //makl 19028 [TestMethod]
        public void
            C1_ReadingIsDoorUnlockedFromRemoteControlUnits_WhenAllButOneRemoteDoorsAreLocked_Expect_CorrectResponse()
        {

            ConfigHandler configHandlerFrontDoor = new ConfigHandler("Framsidan", "1341", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Baksidan", "2","192.168.1.8", "1348"),
                }
            );
            var frontDoorLcuRemoteCentralUnitsController = new RemoteCentralUnitsController(null, configHandlerFrontDoor.GetRemoteLcus());

            ConfigHandler configHandlerBackDoor = new ConfigHandler("Baksidan", "1348", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Framsidan", "1", "192.168.1.8", "1341"),
                }
            );

            var backDoorLcuRemoteCentralUnitsController = new RemoteCentralUnitsController(null, configHandlerBackDoor.GetRemoteLcus());


            frontDoorLcu = new LocalCentralUnit(testLcuHandler, frontDoorRgbLedMock.Object, frontDoorLedControllerMock.Object, frontDoorMock.Object,
                frontDoorLcuRemoteCentralUnitsController, frontDoorSirenMock.Object, frontDoorSirenControllerMock.Object);

            backDoorLcu = new LocalCentralUnit(testLcuHandler, backDoorRgbLedMock.Object, backDoorLedControllerMock.Object, backDoorMock.Object,
                backDoorLcuRemoteCentralUnitsController, backDoorSirenMock.Object, backDoorSirenControllerMock.Object);

            /* 190602 Väntar lite med att blanda in även altanen.
            altanenDoorLcu = new LocalCentralUnit(altanenDoorRgbLedMock.Object, altanenDoorLedControllerMock.Object, altanenDoorMock.Object,
                null, altanenDoorSirenMock.Object, altanenDoorSirenControllerMock.Object);
                */
            // Use mocks for doorControllers
            frontDoorLcu.LcuDoorController = frontDoorControllerMock.Object;
            backDoorLcu.LcuDoorController = frontDoorControllerMock.Object;

            /* 190602 Väntar lite med att blanda in även altanen.
            altanenDoorLcu.LcuDoorController = frontDoorControllerMock.Object;
            */
            RemoteMockService.UseMocks = true;

            // Set up backdoor lcu door controler to say that door is locked
            backDoorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);
            frontDoorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);

            //LocalCentralUnit.LcuConfigHandler = configHandlerMock.Object;
            //remoteCentralUnitsController.Setup(lcu);
            //frontDoorLcu.ActivateAlarm(0);

            

            Task.Delay(3000).Wait();


            //// Since front door is first in list it will be asked first and then the second one but the last one will not be checked.
            //rcuFrontDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            //rcuBackDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            //rcuAltanenDoor.Verify(f => f.IsDoorUnlocked(), Times.Never);

            Assert.IsFalse(frontDoorLcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked(),
                "All other doors except for one are locked but controller thinks all are locked.");


            // Set up backdoor lcu door controler to say that door is locked
            backDoorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);
            frontDoorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);

            Task.Delay(3000).Wait();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.IsAnyRemoteDoorUnlocked(),
                "All other doors except for one are locked but controller thinks all are locked.");

        }

        //makl 19028 [UITestMethod]
        public void C20_IntrusionOccursAtOneRemoteLcu_When_AlarmIsActive_Expect_SirenTurnedOnAtLocalCentralUnit()
        {
            testName = "C20v1";
            Logger.Logg(testName, Logger.Test_Cat, "Starting");
            // Sätt upp några remote lcu:er för de olika LCU:erna med mockad siren och dörr.
            // Dess remote controller och dess proxies ska inte vara mockade.
            ConfigHandler configHandlerFrontDoor = new ConfigHandler("Framsidan", "1341", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Baksidan", "2","192.168.1.8", "1348"),
                }
                );
            ConfigHandler configHandlerBackDoor = new ConfigHandler("Baksidan", "1348", new List<IRemoteCentralUnitConfiguration>()
                {
                    new RemoteCentralUnitConfiguration("Framsidan", "1", "192.168.1.8", "1341"),
                }
            );

            //string lcuFrontdoorName = "Framsidan";
            //string lcuBackdoorName = "Baksidan";
            //string lcuAltanenName = "Altanen";

            //string lcuFrontdoorIp = "192.168.11.1";
            //string lcuBackdoorIp = "192.168.11.2";
            //string lcuAltanenIp = "192.168.11.3";

            // Front door
            frontDoorLcu = new LocalCentralUnit(testLcuHandler, configHandlerFrontDoor);
            frontDoorLcu.LcuDoorController.Door = frontDoorMock.Object;
            frontDoorLcu.LcuSirenController.Siren= frontDoorSirenMock.Object;
            frontDoorLcu.StartSurveillance();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 1, "(1)Front door LCU does not have correct number of remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");

            // Back door
            backDoorLcu = new LocalCentralUnit(testLcuHandler, configHandlerBackDoor);
            backDoorLcu.LcuDoorController.Door = backDoorMock.Object;
            backDoorLcu.LcuSirenController.Siren = backDoorSirenMock.Object;
            backDoorLcu.StartSurveillance();

            Logger.Logg(testName, Logger.Test_Cat, "Surveillance started on both LCU:s, wainting 3 sec");
            Task.Delay(3000).Wait();

            Assert.IsTrue(backDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 1, "(2)Back door LCU does not have correct number of remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");
            Logger.Logg(testName, Logger.Test_Cat, "Assert 203");

            // Altanen door
            //altanenDoorLcu = new LocalCentralUnit();
            //altanenDoorLcu.LcuDoorController.Door = altanenDoorMock.Object;
            //altanenDoorLcu.LcuSirenController.Siren = altanenDoorSirenMock.Object;
            //altanenDoorLcu.StartSurveillance();

            Task.Delay(2000).Wait();

            //Assert.IsTrue(altanenDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 1, "(3)Altanen door LCU does not have correct number of  remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");
            //Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.VerifyContact(), "Altanen door LCU does not have contact with all the other remote LCU:s.");

            // Activate alarm. Activating it at the front door shall activate the alarm on all lcu:s.
            Logger.Logg(testName, Logger.Test_Cat, "Alarm will be scheduled for activation");
            frontDoorLcu.ActivateAlarm(500);
            Logger.Logg(testName, Logger.Test_Cat, "Alarm scheduled for activation, sleeping");
            Task.Delay(3000).Wait();


            // Check that alarm really is active on all LCU:s now.
            Assert.IsTrue(frontDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for front door LCU");
            Assert.IsTrue(backDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for back door LCU");
            Assert.IsTrue(altanenDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for altanen LCU");

            Assert.IsFalse(frontDoorLcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "No intrusion at any door but controller does not think so.");
            Task.Delay(2000).Wait();

            // Setup intrusion.
            backDoorLcu.LcuAlarmHandler.EntranceDelayMs = 0;
            Logger.Logg(testName, Logger.Test_Cat, "Opening back door");
            backDoorMock.Setup(f => f.IsOpen).Returns(true);
            Task.Delay(2000).Wait();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "Intrusion at back door but controller does not think so.");

            // Verify that backdoor lcu is alarming
            Assert.IsTrue(backDoorLcu.LcuAlarmHandler.CurrentLocalStatus == AlarmHandler.AlarmActivityStatus.Siren, "Back door LCU is not alarming");

            // Verify that siren is turned on all LCU:s.
            frontDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at front door"); // Will be called many times...
            backDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at back door"); // Will be called many times...
            altanenDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at altanen"); // Will be called many times...

        }

    }
}
