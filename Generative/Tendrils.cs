using System;
using SkiaSharp;

namespace Generative
{
    public class Tendrils : BoundsPainter
    {
        public float NoiseXOffset { get; set; }
        public float NoiseYOffset { get; set; }
        public float NoiseScale { get; set; }

        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        SKColor[] colors = Palette.Vibrant;
        int numSegments = 200;
        float lineLength = 2.0f;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        SKPaint outlinePaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 50),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        public Tendrils()
        {
            NoiseXOffset = 0;
            NoiseYOffset = 0;
            NoiseScale = 1;
        }

        public override void Paint(SKRect bounds)
        {
            Canvas.Clear(new SKColor(50, 50, 100));
            //Canvas.Clear(new SKColor(80, 120, 200));

            perlin.Seed = Random.Next(int.MaxValue);

            //noiseOffset = (float)random.NextDouble() * 0.1f;

            //bounds = new SKRect(bounds.Left - (bounds.Width * 0.1f), bounds.Top - (bounds.Height * 0.1f), bounds.Right + (bounds.Width * 0.1f), bounds.Bottom + (bounds.Height * 0.1f));

            int numLines = 50;

            float lineWidth = 1.0f / (float)numLines;
            float lineStart = 0;

            paint.StrokeWidth = 6; // bounds.Width / (float)numLines;
            outlinePaint.StrokeWidth = 8;

            for (int i = 0; i < numLines; i++)
            {
                paint.Color = colors[i % colors.Length];

                DrawFlow(lineStart, 0, bounds, lineLength, numSegments);
                DrawFlow(1, lineStart, bounds, lineLength, numSegments);
                DrawFlow(0, lineStart, bounds, lineLength, numSegments);
                DrawFlow(lineStart, 1, bounds, lineLength, numSegments);

                lineStart += lineWidth;
            }
        }

        void DrawFlow(float startX, float startY, SKRect bounds, float length, int numSegments)
        {
            SKPath flowPath;

            flowPath = CreateFlowPath(startX, startY, bounds, lineLength, numSegments, 1);
            Canvas.DrawPath(flowPath, outlinePaint);
            Canvas.DrawPath(flowPath, paint);

            flowPath = CreateFlowPath(startX, startY, bounds, lineLength, numSegments, -1);
            Canvas.DrawPath(flowPath, outlinePaint);
            Canvas.DrawPath(flowPath, paint);
        }

        SKPath CreateFlowPath(float startX, float startY, SKRect bounds, float length, int numSegments, int flowDirection)
        {
            float segmentLength = length / (float)numSegments;

            float x = startX;
            float y = startY;

            SKPath path = new SKPath();
            path.MoveTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));

            for (int i = 0; i < numSegments; i++)
            {
                float noise = perlin.GetValue((x + NoiseXOffset) * NoiseScale, (y + NoiseYOffset) * NoiseScale);

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

        static SKPoint GetAngleUnitVector(float angle)
        {
            return new SKPoint((float)Math.Cos(angle), -(float)Math.Sin(angle));
        }
    }
}
