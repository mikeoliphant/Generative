using System;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class Impressionist : BoundsPainter
    {
        SKBitmap bitmap;
        float[,] bitmapDensity;
        SKBitmap brushBitmap;

        public Impressionist()
        {
            bitmap = ImageUtil.BitmapFromURL("https://avatars.githubusercontent.com/u/6710799?v=4");

            brushBitmap = ImageUtil.BitmapFromResource("ExampleBrowser.Images.Brush.png");

            int maxSize = 512;

            if ((bitmap.Width > maxSize) || (bitmap.Height > maxSize))
            {
                float xScale = (float)maxSize / (float)bitmap.Width;
                float yScale = (float)maxSize / (float)bitmap.Height;

                float scale = Math.Min(xScale, yScale);

                bitmap = bitmap.Resize(new SKImageInfo((int)(bitmap.Width * scale), (int)(bitmap.Height * scale)), SKFilterQuality.High);
            }

            bitmapDensity = ImageUtil.GetBitmapDensity(bitmap);

            DesiredAspectRatio = (float)bitmap.Width / (float)bitmap.Height;
        }

        public override void Paint(SKRect bounds)
        {
            int alpha = 128;

            DrawDots(10, 128, 10000, 255, bounds);
            DrawDots(100, 32, 10000, 255, bounds);
            DrawDots(1000, 16, 10000, 255, bounds);
            DrawDots(10000, 8, 10000, alpha, bounds);
            DrawDots(100000, 0, 8, alpha, bounds, useTexture: true);
        }

        public void DrawDots(int numDots, float minRadius, float maxRadius, int alpha, SKRect bounds)
        {
            DrawDots(numDots, minRadius, maxRadius, alpha, bounds, useTexture: false);
        }

        public void DrawDots(int numDots, float minRadius, float maxRadius, int alpha, SKRect bounds, bool useTexture)
        {
            Random random = new Random();

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                //StrokeWidth = 2,
            };

            float xScale = bounds.Width / (float)bitmap.Width;
            float yScale = bounds.Height / (float)bitmap.Height;
            float radiusScale = Math.Min(xScale, yScale);

            for (int i = 0; i < numDots; i++)
            {
                float dotX = ((float)random.NextDouble() * (bounds.Width - 1));
                float dotY = ((float)random.NextDouble() * (bounds.Height - 1));

                int px = (int)(dotX / xScale);
                int py = (int)(dotY / yScale);

                SKColor imageColor = bitmap.GetPixel(px, py);

                paint.Color = new SKColor(imageColor.Red, imageColor.Green, imageColor.Blue, (byte)alpha);

                float density = bitmapDensity[px, py];

                if (density > maxRadius)
                    continue;

                if (density < minRadius)
                    density = minRadius;

                //i++;

                float rand = 0.25f;

                float radius = density + (density * (-rand + ((float)random.NextDouble() * rand * 2)));

                if (useTexture)
                {
                    paint.ColorFilter = SKColorFilter.CreateLighting(paint.Color, paint.Color);

                    SKRect destRect = new SKRect(bounds.Left + dotX - (radius * radiusScale), bounds.Top + dotY - (radius * radiusScale),
                        bounds.Left + dotX + (radius * radiusScale), bounds.Top + dotY + (radius * radiusScale));

                    Canvas.DrawBitmap(brushBitmap, destRect, paint);
                }
                else
                {
                    Canvas.DrawCircle(new SKPoint(bounds.Left + dotX, bounds.Top + dotY), radius * radiusScale, paint);
                }
            }
        }
    }
}
