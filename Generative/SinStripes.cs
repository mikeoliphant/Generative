using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Generative
{
    public class SinStripes : BoundsPainter
    {
        SKColor[] colors = new SKColor[] { SKColor.Parse("#220022"), SKColor.Parse("#334455"), SKColor.Parse("#779988"), SKColor.Parse("#ffffdd") };

        public override void Paint(SKRect bounds)
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
            float sinDelta = 0.12f;

            for (int i = 0; i < numPoints; i++)
            {
                SKPath path = GenerateSinPath(x, bounds.Top, bounds.Bottom, sinOffset, 5, 100);

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
