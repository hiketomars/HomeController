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
            frontDoorLcu.LcuDoorController = frontDoorControllerMock.Object;
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
    }
}
