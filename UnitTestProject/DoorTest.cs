using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.comm;
using HomeController.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    public class DoorTest
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
        private TestLcuHandler testLcuHandler;


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
            testLcuHandler = new TestLcuHandler();

            lcu = new LocalCentralUnit(testLcuHandler, rgbLedMock.Object, ledControllerMock.Object, doorMock.Object, remoteCentralUnitsControllerMock.Object, sirenMock.Object, sirenControllerMock.Object);
        }

        [TestCleanup]
        public void DoClean()
        {
            lcu?.Reset();
        }

        // Door
        [TestMethod]
        public void WhenActivatingTheAlarmShortly_Expect_IsDoorOpenCalled()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Activate alarm.
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());

        }

        // Siren
        [TestMethod]
        public void DoorIsOpened_When_AlarmIsActiveWithoutDelays_Expect_SirenIsTurnedOn()
        {
            // No activation delay.
            //lcu.LcuAlarmHandler.ActivationDelayMs = 0;

            // No entrance delay.
            lcu.LcuAlarmHandler.EntranceDelayMs = 0;

            // När LCU anropar IsOpen ska den vara false.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Activate alarm.
            lcu.ActivateAlarm(0);

            // Open the door.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);

            Task.Delay(2000).Wait();
            //lcu.DeactivateAlarm();

            Task.Delay(10).Wait();

            // Verify that alarm is turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Exactly(1));
        }

        [TestMethod]
        public void DoorIsOpened_When_AlarmHasBeenInactivatedWithoutDelays_Expect_SirenIsNotTurnedOn()
        {
            // No entrance delay.
            lcu.LcuAlarmHandler.EntranceDelayMs = 0;

            // Activate alarm.
            lcu.ActivateAlarm(0);

            lcu.DeactivateAlarm();


            // Open the door.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);

            Task.Delay(2000).Wait();

            // Verify that alarm is turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never);
        }

        [TestMethod]
        public void DoorIsOpened_When_AlarmHasBeenActivatedAndThenDeactivated_Expect_SirenIsNotTurnedOn()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Verify that door was checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());

            Task.Delay(100).Wait();

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());

        }

    }
}
