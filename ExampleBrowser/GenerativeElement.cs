using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Generative;

namespace ExampleBrowser
{
    [DefaultEvent("PaintSurface")]
    [DefaultProperty("Name")]
    public class GenerativeElement : FrameworkElement
    {
        private const double BitmapDpi = 96.0;

        private readonly bool designMode;

        private WriteableBitmap bitmap;
        private bool ignorePixelScaling;

        GenerativeExample example;
        IEnumerator<bool> paintEnumerator = null;
        bool needRePaint = false;

        public GenerativeElement()
        {
            designMode = DesignerProperties.GetIsInDesignMode(this);
        }

        public SKSize CanvasSize { get; private set; }

        public bool IgnorePixelScaling
        {
            get => ignorePixelScaling;
            set
            {
                ignorePixelScaling = value;

                RePaint();
            }
        }

        public GenerativeExample Example
        {
            get => this.example;
            set
            {
                this.example = value;

                double desiredWidth = example.Value.DesiredAspectRatio * Height;

                if (Width != desiredWidth)
                {
                    Width = desiredWidth;
                }

                RePaint();
            }
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (designMode)
                return;

            if (Visibility != Visibility.Visible || PresentationSource.FromVisual(this) == null)
                return;

            var size = CreateSize(out var unscaledSize, out var scaleX, out var scaleY);
            var userVisibleSize = IgnorePixelScaling ? unscaledSize : size;

            CanvasSize = userVisibleSize;

            if (size.Width <= 0 || size.Height <= 0)
                return;

            var info = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            // reset the bitmap if the size has changed
            if (bitmap == null || info.Width != bitmap.PixelWidth || info.Height != bitmap.PixelHeight)
            {
                needRePaint = true;

                bitmap = new WriteableBitmap(info.Width, size.Height, BitmapDpi * scaleX, BitmapDpi * scaleY, PixelFormats.Pbgra32, null);
            }

            if (needRePaint)
            {
                needRePaint = false;

                // draw on the bitmap
                bitmap.Lock();
                using (var surface = SKSurface.Create(info, bitmap.BackBuffer, bitmap.BackBufferStride))
                {
                    if (IgnorePixelScaling)
                    {
                        var canvas = surface.Canvas;
                        canvas.Scale(scaleX, scaleY);
                        canvas.Save();
                    }

                    PaintSurface(surface, info.WithSize(userVisibleSize));
                }

                // draw the bitmap to the screen
                bitmap.AddDirtyRect(new Int32Rect(0, 0, info.Width, size.Height));
                bitmap.Unlock();
            }

            drawingContext.DrawImage(bitmap, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        protected virtual void PaintSurface(SKSurface surface, SKImageInfo info)
        {
            if (Example == null)
                return;

            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            try
            {
                if (paintEnumerator == null)
                {
                    BoundsPainter drawer = Example.Value;
                    drawer.RandomSeed = (int)(DateTime.Now.Ticks % uint.MaxValue);

                    drawer.SetCanvas(canvas);

                    paintEnumerator = drawer.ProgressivePaint(new SKRect(0, 0, info.Width, info.Height)).GetEnumerator();
                }

                do
                {
                    if (paintEnumerator.MoveNext())
                    {
                        if (paintEnumerator.Current)
                        {
                            //Dispatcher.Invoke(delegate
                            //{
                            //    SkiaCanvas.InvalidateVisual();
                            //});
                        }
                    }
                    else
                    {
                        paintEnumerator = null;
                    }
                }
                while ((paintEnumerator != null) && !paintEnumerator.Current);
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    MessageBox.Show(ex.ToString(), "Error Running Example");
                });
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            InvalidateVisual();
        }

        public void RePaint()
        {
            InvalidateVisual();

            needRePaint = true;
        }

        private SKSizeI CreateSize(out SKSizeI unscaledSize, out float scaleX, out float scaleY)
        {
            unscaledSize = SKSizeI.Empty;
            scaleX = 1.0f;
            scaleY = 1.0f;

            var w = ActualWidth;
            var h = ActualHeight;

            if (!IsPositive(w) || !IsPositive(h))
                return SKSizeI.Empty;

            unscaledSize = new SKSizeI((int)w, (int)h);

            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            scaleX = (float)m.M11;
            scaleY = (float)m.M22;
            return new SKSizeI((int)(w * scaleX), (int)(h * scaleY));

            bool IsPositive(double value)
            {
                return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0;
            }
        }
    }
}