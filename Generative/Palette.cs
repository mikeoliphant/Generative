using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Generative
{
    public static class Palette
    {
        public static SKColor[] RGBY = new SKColor[] { SKColors.Yellow, SKColors.Red, SKColors.Green, SKColors.Blue };
        public static SKColor[] ColdGB = new SKColor[] { SKColor.Parse("220022"), SKColor.Parse("334455"), SKColor.Parse("779988"), SKColor.Parse("ffffdd") };
        public static SKColor[] Pastel = new SKColor[] { SKColor.Parse("7f58af"), SKColor.Parse("64c5eb"), SKColor.Parse("e84d8a"), SKColor.Parse("feb326") };
        public static SKColor[] Sunset = new SKColor[] { SKColor.Parse("264653"), SKColor.Parse("2A9D8F"), SKColor.Parse("E9C46A"), SKColor.Parse("F4A261"), SKColor.Parse("E76F51") };

    }
}
