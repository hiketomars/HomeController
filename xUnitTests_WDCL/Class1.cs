using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTests_WDCL
{
    public class Class1
    {
        [Fact]
        public void PassingTest_1_xUnit_WDCL()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest_1_xUnit_WDCL()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
