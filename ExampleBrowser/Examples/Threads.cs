using System;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class Threads : BoundsPainter
    {
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
                SKPath threadPath = PathUtils.CreateThreadPath(path, Random, 100, 2, 20);

                float darkness = (float)(Random.NextDouble() * 0.5);

                paint.Color = new SKColor((byte)(darkness * 255), (byte)(darkness * 255), (byte)(darkness * 255));

                Canvas.DrawPath(threadPath, paint);
            }
        }
    }
}
