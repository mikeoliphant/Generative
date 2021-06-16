using System;
using SkiaSharp;

namespace Generative
{
    public class Threads : BoundsPainter
    {
        Random random = new Random();

        public override void Paint(SKRect bounds)
        {
            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f
            };

            SKPath path = new SKPath();

            path.MoveTo(bounds.Left, bounds.Top);
            path.LineTo(bounds.Right, bounds.Bottom);

            int numThreads = 100;

            for (int i = 0; i < numThreads; i++)
            {
                SKPath threadPath = CreateThreadPath(path, 100, 2, 20);

                float darkness = (float)(random.NextDouble() * 0.5);

                paint.Color = new SKColor((byte)(darkness * 255), (byte)(darkness * 255), (byte)(darkness * 255));

                Canvas.DrawPath(threadPath, paint);
            }
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
    }
}
