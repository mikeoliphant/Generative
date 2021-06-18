using System;
using SkiaSharp;

namespace Generative
{
    public class Squiggles : BoundsPainter
    {
        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        float noiseScale = 1;
        SKColor[] colors = Palette.Pastel;

        public override void Paint(SKRect bounds)
        {
            perlin.Seed = (int)(DateTime.Now.Ticks % uint.MaxValue);

            //bounds = new SKRect(bounds.Left - (bounds.Width * 0.1f), bounds.Top - (bounds.Height * 0.1f), bounds.Right + (bounds.Width * 0.1f), bounds.Bottom + (bounds.Height * 0.1f));

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            int numLines = 50;

            float lineWidth = 1.0f / (float)numLines;
            float lineStart = 0;

            paint.StrokeWidth = 5; // bounds.Width / (float)numLines;

            for (int i = 0; i < numLines; i++)
            {
                paint.Color = colors[i % colors.Length];

                SKPath flowPath;

                int numSegments = 200;
                float lineLength = 2.0f;

                flowPath = CreateFlowPath(lineStart, 0, bounds, lineLength, numSegments, 1);
                Canvas.DrawPath(flowPath, paint);
                flowPath = CreateFlowPath(lineStart, 0, bounds, lineLength, numSegments, -1);
                Canvas.DrawPath(flowPath, paint);

                flowPath = CreateFlowPath(1, lineStart, bounds, lineLength, numSegments, 1);
                Canvas.DrawPath(flowPath, paint);
                flowPath = CreateFlowPath(1, lineStart, bounds, lineLength, numSegments, -1);
                Canvas.DrawPath(flowPath, paint);

                flowPath = CreateFlowPath(0, lineStart, bounds, lineLength, numSegments, 1);
                Canvas.DrawPath(flowPath, paint);
                flowPath = CreateFlowPath(0, lineStart, bounds, lineLength, numSegments, -1);
                Canvas.DrawPath(flowPath, paint);

                flowPath = CreateFlowPath(lineStart, 1, bounds, lineLength, numSegments, 1);
                Canvas.DrawPath(flowPath, paint);
                flowPath = CreateFlowPath(lineStart, 1, bounds, lineLength, numSegments, -1);
                Canvas.DrawPath(flowPath, paint);

                lineStart += lineWidth;
            }
        }

        public SKPath CreateFlowPath(float startX, float startY, SKRect bounds, float length, int numSegments, int flowDirection)
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

                if (flowDirection == -1)
                    angle -= (float)Math.PI;

                SKPoint direction = GetAngleUnitVector(angle);

                x += direction.X * segmentLength;
                y += direction.Y * segmentLength;

                path.LineTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));
            }

            return path;
        }

        public static SKPoint GetAngleUnitVector(float angle)
        {
            return new SKPoint((float)Math.Cos(angle), -(float)Math.Sin(angle));
        }
    }
}
