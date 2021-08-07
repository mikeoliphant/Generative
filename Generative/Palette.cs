using System;
using System.Numerics;
using SkiaSharp;

namespace Generative
{
    public static class Palette
    {
        public static SKColor[] RGBY = new SKColor[] { SKColors.Yellow, SKColors.Red, SKColors.Green, SKColors.Blue };
        public static SKColor[] ColdGB = new SKColor[] { SKColor.Parse("220022"), SKColor.Parse("334455"), SKColor.Parse("779988"), SKColor.Parse("ffffdd") };
        public static SKColor[] Pastel = new SKColor[] { SKColor.Parse("7f58af"), SKColor.Parse("64c5eb"), SKColor.Parse("e84d8a"), SKColor.Parse("feb326") };
        public static SKColor[] Sunset = new SKColor[] { SKColor.Parse("264653"), SKColor.Parse("2A9D8F"), SKColor.Parse("E9C46A"), SKColor.Parse("F4A261"), SKColor.Parse("E76F51") };
        public static SKColor[] Sunny = new SKColor[] { SKColor.Parse("8ECAE6"), SKColor.Parse("219EBC"), SKColor.Parse("023047"), SKColor.Parse("FFB703"), SKColor.Parse("FB8500") };
        public static SKColor[] Rainbow = new SKColor[] { SKColor.Parse("ff0000"), SKColor.Parse("ffa500"), SKColor.Parse("ffff00"), SKColor.Parse("008000"), SKColor.Parse("0000ff"), SKColor.Parse("4b0082"), SKColor.Parse("ee82ee") };
        public static SKColor[] RainbowMuted = new SKColor[] { SKColor.Parse("e6261f"), SKColor.Parse("eb7532"), SKColor.Parse("f7d038"), SKColor.Parse("a3e048"), SKColor.Parse("49da9a"), SKColor.Parse("34bbe6"), SKColor.Parse("4355db"), SKColor.Parse("d23be7") };
        public static SKColor[] CandyCane = new SKColor[] { SKColor.Parse("FF4747"), SKColor.Parse("FFE6E6") };
        public static SKColor[] BarberPole = new SKColor[] { SKColor.Parse("E24C3B"), SKColor.Parse("FBF3EF"), SKColor.Parse("0088E0"), SKColor.Parse("FBF3EF") };
        public static SKColor[] Seventies = new SKColor[] { SKColor.Parse("75c8ae"), SKColor.Parse("5a3d2b"), SKColor.Parse("ffecb4"), SKColor.Parse("e5771e"), SKColor.Parse("f4a127") };
        public static SKColor[] SeventiesBright = new SKColor[] { SKColor.Parse("f5c600"), SKColor.Parse("d8460b"), SKColor.Parse("c21703"), SKColor.Parse("9b4923"), SKColor.Parse("007291") };
        public static SKColor[] Vibrant = new SKColor[] { SKColor.Parse("FFBE0B"), SKColor.Parse("FB5607"), SKColor.Parse("FF006E"), SKColor.Parse("8338EC"), SKColor.Parse("3A86FF") };
        public static SKColor[] Neaplolitan = new SKColor[] { SKColor.Parse("ff8daa"), SKColor.Parse("fdaeae"), SKColor.Parse("f6e2b3"), SKColor.Parse("b49982"), SKColor.Parse("7e6651") };
        public static SKColor[] Seaweed = new SKColor[] { SKColor.Parse("0B7B8B"), SKColor.Parse("20B0AA"), SKColor.Parse("9EE2BE"), SKColor.Parse("6EBB7F"), SKColor.Parse("4A9A64") };
        public static SKColor[] BlackAndWhite = new SKColor[] { SKColors.Black, SKColors.White };
    }

    public class CosinePalette
    {
        Vector3 a, b, c, d;

        public CosinePalette(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public SKColor GetColor(float t)
        {
            Vector3 result = ((c * t) + d) * (float)Math.PI * 2;

            result = new Vector3((float)Math.Cos(result.X), (float)Math.Cos(result.Y), (float)Math.Cos(result.Z));

            result =  a + (b * result);

            return new SKColor((byte)(result.X * 255), (byte)(result.Y * 255), (byte)(result.Z * 255));
        }
    }
}
