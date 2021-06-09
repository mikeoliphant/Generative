using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Generative
{
    public class BoundsPainter
    {
        public SKCanvas Canvas { get; private set; }

        public void SetCanvas(SKCanvas canvas)
        {
            this.Canvas = canvas;
        }

        public virtual void Paint(SKRect bounds)
        {
        }
    }
}
