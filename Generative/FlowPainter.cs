using System;
using SkiaSharp;

namespace Generative
{
    public class FlowPainter : BoundsPainter
    {
        LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin();
        float noiseScale = 1;
        SKBitmap colorBitmap;
        float[,] bitmapDensity;

        public FlowPainter()
        {
            //colorBitmap = ImageUtil.BitmapFromURL("https://pbs.twimg.com/profile_images/1474838354/KungFu128x128_400x400.png");
            //colorBitmap = ImageUtil.BitmapFromURL("https://media.istockphoto.com/photos/beautiful-sunset-over-the-tropical-sea-picture-id1172427455?k=6&m=1172427455&s=612x612&w=0&h=yGHpsUyZTE15UPI2pRTkDUnZzLAciyV6Sl2Ieh2QMW4=");
            //colorBitmap = ImageUtil.BitmapFromURL("https://i2.wp.com/digital-photography-school.com/wp-content/uploads/2021/03/sunset-photography-tips-1.jpg?resize=1500%2C1000&ssl=1");
            //colorBitmap = ImageUtil.BitmapFromURL("https://cdnb.artstation.com/p/assets/images/images/018/511/569/large/ben-j-pastoral.jpg?1559660489");
            //colorBitmap = ImageUtil.BitmapFromURL("https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/2d895df5-10aa-4a1a-884e-3381a18ea74d/dbzzfgu-feb671d5-472c-4227-aefa-6dea00bb5b23.jpg/v1/fill/w_1024,h_765,q_75,strp/pastoral_scene_by_virtuella_dbzzfgu-fullview.jpg?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOiIsImlzcyI6InVybjphcHA6Iiwib2JqIjpbW3sicGF0aCI6IlwvZlwvMmQ4OTVkZjUtMTBhYS00YTFhLTg4NGUtMzM4MWExOGVhNzRkXC9kYnp6Zmd1LWZlYjY3MWQ1LTQ3MmMtNDIyNy1hZWZhLTZkZWEwMGJiNWIyMy5qcGciLCJoZWlnaHQiOiI8PTc2NSIsIndpZHRoIjoiPD0xMDI0In1dXSwiYXVkIjpbInVybjpzZXJ2aWNlOmltYWdlLndhdGVybWFyayJdLCJ3bWsiOnsicGF0aCI6Ilwvd21cLzJkODk1ZGY1LTEwYWEtNGExYS04ODRlLTMzODFhMThlYTc0ZFwvdmlydHVlbGxhLTQucG5nIiwib3BhY2l0eSI6OTUsInByb3BvcnRpb25zIjowLjQ1LCJncmF2aXR5IjoiY2VudGVyIn19.SXfvyiAEMbAx7VS6HJGX_a7XCxieSps8RjcW2ge-3LE");
            //colorBitmap = ImageUtil.BitmapFromURL("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3CtwmXO0tnD7_J_5mA7XPd0ZyOM_mxHSPFw&usqp=CAU");
            colorBitmap = ImageUtil.BitmapFromURL(@"c:\tmp\src.png");

            int maxSize = 512;

            if ((colorBitmap.Width > maxSize) || (colorBitmap.Height > maxSize))
            {
                float xScale = (float)maxSize / (float)colorBitmap.Width;
                float yScale = (float)maxSize / (float)colorBitmap.Height;

                float scale = Math.Min(xScale, yScale);

                colorBitmap = colorBitmap.Resize(new SKImageInfo((int)(colorBitmap.Width * scale), (int)(colorBitmap.Height * scale)), SKFilterQuality.High);
            }

            bitmapDensity = ImageUtil.GetBitmapDensity(colorBitmap);

            DesiredAspectRatio = (float)colorBitmap.Width / (float)colorBitmap.Height;
        }

        
        public override void Paint(SKRect bounds)
        {
            perlin.Seed = Random.Next(int.MaxValue);

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round
            };

            int numFlows = 100000;

            for (int i = 0; i < numFlows; i++)
            {
                float x = (float)Random.NextDouble();
                float y = (float)Random.NextDouble();

                int px = (int)(colorBitmap.Width * x);
                int py = (int)(colorBitmap.Height * y);

                float density = bitmapDensity[px, py];

                //float maxLength = 0.05f + (float)Random.NextDouble() * 0.3f;
                float maxLength = 0.005f * density;

                SKPath flowPath = CreateFlowPath(x, y, bounds, maxLength, 0.5f, 20);

                float noise = perlin.GetValue(x * noiseScale, y * noiseScale);

                SKColor baseColor = colorBitmap.GetPixel(px, py);

                float darkness = .75f + (float)(Random.NextDouble() * 0.2);

                paint.Color = baseColor;

                paint.StrokeWidth = 5 + ((float)Random.NextDouble() * 5);

                Canvas.DrawPath(flowPath, paint);
            }
        }

        SKPath CreateFlowPath(float startX, float startY, SKRect bounds, float maxLength, float maxCurve, int numSegments)
        {
            float segmentLength = maxLength / (float)numSegments;

            float x = startX;
            float y = startY;

            SKPath path = new SKPath();
            path.MoveTo(bounds.Left + (x * bounds.Width), bounds.Top + (y * bounds.Height));

            float startAngle = 0;

            for (int i = 0; i < numSegments; i++)
            {
                float noise = perlin.GetValue(x * noiseScale, y * noiseScale);

                float angle = noise * (float)Math.PI * 2;

                if (i == 0)
                {
                    startAngle = angle;
                }
                else
                {
                    float angleDeviation = Math.Abs(startAngle - angle) % (float)(Math.PI * 2);

                    if (angleDeviation > maxCurve)
                        return path;
                }

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