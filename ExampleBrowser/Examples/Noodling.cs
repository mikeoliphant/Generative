using System;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class Noodling : BoundsPainter
    {
        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        SKColor[] colors = Palette.BlackAndWhite;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f
        };

        public Noodling()
        {
        }

        public override void Paint(SKRect bounds)
        {
            perlin.Seed = Random.Next(int.MaxValue);

            //Canvas.Clear(SKColors.AntiqueWhite);

            int numRings = 140 + Random.Next(colors.Length);

            float startRadiusX = bounds.Width * 0.01f;
            float startRadiusY = bounds.Height * 0.01f;

            SKPath ringPath = new SKPath();
            ringPath.AddOval(new SKRect(bounds.MidX - startRadiusX, bounds.MidY - startRadiusY, bounds.MidX + startRadiusX, bounds.MidY + startRadiusY));

            float scale = 1;

            int drawRings = 0; // (int)(0.75f * numRings);
            //int drawRings = numRings - (colors.Length * 2);

            for (int i = 0; i < numRings; i++)
            {
                if (i >= drawRings)
                {
                    paint.Color = colors[i % colors.Length];

                    //float alpha = (float)(i - (numRings * drawPercent)) / (float)(numRings * (1 - drawPercent));

                    //alpha *= alpha;

                    //paint.Color = new SKColor(paint.Color.Red, paint.Color.Green, paint.Color.Blue, (byte)(alpha * 255));

                    Canvas.DrawPath(ringPath, paint);
                }

                ringPath.Transform(SKMatrix.CreateScale(1 / scale, 1 / scale, bounds.MidX, bounds.MidY));

                ringPath = PathUtils.NoisifyPath(ringPath, perlin, 1, 10000, 0, 0.02f);

                scale += 0.3f;
                
                ringPath.Transform(SKMatrix.CreateScale(scale, scale, bounds.MidX, bounds.MidY));
            }
        }
    }
}
