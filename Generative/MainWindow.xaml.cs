using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        //TieDye drawing = new TieDye();
        //SolarCorruption drawing = new SolarCorruption();


        public MainWindow()
        {
            InitializeComponent();
            CreateDefaultPainers();
            combox.DataContext = this.PainerList;
            //double size = 800;

            //Height = size;
            //Width = size * drawing.DesiredAspectRatio;

            //SkiaCanvas.Focus();

            ////for (int i = 0; i < 10; i++)
            ////{
            ////    drawing.SavePng(@"c:\tmp\test" + i + ".png", (int)Width, (int)Height);
            ////}

            //drawing.SavePng(GetPath("test.png"), (int)Width, (int)Height);
        }

        bool skipFirst = true;
        

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
            if (skipFirst || Painter == null)
            {
                skipFirst = false;

                return;
            }
            SKCanvas canvas = e.Surface.Canvas;

			float scale = (float)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
			SKSize scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

			canvas.Scale(scale);

            canvas.Clear(SKColors.White);
            try
            {
                var drawer = Painter.Value;
                drawer.RandomSeed = (int)(DateTime.Now.Ticks % uint.MaxValue);

                drawer.SetCanvas(canvas);
                drawer.Paint(new SKRect(0, 0, scaledSize.Width, scaledSize.Height));
            }
            catch(Exception ex)
            {
                Task.Run(() =>
                {
                    MessageBox.Show(ex.ToString());
                });
            }
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

        private string GetPath(string fileName)
        {
            var tempDir = Path.GetTempPath();
            var targetDir = Path.Combine(tempDir, "GenerativeWPF");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            var filePath = Path.Combine(targetDir, fileName);
            return filePath;
        }


        public IEnumerable<KeyValuePair<Type,Lazy<BoundsPainter>>> PainerList { get; private set; }

        public void CreateDefaultPainers()
        {
            var list = new List<KeyValuePair<Type, Lazy<BoundsPainter>>>()
            {
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(StainedGlass), new Lazy<BoundsPainter>(()=>new StainedGlass())),
                //new Impressionist(),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Ribbons), new Lazy<BoundsPainter>(()=>new Ribbons())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(BrokenCircle), new Lazy<BoundsPainter>(()=>new BrokenCircle())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Threads), new Lazy<BoundsPainter>(()=>new Threads())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Hair), new Lazy<BoundsPainter>(()=>new Hair())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Tendrils), new Lazy<BoundsPainter>(()=>new Tendrils())),
                //new FlowPainter(),
                //new ColorSubdivisions(),
                //new Canvasify(),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Noodling), new Lazy<BoundsPainter>(()=>new Noodling())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(TieDye), new Lazy<BoundsPainter>(()=>new TieDye())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(SolarCorruption), new Lazy<BoundsPainter>(()=>new SolarCorruption())),
                new KeyValuePair<Type, Lazy<BoundsPainter>>( typeof(Clifford), new Lazy<BoundsPainter>(()=>Clifford.Jellyfish))
            };
            this.PainerList = list;
        }

        private Lazy<BoundsPainter> _painter;
        public Lazy<BoundsPainter> Painter
        {
            get => this._painter;
            set
            {
                this._painter = value;
                this.RePaint();
            }
        }

        public void RePaint()
        {
            SkiaCanvas.InvalidateVisual();
        }

        
    }
}
