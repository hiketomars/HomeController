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
    [TestFixture]
    public class UnitTest1
    {

        private IDoor door;
        private Mock<IDoor> doorMock;
        private Mock<IDoorController> doorControllerMock;
        private IDoorController doorController;
        private Mock<IRgbLed> rgbLedMock;
        private Mock<ISiren> sirenMock;
        private Mock<ILEDController> ledControllerMock;
        private LocalCentralUnit lcu;
        

        // Test LCU.
        [SetUp]
        public void DoSetup()
        {
            // Construct mocks.
            doorMock = new Mock<IDoor>();
            doorMock.SetupAllProperties();

            doorControllerMock = new Mock<IDoorController>();
            doorControllerMock.SetupAllProperties();


            rgbLedMock = new Mock<IRgbLed>();
            rgbLedMock.SetupAllProperties();

            sirenMock = new Mock<ISiren>();
            sirenMock.SetupAllProperties();

            ledControllerMock = new Mock<ILEDController>();
            ledControllerMock.SetupAllProperties();

            // Inject properties.
            ledControllerMock.Object.ControlledRgbLed = rgbLedMock.Object;
            
            // Create objects
            door = new Door();
            doorController = new Mock<IDoorController>().Object;
            doorController.Door = door;

            lcu = new LocalCentralUnit();
            lcu.LEDController = ledControllerMock.Object;
            lcu.DoorController = doorControllerMock.Object;
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
        [Test]
        public void WhenActivatingTheAlarmShortly_Expect_DoorControllerIsOpenCalled()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorControllerMock.Setup(f => f.IsDoorOpen).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            lcu.ActivateAlarm(delayInMs);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());
        }

        [TestMethod]
        [Test]
        public void WhenAlarmIsActiveAndDoorIsOpened_Expect_SirenIsTurnedOn()
        {
            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            
            lcu.ActivateAlarm(delayInMs);
            Task.Delay(2000).Wait();
            //lcu.DeactivateAlarm();

            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);


            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());
        }

        [TestMethod]
        [Test]
        public void WhenAlarmIsActivatedAndThenDeactivatedAndDoorIsOpened_Expect_SirenIsNotTurnedOn()
        {
            // Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.
            var doorMock = new Mock<IDoor>();
            var rgbLedMock = new Mock<IRgbLed>();
            var sirenMock = new Mock<ISiren>();
            LocalCentralUnit lcu = new LocalCentralUnit();
            lcu.Door = doorMock.Object;
            //lcu.RgbLed = rgbLedMock.Object;
            lcu.Siren = sirenMock.Object;
            doorMock.SetupAllProperties();

            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            lcu.ActivateAlarm(delayInMs);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());

            // Siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());

            // Kan jag kolla här att IsOn är false?
        }


        [TestMethod]
        [Test]
        public void WhenAlarmIsNotActiveAndDoorIsOpen_Expect_GreenLEDTurnedOn()
        {
            // Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.
            var doorMock = new Mock<IDoor>();
            var ledControllerMock = new Mock<ILEDController>();
            var rgbLedMock = new Mock<IRgbLed>();
            var sirenMock = new Mock<ISiren>();

            LocalCentralUnit lcu = new LocalCentralUnit();
            lcu.Door = doorMock.Object;
            lcu.LEDController = ledControllerMock.Object;
            doorMock.SetupAllProperties();

            // När LCU anropar IsOpen ska den vara true.
            doorMock.Setup(f => f.IsOpen).Returns(true);

            // Activate is not turned on.
            //int delayInMs = 0;
            //lcu.ActiveAlarm(delayInMs);
            //Task.Delay(2000).Wait();
            //lcu.DeactivateAlarm();

            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());

            //rgbLedMock.Verify(f=>f.);
            // Siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());

            // Kan jag kolla här att IsOn är false?
        }

        [Test]
        public void Test4_UTP_UW()
        {
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