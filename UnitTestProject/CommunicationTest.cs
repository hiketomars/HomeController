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

        // Test LCU.
        [TestInitialize]
        public void DoSetup()
        {
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

        }


        [TestCleanup]
        public void DoClean()
        {
            frontDoorLcu?.Reset();
            backDoorLcu?.Reset();
            altanenDoorLcu?.Reset();
        }



        [TestMethod]
        public void
            C1_ReadingIsDoorUnlockedFromRemoteControlUnits_WhenAllButOneRemoteDoorsAreLocked_Expect_CorrectResponse()
        {

            frontDoorLcu = new LocalCentralUnit(frontDoorRgbLedMock.Object, frontDoorLedControllerMock.Object, frontDoorMock.Object,
                null, frontDoorSirenMock.Object, frontDoorSirenControllerMock.Object);

            backDoorLcu = new LocalCentralUnit(backDoorRgbLedMock.Object, backDoorLedControllerMock.Object, backDoorMock.Object,
               null, backDoorSirenMock.Object, backDoorSirenControllerMock.Object);

            altanenDoorLcu = new LocalCentralUnit(altanenDoorRgbLedMock.Object, altanenDoorLedControllerMock.Object, altanenDoorMock.Object,
                null, altanenDoorSirenMock.Object, altanenDoorSirenControllerMock.Object);

            // Use mocks for doorControllers
            frontDoorLcu.
LcuDoorController = frontDoorControllerMock.Object;
            backDoorLcu.LcuDoorController = frontDoorControllerMock.Object;
            altanenDoorLcu.LcuDoorController = frontDoorControllerMock.Object;
            RemoteMockService.UseMocks = true;


            //LocalCentralUnit.LcuConfigHandler = configHandlerMock.Object;
            //remoteCentralUnitsController.Setup(lcu);
            //frontDoorLcu.ActivateAlarm(0);
            //Task.Delay(2000).Wait();


            //// Since front door is first in list it will be asked first and then the second one but the last one will not be checked.
            //rcuFrontDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            //rcuBackDoor.Verify(f => f.IsDoorUnlocked(), Times.AtLeastOnce);
            //rcuAltanenDoor.Verify(f => f.IsDoorUnlocked(), Times.Never);

            //Assert.IsTrue(RemoteCentralUnitsController.GetInstance().IsAnyRemoteDoorUnlocked(),
            //    "All other doors except for one are locked but controller thinks all are locked.");
        }

        [UITestMethod]
        public void C20_IntrusionOccursAtOneRemoteLcu_When_AlarmIsActive_Expect_SirenTurnedOnAtLocalCentralUnit()
        {

            // Sätt upp tre lcu:er med mockad siren och dörr. Dess remote controller och dess proxies ska inte vara mockade.
            ConfigHandler configHandlerFrontDoor = new ConfigHandler(
                    new List<IRemoteCentralUnitProxy>()
                    {
                        new RemoteCentralUnitProxy("Baksidan", "192.168.11.2", "80"),
                        //new RemoteCentralUnitProxy("Altanen", "192.168.11.3", "80")
                    }
                );
            ConfigHandler configHandlerBackDoor = new ConfigHandler(
                new List<IRemoteCentralUnitProxy>()
                {
                    new RemoteCentralUnitProxy("Framsidan", "192.168.11.1", "80"),
                    //new RemoteCentralUnitProxy("Altanen", "192.168.11.3", "80")
                }
            );

            string lcuFrontdoorName = "Framsidan";
            string lcuBackdoorName = "Baksidan";
            //string lcuAltanenName = "Altanen";

            string lcuFrontdoorIp = "192.168.11.1";
            string lcuBackdoorIp = "192.168.11.2";
            //string lcuAltanenIp = "192.168.11.3";

            // Front door
            frontDoorLcu = new LocalCentralUnit(configHandlerFrontDoor);
            frontDoorLcu.LcuDoorController.Door = frontDoorMock.Object;
            frontDoorLcu.LcuSirenController.Siren= frontDoorSirenMock.Object;
            frontDoorLcu.StartSurveillance();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 3, "(1)Front door LCU does not have 3 remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");

            // Back door
            backDoorLcu = new LocalCentralUnit(configHandlerBackDoor);
            backDoorLcu.LcuDoorController.Door = backDoorMock.Object;
            backDoorLcu.LcuSirenController.Siren = backDoorSirenMock.Object;
            backDoorLcu.StartSurveillance();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 3, "(2)Front door LCU does not have 3 remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");

            // Altanen door
            //altanenDoorLcu = new LocalCentralUnit();
            //altanenDoorLcu.LcuDoorController.Door = altanenDoorMock.Object;
            //altanenDoorLcu.LcuSirenController.Siren = altanenDoorSirenMock.Object;
            //altanenDoorLcu.StartSurveillance();

            Task.Delay(2000).Wait();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount == 3, "(3)Front door LCU does not have 3 remote LCU:s, it has " + frontDoorLcu.LcuRemoteCentralUnitsController.RemoteCentralUnitsCount + " remote LCU:s.");
            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.VerifyContact(), "Front door LCU does not have contact with all the other remote LCU:s.");

            // Activate alarm. Activating it at the front door shall activate the alarm on all lcu:s.
            frontDoorLcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Check that alarm really is active on all LCU:s now.
            Assert.IsTrue(frontDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for front door LCU");
            Assert.IsTrue(backDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for back door LCU");
            Assert.IsTrue(altanenDoorLcu.LcuAlarmHandler.IsAlarmActive, "Alarm not active for altanen LCU");

            Assert.IsFalse(frontDoorLcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "No intrusion at any door but controller does not think so.");
            Task.Delay(2000).Wait();

            // Setup intrusion.
            backDoorLcu.LcuAlarmHandler.EntranceDelayMs = 0;
            backDoorMock.Setup(f => f.IsOpen).Returns(true);
            Task.Delay(2000).Wait();

            Assert.IsTrue(frontDoorLcu.LcuRemoteCentralUnitsController.HasIntrusionOccurred(),
                "Intrusion at back door but controller does not think so.");

            // Verify that backdoor lcu is alarming
            Assert.IsTrue(backDoorLcu.LcuAlarmHandler.CurrentStatus == AlarmHandler.AlarmActivityStatus.Siren, "Back dorr LCU is not alarming");

            // Verify that siren is turned on all LCU:s.
            frontDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at front door"); // Will be called many times...
            backDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at back door"); // Will be called many times...
            altanenDoorSirenControllerMock.Verify(f => f.TurnOn(), Times.AtLeastOnce, "Siren not turned on at altanen"); // Will be called many times...

        }

    }
}
