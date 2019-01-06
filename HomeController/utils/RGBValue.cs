using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.utils
{
    /// <summary>
    /// RGBValue holds a value between 0 and 255 for each of the three colors red, green and blue.
    /// </summary>
    public class RGBValue
    {
        public RGBValue(byte redPart, byte greenPart, byte bluePart)
        {
            RedPart = redPart;
            GreenPart = greenPart;
            BluePart = bluePart;
        }

        public static utils.RGBValue Red => new utils.RGBValue(255, 0, 0);
        public static utils.RGBValue Green => new utils.RGBValue(0, 255, 0);
        public static utils.RGBValue Blue => new utils.RGBValue(0, 0, 255);

        public byte RedPart { get; set; }
        public byte GreenPart { get; set; }
        public byte BluePart { get; set; }

        public bool HasGreenPart => RedPart   > 0;
        public bool HasRedPart   => GreenPart > 0;
        public bool HasBluePart  => BluePart  > 0;

        public override string ToString()
        {
            return "RGB: " + RedPart + ";" + GreenPart + ";" + BluePart + ";";
        }
    }
}
