﻿
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
    [TestClass][TestFixture]
    public class UnitTest1
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    HouseModelFactory.TestMode = true;
        //    HouseModelFactory.HouseModel = new TestHouseModel();
        //    var testMainView = new TestMainView();
        //    MainPresenter mainPresenter = new MainPresenter(testMainView);

        //}
        

        [TestMethod][Test]
        public void LCUfunctionality_WhenAlarmActiveVeryShort_DoorCalled()
        {
            // Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.
            var doorMock = new Mock<IDoor>();
            var rgbLedMock = new Mock<IRgbLed>();
            LocalCentralUnit lcu = new LocalCentralUnit();
            lcu.Door = doorMock.Object;
            lcu.RgbLed = rgbLedMock.Object;
            doorMock.SetupAllProperties();

            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            lcu.ActiveAlarm(delayInMs);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());
        }

        [TestMethod]
        [Test]
        public void LCUfunctionality_WhenAlarmActiveAndDoorIsOpened_ActivateSirenCalled()
        {
            // Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.
            var doorMock = new Mock<IDoor>();
            var rgbLedMock = new Mock<IRgbLed>();
            LocalCentralUnit lcu = new LocalCentralUnit();
            lcu.Door = doorMock.Object;
            lcu.RgbLed = rgbLedMock.Object;
            doorMock.SetupAllProperties();

            // När LCU anropar IsOpen ska den vara false.
            doorMock.Setup(f => f.IsOpen).Returns(false);

            // Activate alarm.
            int delayInMs = 0;
            lcu.ActiveAlarm(delayInMs);
            Task.Delay(2000).Wait();
            lcu.DeactivateAlarm();

            // Called at least once
            doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());
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
