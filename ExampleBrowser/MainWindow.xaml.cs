using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Generative;

namespace ExampleBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool skipFirst = false;

        private GenerativeExample example;
        public GenerativeExample Example
        {
            get => this.example;
            set
            {
                this.example = value;

                double desiredWidth = example.Value.DesiredAspectRatio * Height;

                if (Width != desiredWidth)
                {
                    skipFirst = true;

                    Width = desiredWidth;
                }
                else
                    this.RePaint();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            combox.DataContext = GenerativeExample.ExampleList;

            Window.GetWindow(this).KeyDown += MainWindow_KeyDown;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (skipFirst || Example == null)
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
                BoundsPainter drawer = Example.Value;
                drawer.RandomSeed = (int)(DateTime.Now.Ticks % uint.MaxValue);

                drawer.SetCanvas(canvas);
                drawer.Paint(new SKRect(0, 0, scaledSize.Width, scaledSize.Height));
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    MessageBox.Show(ex.ToString(), "Error Running Example");
                });
            }
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F5:
                    RePaint();
                    break;
            }
        }

        public void RePaint()
        {
            SkiaCanvas.InvalidateVisual();
        }

        private void ReDrawButton_Click(object sender, RoutedEventArgs e)
        {
            RePaint();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "png";
            dialog.Filter = "Png Images|*.png";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == true)
            {
                BoundsPainter drawer = Example.Value;

                drawer.SavePng(dialog.FileName, (int)Width, (int)Height);
            }
        }
    }
}
