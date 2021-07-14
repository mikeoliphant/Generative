using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Generative
{
    public class Canvasify : BoundsPainter
    {
        SKBitmap colorBitmap;
        float[,] bitmapDensity;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        public Canvasify()
        {
            colorBitmap = ImageUtil.BitmapFromURL("https://cdn.lifestyleasia.com/wp-content/uploads/sites/3/2020/11/16190957/photo-1543349689-9a4d426bee8e-1243x900.jpeg");

            int maxSize = 512;

            if ((colorBitmap.Width > maxSize) || (colorBitmap.Height > maxSize))
            {
                float xScale = (float)maxSize / (float)colorBitmap.Width;
                float yScale = (float)maxSize / (float)colorBitmap.Height;

                float scale = Math.Min(xScale, yScale);

                colorBitmap = colorBitmap.Resize(new SKImageInfo((int)(colorBitmap.Width * scale), (int)(colorBitmap.Height * scale)), SKFilterQuality.High);
            }

            bitmapDensity = ImageUtil.GetBitmapDensity(colorBitmap);

            DesiredAspectRatio = (float)colorBitmap.Width / (float)colorBitmap.Height;
        }

        public override void Paint(SKRect bounds)
        {
            Canvas.DrawBitmap(colorBitmap, bounds);

            int numThreads = 100000;

            for (int x = 0; x < numThreads; x++)
            {
                float xCenter = (float)Random.NextDouble();
                float yCenter = (float)Random.NextDouble();

                SKColor color = colorBitmap.GetPixel((int)(colorBitmap.Width * xCenter), (int)(colorBitmap.Height * yCenter));

                float density = bitmapDensity[(int)(xCenter * colorBitmap.Width), (int)(yCenter * colorBitmap.Height)] / 32;

                float length = (float)Random.NextDouble() * density;

                SKPath path = new SKPath();

                if (Random.NextDouble() < 0.5)
                {
                    path.MoveTo((xCenter - (length / 2)) * bounds.Width, yCenter * bounds.Height);
                    path.LineTo((xCenter + (length / 2)) * bounds.Width, yCenter * bounds.Height);
                }
                else
                {
                    path.MoveTo(xCenter * bounds.Width, (yCenter - (length / 2)) * bounds.Height);
                    path.LineTo(xCenter * bounds.Width, (yCenter + (length / 2)) * bounds.Height);
                }

                paint.Color = color;

                Canvas.DrawPath(path, paint);
            }

            //SKPath borderPath = new SKPath();

            //borderPath.AddRoundRect(new SKRoundRect(bounds, 20));

            //LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
            //perlin.Seed = Random.Next(int.MaxValue);

            //borderPath = PathUtils.NoisifyPath(borderPath, perlin, 0.07f, 10000, -5, 5);

            //Canvas.DrawPath(borderPath, new SKPaint { Color = SKColors.White, StrokeWidth = 20, IsAntialias = true, Style = SKPaintStyle.Stroke });

        }
    }
}