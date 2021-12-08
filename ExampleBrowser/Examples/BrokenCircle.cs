using System;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class BrokenCircle : BoundsPainter
    {
        public override void Paint(SKRect bounds)
        {
            SKColor[] colors = Palette.Pastel;

            Canvas.Clear(SKColors.AntiqueWhite);

            int numRings = 35;

            float outerRadius = bounds.Height * 0.4f;
            float innerRadius = bounds.Height * 0.1f;

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = ((outerRadius - innerRadius) / (float)numRings) + 1,
                StrokeCap = SKStrokeCap.Square
            };

            float radiusDec = (outerRadius - innerRadius) / (float)numRings;

            float radius = outerRadius;

            float minRetract = 5;
            float randRetract = 50;

            for (int i = 0; i < numRings; i++)
            {
                paint.Color = colors[i % colors.Length];

                float upperLeftRetract = minRetract + (float)Random.NextDouble() * randRetract;
                float upperRightRetract = minRetract + (float)Random.NextDouble() * randRetract;
                float lowerLeftRetract = minRetract + (float)Random.NextDouble() * randRetract;
                float lowerRightRetract = minRetract + (float)Random.NextDouble() * randRetract;

                Canvas.DrawArc(new SKRect(bounds.MidX - radius, bounds.MidY - radius, bounds.MidX + radius, bounds.MidY + radius),
                    lowerRightRetract, 180 - lowerRightRetract - lowerLeftRetract, false, paint);

                Canvas.DrawArc(new SKRect(bounds.MidX - radius, bounds.MidY - radius, bounds.MidX + radius, bounds.MidY + radius),
                    180 + upperLeftRetract, 180 - upperLeftRetract - upperRightRetract, false, paint);

                radius -= radiusDec;
            }
        }
    }
}
