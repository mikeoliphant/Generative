using System;
using System.Numerics;
using SkiaSharp;

namespace Generative
{
    public class TieDye : BoundsPainter
    {
        public float NoiseXOffset { get; set; }
        public float NoiseYOffset { get; set; }
        public float NoiseScale { get; set; }

        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        SKColor[] colors = Palette.Vibrant;
        int numSegments = 200;
        float lineLength = 1f;
        CosinePalette cosinePalette = new CosinePalette(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.00f, 0.33f, 0.67f));

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        public TieDye()
        {
            NoiseXOffset = 0;
            NoiseYOffset = 0;
            NoiseScale = 10;
        }

        public override void Paint(SKRect bounds)
        {
            perlin.Seed = Random.Next(int.MaxValue);

            //noiseOffset = (float)random.NextDouble() * 0.1f;

            //bounds = new SKRect(bounds.Left - (bounds.Width * 0.1f), bounds.Top - (bounds.Height * 0.1f), bounds.Right + (bounds.Width * 0.1f), bounds.Bottom + (bounds.Height * 0.1f));

            paint.Color = new SKColor(0, 0, 0, 5);

            int numLines = 1000;

            float lineWidth = 1.0f / (float)numLines;

            for (int i = 0; i < numLines; i++)
            {
                lineLength = (0.1f + (float)Random.NextDouble()) * 5;

                float lineStart = (float)Random.NextDouble();

                SKColor color = cosinePalette.GetColor(perlin.GetValue(lineStart));

                paint.Color = new SKColor(color.Red, color.Green, color.Blue, 5);

                DrawFlow(lineStart, 0, bounds, lineLength, numSegments);
                DrawFlow(1, lineStart, bounds, lineLength, numSegments);
                DrawFlow(0, lineStart, bounds, lineLength, numSegments);
                DrawFlow(lineStart, 1, bounds, lineLength, numSegments);
            }
        }

        void DrawFlow(float startX, float startY, SKRect bounds, float length, int numSegments)
        {
            SKPath flowPath;

            flowPath = CreateFlowPath(startX, startY, bounds, lineLength, numSegments, 1);
            Canvas.DrawPath(flowPath, paint);

            flowPath = CreateFlowPath(startX, startY, bounds, lineLength, numSegments, -1);
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
