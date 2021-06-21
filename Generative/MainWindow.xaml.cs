using System;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Generative
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //BoundsPainter drawing = new StainedGlass();
        //BoundsPainter drawing = new Impressionist();
        //BoundsPainter drawing = new Ribbons();
        //BoundsPainter drawing = new BrokenCircle();
        //BoundsPainter drawing = new Threads();
        //BoundsPainter drawing = new Hair();
        BoundsPainter drawing = new Tendrils();

        public MainWindow()
        {
            InitializeComponent();

            SkiaCanvas.Focus();

            //drawing.SavePng(@"C:\tmp\test.png", 800, 800);

            //for (int i = 0; i < 10; i++)
            //{
            //    drawing.SavePng(@"C:\tmp\test" + i + ".png", 2048, 2048);
            //}
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;

			float scale = (float)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
			SKSize scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

			canvas.Scale(scale);

            canvas.Clear(SKColors.White);

            drawing.RandomSeed = (int)(DateTime.Now.Ticks % uint.MaxValue);

            drawing.SetCanvas(canvas);
            drawing.Paint(new SKRect(0, 0, scaledSize.Width, scaledSize.Height));
        }

        private void SKElement_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F5:
                    SkiaCanvas.InvalidateVisual();
                    break;
            }
        }
    }
}
