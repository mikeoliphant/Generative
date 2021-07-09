using System;
using SkiaSharp;

namespace Generative
{
    public static class PathUtils
    {
        public static SKPath CreateThreadPath(SKPath basePath, Random random, int numPoints, float deltaDev, float maxDev)
        {
            SKPath threadPath = new SKPath();

            SKPathMeasure measure = new SKPathMeasure(basePath);

            float pathDelta = measure.Length / (float)numPoints;

            float pathDistance = 0;
            float tangentDev = -maxDev + (float)(random.NextDouble() * maxDev * 2);

            for (int i = 0; i <= numPoints; i++)
            {
                SKPoint pathPoint = measure.GetPosition(pathDistance);
                SKPoint pathTangent = measure.GetTangent(pathDistance);

                tangentDev += -deltaDev + (float)(random.NextDouble() * deltaDev * 2);

                if (tangentDev > maxDev)
                    tangentDev = maxDev;
                else if (tangentDev < -maxDev)
                    tangentDev = -maxDev;

                pathPoint = new SKPoint(pathPoint.X + (-pathTangent.Y * tangentDev), pathPoint.Y + (pathTangent.X * tangentDev));

                if (i == 0)
                    threadPath.MoveTo(pathPoint);
                else
                    threadPath.LineTo(pathPoint);

                pathDistance += pathDelta;
            }

            return threadPath;
        }
    }
}
