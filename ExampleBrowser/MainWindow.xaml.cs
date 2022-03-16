using System;
using System.Collections;
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
        int saveHeight = 1440;

        public MainWindow()
        {
            InitializeComponent();

            combox.DataContext = GenerativeExample.ExampleList;

            Window.GetWindow(this).KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F5:
                    SkiaCanvas.RePaint();
                    break;
            }
        }

        private void ReDrawButton_Click(object sender, RoutedEventArgs e)
        {
            SkiaCanvas.RePaint();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "png";
            dialog.Filter = "Png Images|*.png";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == true)
            {
                BoundsPainter drawer = SkiaCanvas.Example.Value;

                float saveWidth = drawer.DesiredAspectRatio * saveHeight;

                drawer.SavePng(dialog.FileName, (int)saveWidth, (int)saveHeight);
            }
        }
    }
}
