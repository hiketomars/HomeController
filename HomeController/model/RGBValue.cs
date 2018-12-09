using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.model
{
    public class RGBValue
    {
        public RGBValue(byte redPart, byte greenPart, byte bluePart)
        {
            RedPart = redPart;
            GreenPart = greenPart;
            BluePart = bluePart;
        }

        public static RGBValue Red => new RGBValue(255, 0, 0);
        public static RGBValue Green => new RGBValue(0, 255, 0);
        public static RGBValue Blue => new RGBValue(0, 0, 255);

        public byte RedPart { get; set; }
        public byte GreenPart { get; set; }
        public byte BluePart { get; set; }
    }
}
