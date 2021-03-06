﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using HomeController.comm;
using HomeController.utils;
using Moq;

namespace HomeController.model
{
    /// <summary>
    /// Creates or returns the appropriate IHouseModel implementation.
    /// A real implementation or a Mock.
    /// </summary>
    public class HouseModelFactory
    {
        public static bool TestMode { get; set; }
        public static IHouseModel HouseModel { get; set; }
        public static IHouseModel GetHouseModel()
        {
            // TODO At this point we always returns the real implementation but in future we can return a mock if we're running a unit test.

            if(!TestMode)
            {
                return LcuHandler.GetInstance();
            }

            return HouseModel;// todo return mock here.
        }

        public static IDoor GetDoor(LocalCentralUnit lcu)
        {
            //if (!ExecutionHandler.OsHasGpioCapacity() || lcu.UseAnyMockedDoorProperty)
            //{
            //    // Here goes Windows on my PC.
            //    var mock = new Mock<IDoor>();
            //    mock.SetupAllProperties();
            //    return mock.Object;
            //}
            return new Door();
        }

        public static IRgbLed GetRgbLed(LocalCentralUnit lcu)
        {
            //if(!ExecutionHandler.OsHasGpioCapacity() || lcu.UseAnyMockedDoorProperty)
            //{
            //    var mock = new Mock<IRgbLed>();
            //    mock.SetupAllProperties();
            //    return mock.Object;
            //}
            return new RgbLed();
        }

        public static ISiren GetSiren(LocalCentralUnit lcu)
        {
            //if(!ExecutionHandler.OsHasGpioCapacity() || lcu.UseAnyMockedDoorProperty)
            //{
            //    var mock = new Mock<ISiren>();
            //    mock.SetupAllProperties();
            //    return mock.Object;
            //}
            return new Siren();
        }

    }
}
