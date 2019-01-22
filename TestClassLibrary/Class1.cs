using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace TestClassLibrary
{
    // This project is a Desktop windows class library
    public class Class1
    {

        [Fact]
        public void Test671()
        {
            Assert.Equal(1, 2);
        }

        [Fact]
        public void Test3()
        {
            //// Skapa en kontrollenhet (RPi) och injecta dörr och led och ev andra kontrollenheter som den ska kommunicera med.
            //var doorMock = new Mock<IDoor>();
            //var rgbLedMock = new Mock<IRgbLed>();
            //LocalCentralUnit lcu = new LocalCentralUnit();
            //lcu.Door = doorMock.Object;
            //lcu.RgbLed = rgbLedMock.Object;

            //// Activate alarm.
            //int delayInMs = 0;
            //lcu.ActiveAlarm(delayInMs);
            //Task.Delay(2000).Wait();
            //lcu.DeactivateAlarm();

            //// Called at least once
            //doorMock.Verify(f => f.IsOpen, Times.AtLeastOnce());
        }

    }
}
