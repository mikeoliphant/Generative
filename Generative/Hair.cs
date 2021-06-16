using System;
using SkiaSharp;

namespace Generative
{
    public class Hair : BoundsPainter
    {
        Random random = new Random();
        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        float noiseScale = 1;
        SKColor[] colors = Palette.Pastel;

        public override void Paint(SKRect bounds)
        {
            perlin.Seed = (int)(DateTime.Now.Ticks % uint.MaxValue);

            bounds = new SKRect(bounds.Left - (bounds.Width * 0.1f), bounds.Top - (bounds.Height * 0.1f), bounds.Right + (bounds.Width * 0.1f), bounds.Bottom + (bounds.Height * 0.1f));

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,                
                StrokeWidth = 1.5f,
            };

            int numFlows = 20000;
            int numThreads = 10;

            for (int i = 0; i < numFlows; i++)
            {
                float x = (float)random.NextDouble();
                float y = (float)random.NextDouble();

                float length = 0.05f + (float)random.NextDouble() * 0.3f;

                SKPath flowPath = CreateFlowPath(x, y, bounds, length, 20);

                float noise = perlin.GetValue(x * noiseScale, y * noiseScale);

                SKColor baseColor = colors[(int)(((noise + 1) / 2) * colors.Length)];

                for (int t = 0; t < numThreads; t++)
                {
                    SKPath threadPath = CreateThreadPath(flowPath, 20, 0.5f, 50);

                    float darkness = .1f + (float)(random.NextDouble() * 0.75);

                    paint.Color = new SKColor((byte)(darkness * baseColor.Red), (byte)(darkness * baseColor.Green), (byte)(darkness * baseColor.Blue), 100);

                    Canvas.DrawPath(threadPath, paint);
                }
            }
        }

        public SKPath CreateFlowPath(float startX, float startY, SKRect bounds, float length, int numSegments)
        {
            float segmentLength = length / (float)numSegments;

            float x = startX;
            float y = startY;

            SKPath path = new SKPath();
            path.MoveTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));

            for (int i = 0; i < numSegments; i++)
            {
                float noise = perlin.GetValue(x * noiseScale, y * noiseScale);

                float angle = noise * (float)Math.PI * 2;

                SKPoint direction = GetAngleUnitVector(angle);

                x += direction.X * segmentLength;
                y += direction.Y * segmentLength;

                path.LineTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));
            }

            return path;
        }

        SKPath CreateThreadPath(SKPath basePath, int numPoints, float deltaDev, float maxDev)
        {
            SKPath threadPath = new SKPath();

            SKPathMeasure measure = new SKPathMeasure(basePath);

            float pathDelta = measure.Length / (float)numPoints;

            float pathDistance = 0;
            float tangentDev = -maxDev + (float)(random.NextDouble() * maxDev * 2);

            for (int i = 0; i <= numPoints; i++)
            {
                SKPoint pathPoint = measure.GetPosition(pathDistance);
                SKPoint pathTangent = measure.GetTangent(pathDistance);

                tangentDev += -deltaDev + (float)(random.NextDouble() * deltaDev * 2);

                if (tangentDev > maxDev)
                    tangentDev = maxDev;
                else if (tangentDev < -maxDev)
                    tangentDev = -maxDev;

                pathPoint = new SKPoint(pathPoint.X + (-pathTangent.Y * tangentDev), pathPoint.Y + (pathTangent.X * tangentDev));

                if (i == 0)
                    threadPath.MoveTo(pathPoint);
                else
                    threadPath.LineTo(pathPoint);

                pathDistance += pathDelta;
            }

            return threadPath;
        }


        public static SKPoint GetAngleUnitVector(float angle)
        {
            return new SKPoint((float)Math.Cos(angle), -(float)Math.Sin(angle));
        }
    }
}
