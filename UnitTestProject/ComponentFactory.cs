using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeController.model;
using Moq;

namespace UnitTestProject
{
    public class ComponentFactory
    {
        public static ILEDController GetLedController()
        {
            return new Mock<ILEDController>().Object;
        }
    }
}
