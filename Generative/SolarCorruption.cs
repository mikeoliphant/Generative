using System;
using System.Numerics;
using SkiaSharp;

namespace Generative
{
    public class SolarCorruption : BoundsPainter
    {
        public float NoiseXOffset { get; set; }
        public float NoiseYOffset { get; set; }
        public float NoiseScale { get; set; }

        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        SKColor[] colors = Palette.Vibrant;
        CosinePalette cosinePalette = new CosinePalette(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.00f, 0.33f, 0.67f));
        byte colorAlpha = 20;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black, // SKColors.Orange,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        SKPaint circlePaint = new SKPaint
        {
            Color = new SKColor(255, 255, 245),
            IsAntialias = true,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeWidth = 1
        };

        public SolarCorruption()
        {
            NoiseXOffset = 0;
            NoiseYOffset = 0;
            NoiseScale = 10;
        }

        public override void Paint(SKRect bounds)
        {
            //Canvas.Clear(new SKColor(250, 250, 250));

            float size = Math.Min(bounds.Width, bounds.Height);

            float left = bounds.Left + ((bounds.Width - size) / 2);
            float top = bounds.Top + ((bounds.Height - size) / 2);

            bounds = new SKRect(left, top, left + size, top + size);

            perlin.Seed = Random.Next(int.MaxValue);

            float radiusPercent = 0.35f;

            SKPath path = new SKPath();
            path.AddCircle(0.5f, 0.5f, radiusPercent);

            Canvas.DrawCircle(bounds.MidX, bounds.MidY, size * radiusPercent, circlePaint);

            SKPathMeasure pathMeasure = new SKPathMeasure(path);

            paint.Color = new SKColor(paint.Color.Red, paint.Color.Green, paint.Color.Blue, colorAlpha);

            int numLines = 5000;

            float pathDelta = pathMeasure.Length / (float)numLines;
            float pathDistance = 0;

            for (int i = 0; i < numLines; i++)
            {
                //SKColor color = cosinePalette.GetColor(perlin.GetValue(pathDistance * 100));

                //paint.Color = new SKColor(color.Red, color.Green, color.Blue, colorAlpha);

                SKPoint point = pathMeasure.GetPosition(pathDistance);

                DrawFlow(point.X, point.Y, bounds, 0.2f, 200);

                pathDistance += pathDelta;
            }
        }

        void DrawFlow(float startX, float startY, SKRect bounds, float length, int numSegments)
        {
            SKPath flowPath;

            flowPath = CreateFlowPath(startX, startY, bounds, length, numSegments, 1);
            Canvas.DrawPath(flowPath, paint);

            flowPath = CreateFlowPath(startX, startY, bounds, length, numSegments, -1);
            Canvas.DrawPath(flowPath, paint);
        }

        SKPath CreateFlowPath(float startX, float startY, SKRect bounds, float length, int numSegments, int flowDirection)
        {
            float segmentLength = length / (float)numSegments;

            float x = startX;
            float y = startY;

            SKPath path = new SKPath();
            path.MoveTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));

            float angleRandom = .5f;
            float halfAngleRandom = angleRandom / 2;

            for (int i = 0; i < numSegments; i++)
            {
                float noise = perlin.GetValue((x + NoiseXOffset) * NoiseScale, (y + NoiseYOffset) * NoiseScale);

                float angle = (noise * (float)Math.PI * 2) + (-halfAngleRandom + (float)(Random.NextDouble() * angleRandom));

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
