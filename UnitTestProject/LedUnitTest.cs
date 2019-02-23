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
    public class LedUnitTest
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

        // Steady green = Door is unlocked but all other are locked.
        [TestMethod] // Ready
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

        // Steady green with flicker = Door is unlocked. Not all of the others are locked.
        [TestMethod]
        public void Generally_When_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked_Expect_CorrectLedLight()
        {
            // Door is closed and unlocked. One or more remote doors are unlocked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(true);

            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify correct led light.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        [TestMethod]
        public void Normally_When_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers_Expect_CorrectLedLight()
        {
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(false);

            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());

        }

        // Steady red with flicker = Door is locked. Not all of the others are locked.
        [TestMethod]
        public void Normally_When_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked_Expect_CorrectLedLight()
        {
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(true);

            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        // Flashing red = Alarm is active. All doors are locked.
        [TestMethod]
        public void Normally_When_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell_Expect_FlashingRedLED()
        {
            // Door is closed and locked. All remote doors are locked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(false);

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        // Flashing red with flicker = Alarm is active. Door is Locked. Not all of the others are locked.
        [TestMethod]
        public void Normally_When_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked_Expect_FlashingFlickeringRedLED()
        {
            // Door is closed and locked. All remote doors are locked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(true);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(true);

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        // Flashing red / green = Alarm is active.Door is not locked. All the others are locked.
        [TestMethod]
        public void Normally_When_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked_Expect_CorrectLedLight()
        {
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(false);

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        // Flashing red / green = Alarm is active.Door is not locked. Not all of the others are locked.
        [TestMethod]
        public void Normally_When_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked_Expect_FlashingFlickeringRedAndGreenLED()
        {
            // Door is closed but not locked. All remote doors are locked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);
            remoteCentralUnitsControllerMock.Setup(f => f.IsAnyRemoteDoorUnlocked()).Returns(true);

            lcu.ActivateAlarm(0);
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            // Verify that remote doors are checked.
            remoteCentralUnitsControllerMock.Verify(f => f.IsAnyRemoteDoorUnlocked(), Times.AtLeastOnce());

            // Verify that correct light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        [TestMethod]
        public void DoorIsOpenAndThenClosed_When_AlarmIsInactiveAndAllOthersAreLocked_Expect_CorrectLedLightInBothSituations()
        {
            // Door is open.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(true);

            // Delay
            Task.Delay(2000).Wait();

            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());

            // Verify correct led light.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsOpenButAllOtherDoorsAreLocked(), Times.AtLeastOnce);

            // Verify that siren is not turned on.
            sirenMock.Verify(f => f.TurnOn(), Times.Never());

            // Door is closed.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);

            // Delay
            Task.Delay(2000).Wait();
        
            // Verify correct led light.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked(), Times.AtLeastOnce);
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