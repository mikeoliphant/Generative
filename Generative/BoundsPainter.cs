using System;
using System.IO;
using SkiaSharp;

namespace Generative
{
    public class BoundsPainter
    {
        public SKCanvas Canvas { get; private set; }
        public Random Random { get; set; }

        int randomSeed;
        public int RandomSeed
        {
            get => randomSeed;
            set
            {
                randomSeed = value;
                Random = new Random(randomSeed);
            }
        }

        public BoundsPainter()
        {
            RandomSeed = (int)(DateTime.Now.Ticks % uint.MaxValue);
            Random = new Random(RandomSeed);
        }

        public void SetCanvas(SKCanvas canvas)
        {
            this.Canvas = canvas;
        }

        public virtual void Paint(SKRect bounds)
        {
        }

        public void SavePng(string savePath, int width, int height)
        {
            SKBitmap bitmap = new SKBitmap(width, height);

            SKCanvas pngCanvas = new SKCanvas(bitmap);

            pngCanvas.Clear(SKColors.White);

            SetCanvas(pngCanvas);
            Paint(new SKRect(0, 0, width, height));

            pngCanvas.Flush();

            SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 80);

            using (var stream = File.Create(savePath))
            {
                // save the data to a stream
                data.SaveTo(stream);
            }
        }
    }
}
