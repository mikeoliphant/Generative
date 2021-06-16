using System;
using SkiaSharp;

namespace Generative
{
    public class Impressionist : BoundsPainter
    {
        SKBitmap bitmap;
        float[,] bitmapDensity;
        SKBitmap brushBitmap;

        public Impressionist()
        {
            //bitmap = SKBitmap.Decode(@"C:\Users\oliph\Desktop\tmp\Mike.png");
            //bitmap = SKBitmap.Decode(@"C:\Share\OldCode\KungFuFight\Marketplace\Icons\Icon512x512.png");
            bitmap = SKBitmap.Decode(@"C:\Share\OldCode\BlockZombies\Marketplace\Promo\Icon512x512.png");

            brushBitmap = SKBitmap.Decode(@"C:\tmp\Brush.png");

            int maxSize = 512;

            if ((bitmap.Width > maxSize) || (bitmap.Height > maxSize))
            {
                float xScale = (float)maxSize / (float)bitmap.Width;
                float yScale = (float)maxSize / (float)bitmap.Height;

                float scale = Math.Min(xScale, yScale);

                bitmap = bitmap.Resize(new SKImageInfo((int)(bitmap.Width * scale), (int)(bitmap.Height * scale)), SKFilterQuality.High);
            }

            bitmapDensity = GetBitmapDensity(bitmap);
        }

        public override void Paint(SKRect bounds)
        {
            int alpha = 128;

            DrawDots(10, 128, 10000, 255, bounds);
            DrawDots(100, 32, 10000, 255, bounds);
            DrawDots(1000, 16, 10000, 255, bounds);
            DrawDots(10000, 8, 10000, alpha, bounds);
            DrawDots(100000, 0, 8, alpha, bounds, useTexture:true);
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

        static float[,] GetBitmapDensity(SKBitmap bitmap)
        {
            float[,] densityData = new float[bitmap.Width, bitmap.Height];

            Random random = new Random();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    SKColor pixelColor = bitmap.GetPixel(x, y);

                    float ph;
                    float ps;
                    float pv;

                    pixelColor.ToHsv(out ph, out ps, out pv);

                    ph /= 360;
                    ps /= 100;
                    pv /= 100;

                    for (int size = 1; size < 32; size *= 2)
                    {
                        float totErr = 0;

                        int numSamples = (size * size) / 2;

                        if (numSamples > 100)
                            numSamples = 100;

                        float maxErr = numSamples * 0.001f;

                        for (int i = 0; i < numSamples; i++)
                        {
                            int px = x - size + random.Next((size * 2) + 1);
                            int py = y - size + random.Next((size * 2) + 1);

                            if ((px >= 0) && (px < bitmap.Width) && (py >= 0) && (py < bitmap.Height))
                            {
                                SKColor color = bitmap.GetPixel(px, py);

                                float h;
                                float s;
                                float v;

                                color.ToHsv(out h, out s, out v);

                                h /= 360;
                                s /= 100;
                                v /= 100;

                                float hErr = Math.Abs(h - ph);
                                if (hErr > 0.5f)
                                    hErr -= 0.5f;

                                float sErr = Math.Abs(s - ps);

                                float vErr = Math.Abs(v - pv);

                                float err = (hErr + sErr + vErr) / 3;

                                totErr += err;


                                if (totErr > maxErr)
                                    break;
                            }
                        }

                        densityData[x, y] = size;

                        if (totErr > maxErr)
                            break;
                    }
                }
            }

            return densityData;
        }

    }
}
