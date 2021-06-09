using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Generative
{
    public class StainedGlass : BoundsPainter
    {
		SKColor[] colors = new SKColor[] { SKColors.Yellow, SKColors.Red, SKColors.Green, SKColors.Blue };

		public override void Paint(SKRect bounds)
		{
			SKRect drawRect = new SKRect(bounds.Left + (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.1f), bounds.Left + (bounds.Width * 0.9f), bounds.Height * 0.9f);

			DoPaint(drawRect);

			drawRect = new SKRect(bounds.Left + (bounds.Width * 0.11f), bounds.Top + (bounds.Height * 0.11f), bounds.Left + (bounds.Width * 0.89f), bounds.Top + (bounds.Height * 0.89f));

			DoPaint(drawRect);
		}

        public void DoPaint(SKRect bounds)
        {
			float minBound = Math.Min(bounds.Width, bounds.Height);

			SKPaint paint = new SKPaint
			{
				Color = SKColors.Black,
				IsAntialias = true,
				Style = SKPaintStyle.Stroke,
				StrokeWidth = minBound / 400,				
				PathEffect = SKPathEffect.CreateCorner(minBound / 80)
			};

			int gridSizeX = 17;
			int gridSizeY = 47;
			float rotInc = 0.2f;

			float gridWidth = bounds.Width / gridSizeX;
			float gridHeight = bounds.Height / gridSizeY;

			float rot = 0;
			int colorIndex = 0;

			float drawRadius = Math.Min(bounds.Width * 0.5f, bounds.Height * 0.5f);

			for (int gridY = 0; gridY < gridSizeY; gridY++)
			{
				for (int gridX = 0; gridX < gridSizeX; gridX++)
				{
					float x = bounds.Left + (gridX * gridWidth);
					float y = bounds.Top + (gridY * gridHeight);

					SKRect boxRect = new SKRect(x, y, x + gridWidth, y + gridHeight);

					float xDist = boxRect.MidX - bounds.MidX;
					float yDist = boxRect.MidY - bounds.MidY;

					if (Math.Sqrt((xDist * xDist) + (yDist * yDist)) > drawRadius)
						continue;

					paint.Style = SKPaintStyle.StrokeAndFill;
					paint.Color = new SKColor(colors[colorIndex].Red, colors[colorIndex].Green, colors[colorIndex].Blue, 128);

					DrawRect(boxRect, paint, rot);

					paint.Style = SKPaintStyle.Stroke;
					paint.Color = SKColors.Black;

					DrawRect(boxRect, paint, rot);

					rot += rotInc;
					colorIndex = (colorIndex + 1) % colors.Length;
				}
			}
		}

		void DrawRect(SKRect rect, SKPaint paint, float rot)
		{
			Canvas.Save();
			Canvas.RotateRadians(rot, rect.MidX, rect.MidY);
			Canvas.DrawRect(rect, paint);
			Canvas.Restore();
		}
	}
}
