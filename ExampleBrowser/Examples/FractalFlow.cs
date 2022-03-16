using System;
using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class FractalFlow : BoundsPainter
    {
        public override IEnumerable<bool> ProgressivePaint(SKRect bounds)
        {
            bounds.Inflate(new SKSize(bounds.Width * .1f, bounds.Height * .1f));

            Canvas.Clear(SKColors.Black);

            double fractX = 0;
            double fractY = .5;
            double fractWidth = .45;
            double fractHeight = .2;

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
            };

            int numPoints = 10000;

            for (int i = 0; i < numPoints; i++)
            {
                if ((i % 100) == 0)
                    yield return true;

                double x = Random.NextDouble();
                double y = Random.NextDouble();

                double vx = 0;
                double vy = 0;

                double value;
                Complex complex;

                paint.StrokeWidth = 5;

                for (int draw = 0; draw < 50; draw++)
                {
                    if (GetMandelbrot(fractX + (x * fractWidth), fractY + (y * fractHeight), out value, out complex))
                    {
                        if (draw == 0)
                            paint.Color = SKColor.FromHsv((float)(value * 360), 100, 100);

                        complex /= complex.Magnitude;

                        //Canvas.DrawPoint((float)(bounds.Left + (x * bounds.Width)), (float)(bounds.Top + (y * bounds.Height)), paint);

                        vx += (complex.Real * (fractWidth / 100));
                        vy += (complex.Imaginary * (fractHeight / 100));
                    }

                    if (vx == 0)
                        break;

                    double nx = x + vx;
                    double ny = y + vy;

                    Canvas.DrawLine(bounds.Left + ((float)x * bounds.Width), bounds.Top + ((float)y * bounds.Height), bounds.Left + ((float)nx * bounds.Width), bounds.Top + ((float)ny * bounds.Height), paint);

                    x = nx;
                    y = ny;

                    vx /= 1.2;
                    vy /= 1.2;

                    paint.StrokeWidth *= .9f;
                }
            }
        }

        static int NumIterations = 100;

        static bool GetMandelbrot(double x, double y, out double value, out Complex complex)
        {
            Complex start = new Complex(x, y);
            Complex current = new Complex(0, 0);

            for (int i = 0; i < NumIterations; i++)
            {
                current = current * current + start;

                double sqrLength = (current.Real * current.Real) + (current.Imaginary * current.Imaginary);

                if (sqrLength > 2)
                {
                    value = (i - Math.Log(Math.Log(current.Magnitude / Math.Log(2)))) / (double)NumIterations;
                    complex = current;

                    return true;
                }
            }

            value = 0;
            complex = Complex.Zero;

            return false;
        }
    }
}
