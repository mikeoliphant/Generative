using System;
using System.Numerics;
using System.Threading;
using SkiaSharp;

namespace Generative
{
    public class Clifford : BoundsPainter
    {
        const int Iterations = 10000000;

        const double a = -1.22;// 2;
        const double b = 1.35; //2;
        const double c = -1.25; //1;
        const double d = -1.15; //-1;

        CosinePalette cosinePalette = new CosinePalette(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.00f, 0.10f, 0.20f));

        public Clifford()
        {
        }

        public override void Paint(SKRect bounds)
        {
            Canvas.Clear(SKColors.White);

            int histWidth = (int)bounds.Width;
            int histHeight = (int)bounds.Height;

            int[,] pointHistogram = new int[histWidth + 1, histHeight + 1];
            double[,] deltaHistogram = new double[histWidth + 1, histHeight + 1];

            double xScale = 0.2 * histWidth;
            double yScale = 0.2 * histHeight;

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

                        SKColor color = cosinePalette.GetColor(0.3f + ((float)(deltaHistogram[xPos, yPos] / maxDelta) * 0.3f));

                        Canvas.DrawPoint(new SKPoint(xPos, yPos), new SKColor(color.Red, color.Green, color.Blue, (byte)(alpha * 255)));
                    }
                }
            }
        }
    }
}