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
        //StainedGlass drawing = new StainedGlass();
        //Impressionist drawing = new Impressionist();
        //Ribbons drawing = new Ribbons();
        //BrokenCircle drawing = new BrokenCircle();
        //Threads drawing = new Threads();
        //Hair drawing = new Hair();
        //Tendrils drawing = new Tendrils();
        //FlowPainter drawing = new FlowPainter();
        //ColorSubdivisions drawing = new ColorSubdivisions();
        //Canvasify drawing = new Canvasify();
        //Noodling drawing = new Noodling();
        TieDie drawing = new TieDie();

        public MainWindow()
        {
            InitializeComponent();

            Height = 800;
            Width = 800 * drawing.DesiredAspectRatio;

            SkiaCanvas.Focus();

            //for (int i = 0; i < 10; i++)
            //{
            //    drawing.SavePng(@"c:\tmp\test" + i + ".png", (int)Width, (int)Height);
            //}

            //drawing.SavePng(@"c:\tmp\test.png", (int)Width, (int)Height);
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
