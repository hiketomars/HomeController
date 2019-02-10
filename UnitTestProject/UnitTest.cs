using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeController.comm;
using HomeController.model;
using HomeController.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeController.view;
using Moq;
using Assert = NUnit.Framework.Assert;
using NUnit;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {

        //private IDoor door;
        private Mock<IDoor> doorMock;
        private Mock<IDoorController> doorControllerMock;
        //private IDoorController doorController;
        private Mock<IRgbLed> rgbLedMock;
        private Mock<ISiren> sirenMock;
        private Mock<ISirenController> sirenControllerMock;
        private Mock<ILEDController> ledControllerMock;
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

            // Inject properties.
            //ledControllerMock.Object.ControlledRgbLed = rgbLedMock.Object;

            // Create objects
            //doorController = new Mock<IDoorController>().Object;
            //doorController.Door = door;

            lcu = new LocalCentralUnit(rgbLedMock.Object, ledControllerMock.Object, doorMock.Object, doorControllerMock.Object, sirenMock.Object, sirenControllerMock.Object);
        }


        //[TestMethod]
        //public void TestMethod1()
        //{
        //    HouseModelFactory.TestMode = true;
        //    HouseModelFactory.HouseModel = new TestHouseModel();
        //    var testMainView = new TestMainView();
        //    MainPresenter mainPresenter = new MainPresenter(testMainView);

        //}


        [TestMethod] 
        //Kl
        public void WhenActivatingTheAlarmShortly_Expect_IsDoorOpenCalled()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            lcu.ActivateAlarm(delayInMs);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
        }

        [TestMethod]
        // Ready
        public void WhenAlarmIsActiveWithoutEntranceDelayAndDoorIsOpened_Expect_SirenIsTurnedOn()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Activate alarm. No delays

            lcu.EntranceDelay = 0;
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();
            //lcu.DeactivateAlarm();

            // Open the door.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);

            Task.Delay(10).Wait();

            // Verify that alarm is turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Exactly(1));
        }

        [TestMethod]
        public void WhenAlarmIsActivatedAndThenDeactivatedAndDoorIsOpened_Expect_SirenIsNotTurnedOn()
        {
            // Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.

            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            lcu.EntranceDelay = 0;
            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Verify that door was checked.
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());

            Task.Delay(100).Wait();

            // Verify that siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());

        }

        [TestMethod]
        // Ready
        public void WhenAlarmIsNotActiveAndDoorIsOpen_Expect_GreenLEDTurnedOn()
        {
            // Door is open.
            doorMock.Setup(f => f.IsOpen).Returns(true);

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());

            // Verify that green light turned on.
            ledControllerMock.Verify(f => f.SetTotalColor(RGBValue.Green), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());
        }

        [TestMethod]
        // Ready
        public void WhenAlarmIsNotActiveAndDoorIsOpenedAndThenClosed_Expect_GreenLEDTurnedOnThenTurnedOff()
        {
            // Door is open.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);

            // Delay
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());

            // Verify that green light turned on.
            ledControllerMock.Verify(f => f.SetTotalColor(RGBValue.Green), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());

            // Door is closed.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Delay
            Task.Delay(2000).Wait();

            // Verify that green light turned off.
            ledControllerMock.Verify(f => f.SetTotalColor(RGBValue.Black), Times.AtLeastOnce);
        }
    }

    class TestHouseModel : IHouseModel
    {
        public event Definition.VoidEventHandler ModelHasChanged;
        public event Definition.LEDChangedEventHandler LCULedHasChanged;

        public List<string> GetLoggings()
        {
            throw new NotImplementedException();
        }

        public void GetColorForBackdoorLED()
        {
            throw new NotImplementedException();
        }
    }

    class TestMainView : IMainView
    {
        public void AddLoggItem(string text)
        {
            throw new NotImplementedException();
        }

        public void SetLoggItems(List<string> loggings)
        {
            throw new NotImplementedException();
        }

        public void SetColorForBackdoorLED(RGBValue rgbValue)
        {
            throw new NotImplementedException();
        }
    }
}