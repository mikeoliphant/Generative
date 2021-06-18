using System;
using SkiaSharp;

namespace Generative
{
    public class Ribbons : BoundsPainter
    {
        public override void Paint(SKRect bounds)
        {
            DrawStripes(bounds, .4f, Palette.Pastel, doOutline: true);
            //DrawStripes(bounds, .7f, Palette.Sunset, doOutline: true);
            //DrawStripes(bounds, 0.15f, Palette.ColdGB, doOutline: true);
        }

        void DrawStripes(SKRect bounds, float sinDelta, SKColor[] colors, bool doOutline)
        {
            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 50,
                StrokeCap = SKStrokeCap.Square
            };

            int numPoints = 50;

            float xOverscan = bounds.Width * 0.25f;

            bounds = new SKRect(bounds.Left - xOverscan, bounds.Top, bounds.Right + xOverscan, bounds.Bottom);

            float xDelta = bounds.Width / (float)numPoints;

            float x = bounds.Left;
            float sinOffset = 0;

            for (int i = 0; i < numPoints; i++)
            {
                SKPath path = GenerateSinPath(x, bounds.Top, bounds.Bottom, sinOffset, 5, 100);

                if (doOutline)
                {
                    paint.Color = SKColors.Black;
                    paint.StrokeWidth = 55;

                    Canvas.DrawPath(path, paint);

                    paint.StrokeWidth = 50;
                }

                paint.Color = colors[i % colors.Length];

                Canvas.DrawPath(path, paint);

                x += xDelta;
                sinOffset += sinDelta;
            }
        }

        SKPath GenerateSinPath(float startX, float startY, float endY, float sinOffset, float sinYScale, float sinXScale)
        {
            SKPath path = new SKPath();

            int numPoints = 1000;

            float yDelta = (endY - startY) / (float)numPoints;
            float sinDelta = sinYScale / (float)numPoints;

            float y = startY;
            float sinPos = sinOffset;

            path.MoveTo(startX + (float)(Math.Sin(sinPos) * sinXScale), y);

            for (int i = 0; i < numPoints; i++)
            {
                y += yDelta;
                sinPos += sinDelta;

                path.LineTo(startX + (float)(Math.Sin(sinPos) * sinXScale), y);
            }

            return path;
        }
    }
}
