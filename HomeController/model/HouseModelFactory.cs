﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    /// <summary>
    /// Creates or returns the appropriate IHouseModel implementation.
    /// A real implementation or a Mock.
    /// </summary>
    public class HouseModelFactory
    {
        public static IHouseModel GetHouseModel()
        {
            // TODO At this point we always returns the real implementation but in future we can return a mock if we're running a unit test.
            return HouseController.GetInstance();
        }
    }
}