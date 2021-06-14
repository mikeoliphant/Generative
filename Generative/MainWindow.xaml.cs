using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        BoundsPainter drawing = new SinStripes();

        public MainWindow()
        {
            InitializeComponent();

            SavePng(800, 800);
        }

        void SavePng(int width, int height)
        {
            SKBitmap bitmap = new SKBitmap(width, height);

            SKCanvas pngCanvas = new SKCanvas(bitmap);

            pngCanvas.Clear(SKColors.White);

            drawing.SetCanvas(pngCanvas);
            drawing.Paint(new SKRect(0, 0, width, height));

            pngCanvas.Flush();

            SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 80);

            using (var stream = File.Create(@"C:\tmp\test.png"))
            {
                // save the data to a stream
                data.SaveTo(stream);
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;

			float scale = (float)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
			SKSize scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

			canvas.Scale(scale);

            canvas.Clear(SKColors.White);

            drawing.SetCanvas(canvas);
            drawing.Paint(new SKRect(0, 0, scaledSize.Width, scaledSize.Height));

        }
    }
}
