using System;
using System.IO;
using System.Net;
using System.Reflection;
using SkiaSharp;

namespace Generative
{
    public static class ImageUtil
    {
        public static SKBitmap BitmapFromURL(string url)
        {
            SKBitmap bitmap = null;

            using (WebClient webClient = new WebClient())
            {
                bitmap = SKBitmap.Decode(webClient.DownloadData(url));
            }

            return bitmap;
        }

        public static SKBitmap BitmapFromResource(string resourceID)
        {
            SKBitmap bitmap = null;

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                bitmap = SKBitmap.Decode(stream);
            }

            return bitmap;
        }

        public static float[,] GetBitmapDensity(SKBitmap bitmap)
        {
            float[,] densityData = new float[bitmap.Width, bitmap.Height];

            Random random = new Random();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    SKColor pixelColor = bitmap.GetPixel(x, y);

                    float ph;
                    float ps;
                    float pv;

                    pixelColor.ToHsv(out ph, out ps, out pv);

                    ph /= 360;
                    ps /= 100;
                    pv /= 100;

                    for (int size = 1; size < 32; size *= 2)
                    {
                        float totErr = 0;

                        int numSamples = (size * size) / 2;

                        if (numSamples > 100)
                            numSamples = 100;

                        float maxErr = numSamples * 0.001f;

                        for (int i = 0; i < numSamples; i++)
                        {
                            int px = x - size + random.Next((size * 2) + 1);
                            int py = y - size + random.Next((size * 2) + 1);

                            if ((px >= 0) && (px < bitmap.Width) && (py >= 0) && (py < bitmap.Height))
                            {
                                SKColor color = bitmap.GetPixel(px, py);

                                float h;
                                float s;
                                float v;

                                color.ToHsv(out h, out s, out v);

                                h /= 360;
                                s /= 100;
                                v /= 100;

                                float hErr = Math.Abs(h - ph);
                                if (hErr > 0.5f)
                                    hErr -= 0.5f;

                                float sErr = Math.Abs(s - ps);

                                float vErr = Math.Abs(v - pv);

                                float err = (hErr + sErr + vErr) / 3;

                                totErr += err;


                                if (totErr > maxErr)
                                    break;
                            }
                        }

                        densityData[x, y] = size;

                        if (totErr > maxErr)
                            break;
                    }
                }
            }

            return densityData;
        }
    }
}
