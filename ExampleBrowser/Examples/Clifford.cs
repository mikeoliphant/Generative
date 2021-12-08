using System;
using System.Numerics;
using System.Threading;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class Clifford : BoundsPainter
    {
        int Iterations = 10000000;

        double a = -1.22;// 2;
        double b = 1.35; //2;
        double c = -1.25; //1;
        double d = -1.15; //-1;

        GradientPalette palette = GradientPalette.RedYellowGreen;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = false
        };

        SKColor backgroundColor = SKColors.White;

        double scale = 0.2f;

        public static Clifford Jellyfish { get { return new Clifford(-1.22, 1.35, -1.25, -1.15, 0.2, GradientPalette.RedYellowGreenDark, new SKColor(255, 250, 245)); } }
        public static Clifford Marble { get { return new Clifford(2, 2, 1, -1, 0.2, GradientPalette.RedYellowGreen, SKColors.Black); } }
        public static Clifford Basket { get { return new Clifford(1.7, 1.7, 0.6, 1.2, 0.2, GradientPalette.RedYellowGreenDark, SKColors.White); } }
        public static Clifford CircleSwirl { get { return new Clifford(-1.24, -1.25, -1.82, -1.91, 0.15, GradientPalette.RedYellowGreenDark, SKColors.White); } }

        public Clifford()
        {
        }

        public Clifford(double a, double b, double c, double d, double scale,  GradientPalette palette, SKColor backgroundColor)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            this.scale = scale;
            this.palette = palette;
            this.backgroundColor = backgroundColor;
        }

        public override void Paint(SKRect bounds)
        {
            Canvas.Clear(backgroundColor);

            int histWidth = (int)bounds.Width;
            int histHeight = (int)bounds.Height;

            int[,] pointHistogram = new int[histWidth + 1, histHeight + 1];
            double[,] deltaHistogram = new double[histWidth + 1, histHeight + 1];

            double xScale = scale * histWidth;
            double yScale = scale * histHeight;

            int xOffset = histWidth / 2;
            int yOffset = histHeight / 2;

            int numThreads = System.Environment.ProcessorCount;

            Thread[] threads = new Thread[numThreads];

            for (int thread = 0; thread < numThreads; thread++)
            {
                threads[thread] = new Thread(new ThreadStart(delegate
                {
                    double x = Random.NextDouble();
                    double y = Random.NextDouble();

                    for (int i = 0; i < Iterations; i++)
                    {
                        double xNext = Math.Sin(a * y) + (c * Math.Cos(a * x));
                        double yNext = Math.Sin(b * x) + (d * Math.Cos(b * y));

                        int xPos = (int)((xNext * xScale) + xOffset);
                        int yPos = (int)((yNext * yScale) + yOffset);

                        pointHistogram[xPos, yPos]++;

                        double delta = Math.Sqrt(((xNext - x) * (xNext - x)) + ((yNext - y) * (yNext - y)));

                        deltaHistogram[xPos, yPos] += delta;

                        x = xNext;
                        y = yNext;
                    }
                }));

                threads[thread].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            float maxHist = 0;
            double maxDelta = 0;

            for (int xPos = 0; xPos < histWidth; xPos++)
            {
                for (int yPos = 0; yPos < histHeight; yPos++)
                {
                    if (pointHistogram[xPos, yPos] > 0)
                    {
                        deltaHistogram[xPos, yPos] /= (double)pointHistogram[xPos, yPos];

                        maxHist = Math.Max(maxHist, pointHistogram[xPos, yPos]);
                        maxDelta = Math.Max(maxDelta, deltaHistogram[xPos, yPos]);
                    }
                }
            }

            for (int xPos = 0; xPos < histWidth; xPos++)
            {
                for (int yPos = 0; yPos < histHeight; yPos++)
                {
                    int val = pointHistogram[xPos, yPos];

                    if (val > 0)
                    {
                        float alpha = val / maxHist;

                        alpha = (float)Math.Pow(alpha, 0.25);

                        paint.Color = (palette.GetGradientValue((float)(deltaHistogram[xPos, yPos] / maxDelta))).ToSKColor().WithAlpha((byte)(alpha * 255));

                        Canvas.DrawPoint(new SKPoint(xPos, yPos), paint);
                    }
                }
            }
        }
    }
}