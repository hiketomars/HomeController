﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
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
            lcu.LcuDoorController = doorControllerMock.Object;
            lcu.StartSurveillance();
        }


        [TestCleanup]
        public void DoClean()
        {
            lcu?.Reset();
        }


        // LED

        // Steady green = Door is unlocked but all other are locked.
        [TestMethod] // Ready
        public void L1_Generally_When_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked_Expect_CorrectLedLight()
        {
            System.Diagnostics.Debug.WriteLine("hej");


            var localRoot = ApplicationData.Current.LocalFolder.Path;
            DirectoryInfo d = new DirectoryInfo(localRoot + "\\test");
            if (!d.Exists)
            {
                d.Create();
            }

            Logger.Logg("L1", Logger.Test_Cat, "Running test.");
            // Door is closed and unlocked.
            doorControllerMock.Setup(f => f.IsDoorOpen()).Returns(false);
            doorControllerMock.Setup(f => f.IsDoorLocked()).Returns(false);
            int delay = 2000;
            Logger.Logg("L1", Logger.Test_Cat, "Will wait "+delay);

            Task.Delay(delay).Wait();
            Logger.Logg("L1", Logger.Test_Cat, "Have waited "+delay);


            // Verify that door is checked.
            doorControllerMock.Verify(f => f.IsDoorOpen(), Times.AtLeastOnce());
            doorControllerMock.Verify(f => f.IsDoorLocked(), Times.AtLeastOnce());

            Logger.Logg("L1", Logger.Test_Cat, "Verified that door is checked.");

            // Verify that green light turned on.
            ledControllerMock.Verify(f => f.SetLed_AlarmIsInactiveAndDoorIsUnlockedButAllOthersAreLocked(), Times.AtLeastOnce);
            Logger.Logg("L1", Logger.Test_Cat, "Verified that LED is set correctly.");

            // Verify that siren is not turned on.
            sirenControllerMock.Verify(f => f.TurnOn(), Times.Never());
        }

        // Steady green with flicker = Door is unlocked. Not all of the others are locked.
        [TestMethod]
        public void L2_Generally_When_AlarmIsInactiveAndDoorIsUnlockedAndNotAllOthersAreLocked_Expect_CorrectLedLight()
        {
            Logger.Logg("L2", Logger.Test_Cat, "Running test.");

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
        public void L3_Normally_When_AlarmIsInactiveAndDoorIsLockedAndSoAreAllTheOthers_Expect_CorrectLedLight()
        {
            Logger.Logg("L3", Logger.Test_Cat, "Running test.");

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
        public void L4_Normally_When_AlarmIsInactiveAndDoorIsLockedButNotAllTheOthersAreLocked_Expect_CorrectLedLight()
        {
            Logger.Logg("L4", Logger.Test_Cat, "Running test.");

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
        public void L5_Normally_When_AlarmIsActiveAndDoorIsLockedAndAllTheOthersAsWell_Expect_FlashingRedLED()
        {
            Logger.Logg("L5", Logger.Test_Cat, "Running test.");

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
        public void L6_Normally_When_AlarmIsActiveAndDoorIsLockedButNotAllTheOthersAreLocked_Expect_FlashingFlickeringRedLED()
        {
            Logger.Logg("L6", Logger.Test_Cat, "Running test.");

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
        public void L7_Normally_When_AlarmIsActiveAndDoorIsUnlockedButAllTheOthersAreLocked_Expect_CorrectLedLight()
        {
            Logger.Logg("L7", Logger.Test_Cat, "Running test.");

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
        public void L8_Normally_When_AlarmIsActiveAndDoorIsUnlockedAndNotAllTheOthersAreLocked_Expect_FlashingFlickeringRedAndGreenLED()
        {
            Logger.Logg("L8", Logger.Test_Cat, "Running test.");

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
        public void L9_DoorIsOpenAndThenClosed_When_AlarmIsInactiveAndAllOthersAreLocked_Expect_CorrectLedLightInBothSituations()
        {
            Logger.Logg("L9", Logger.Test_Cat, "Running test.");

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
        public event Definition.VoidEventHandler LcuInstancesHasChanged;
        public event Definition.LEDChangedEventHandler LCULedHasChanged;
        public event Definition.RcuMessageReceivedEventHandler RcuReceivedMessage;
        public event Definition.LcuRelatedMessageEventHandler LcuRelatedMessage;
        public event Definition.HomeMessageReceivedEventHandler HomeReceivedMessage;

        public List<string> GetLoggings()
        {
            throw new NotImplementedException();
        }

        public void GetColorForBackdoorLED()
        {
            throw new NotImplementedException();
        }

        public void RequestStatusFromRCU(string lcuName, string rcuName)
        {
            throw new NotImplementedException();
        }

        //public void ListenToRCU(string lcuName, string rcuName)
        //{
        //    throw new NotImplementedException();
        //}

        public List<ILocalCentralUnit> GetLcuList()
        {
            throw new NotImplementedException();
        }

        public void ConnectToAllRCU(string lcuName)
        {
            throw new NotImplementedException();
        }

        public void ListenToAllRCU(string lcuName)
        {
            throw new NotImplementedException();
        }

        public void ConnectToRCU(string lcuName, string rcuName)
        {
            throw new NotImplementedException();
        }

        public void ActionBtn(string lcuName, string rcuName, string actionSelectorSelectValue)
        {
            throw new NotImplementedException();
        }

        public bool UseVirtualIo { get; set; }
    }

    class TestMainView : IMainView
    {
        public void AddHouseLoggText(string text)
        {
            throw new NotImplementedException();
        }

        public void SetLcuInfoText(string lcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddHouseLoggText(List<string> loggings)
        {
            throw new NotImplementedException();
        }

        public void SetColorForBackdoorLED(RGBValue rgbValue)
        {
            throw new NotImplementedException();
        }

        public void SetLcus(List<ILocalCentralUnit> lcus)
        {
            throw new NotImplementedException();
        }

        public void SetHouseStatusText(string text)
        {
            throw new NotImplementedException();
        }

        public void SetLcuLoggText(string lcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddLcuLoggText(string lcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddRcuLoggText(string lcuName, string rcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddRcuSendCounterText(string lcuName, string rcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddRcuReceiveCounterText(string lcuName, string rcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void AddRcuAlarmStatusText(string lcuName, string rcuName, string text)
        {
            throw new NotImplementedException();
        }

        public void ClearRcuText(string lcuName, string rcuName)
        {
            throw new NotImplementedException();
        }

        public void EnableDoorOpenCheckbox(string lcuName, bool enable)
        {
            throw new NotImplementedException();
        }

        public void EnableDoorFloatingCheckbox(string lcuName, bool enable)
        {
            throw new NotImplementedException();
        }

        public void EnableDoorLockedCheckbox(string lcuName, bool enable)
        {
            throw new NotImplementedException();
        }

        public void DisableDoorOpenCheckbox(string lcuName)
        {
            throw new NotImplementedException();
        }

        public void DisableDoorFloatingCheckbox(string lcuName)
        {
            throw new NotImplementedException();
        }

        public void DisableDoorLockedCheckbox(string lcuName)
        {
            throw new NotImplementedException();
        }

        public void CheckAndDisableUseVirtualIo(string lcuName, bool checkAndDisable)
        {
            throw new NotImplementedException();
        }

        public void CheckUncheckAllUseVirtual(string lcuName, bool check)
        {
            throw new NotImplementedException();
        }
    }
}