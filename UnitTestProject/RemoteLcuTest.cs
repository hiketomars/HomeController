using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;
using HomeController.config;
using HomeController.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    public class RemoteLcuTest
    {
        //private IDoor door;
        private Mock<IDoor> doorMock;
        private Mock<IDoorController> doorControllerMock;
        //private IDoorController doorController;
        private Mock<IRgbLed> rgbLedMock;
        private Mock<ISiren> sirenMock;
        private Mock<ISirenController> sirenControllerMock;
        private Mock<ILEDController> ledControllerMock;
        private Mock<IRemoteCentralUnitsController> remoteCentralUnitsControllerMock;
        private LocalCentralUnit lcu;


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

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object, doorControllerMock.Object, remoteCentralUnitsControllerMock.Object, sirenMock.Object, sirenControllerMock.Object);
        }

        [TestCleanup]
        public void DoClean()
        {
            lcu.Reset();
        }

        // LED

        [TestMethod] 
        public void Generally_When_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked_Expect_CorrectLedLight()
        {
            // Door is closed and unlocked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);

            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that green light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        [TestMethod]
        public void RemoteLcuControllerCreated_When_SystemStarted_Expect_ListOfFoundRemoteLcusMatchesConfiguredList()
        {
            var configHandlerMock = new Mock<IConfigHandler>();



            configHandlerMock.Setup(f => f.GetRemoteLcus()).Returns(new List<RemoteCentralUnit>()
            {
                new RemoteCentralUnit("Baksidan", "192.168.0.2", "100"),
                new RemoteCentralUnit("Altanen", "192.168.0.3", "101")
            });

            var remoteCentralUnitsController = new RemoteCentralUnitsController(configHandlerMock.Object);

            //remoteCentralUnitsControllerMock.Setup(f => f.)


        }
    }
}
