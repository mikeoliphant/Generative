using System;
using System.Collections.Generic;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    public class ColorSubdivisions : BoundsPainter
    {
        SKBitmap colorBitmap;
        ColorSubdivider colorSubdivider;
        float drawScaleX;
        float drawScaleY;

        SKPaint paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.StrokeAndFill
        };

        public ColorSubdivisions()
        {
            colorBitmap = ImageUtil.BitmapFromURL("https://avatars.githubusercontent.com/u/6710799?v=4");
            //colorBitmap = ImageUtil.BitmapFromURL("https://cdn.lifestyleasia.com/wp-content/uploads/sites/3/2020/11/16190957/photo-1543349689-9a4d426bee8e-1243x900.jpeg");

            int maxSize = 512;

            if ((colorBitmap.Width > maxSize) || (colorBitmap.Height > maxSize))
            {
                float xScale = (float)maxSize / (float)colorBitmap.Width;
                float yScale = (float)maxSize / (float)colorBitmap.Height;

                float scale = Math.Min(xScale, yScale);

                colorBitmap = colorBitmap.Resize(new SKImageInfo((int)(colorBitmap.Width * scale), (int)(colorBitmap.Height * scale)), SKFilterQuality.High);
            }

            colorSubdivider = new ColorSubdivider(colorBitmap, 0.03f);

            DesiredAspectRatio = (float)colorBitmap.Width / (float)colorBitmap.Height;
        }

        public override void Paint(SKRect bounds)
        {
            drawScaleX = bounds.Width / colorBitmap.Width;
            drawScaleY = bounds.Height / colorBitmap.Height;

            foreach (DivisionTreeNode node in colorSubdivider.Nodes)
            {
                PaintNode(node);
            }
        }

        int index = 0;
        SKColor[] palette = Palette.RainbowMuted;

        void PaintNode(DivisionTreeNode node)
        {
            if (node.UL == null)
            {
                paint.Color = node.Color;

                index++;

                Canvas.DrawRect(new SKRect(node.Bounds.Left * drawScaleX, node.Bounds.Top * drawScaleY, node.Bounds.Right * drawScaleX, node.Bounds.Bottom * drawScaleY), paint);
            }
            else
            {
                PaintNode(node.UL);
                PaintNode(node.UR);
                PaintNode(node.LL);
                PaintNode(node.LR);
            }
        }
    }

    public class DivisionTreeNode
    {
        public SKRectI Bounds { get; set; }
        public DivisionTreeNode UL { get; set; }
        public DivisionTreeNode UR { get; set; }
        public DivisionTreeNode LL { get; set; }
        public DivisionTreeNode LR { get; set; }
        public SKColor Color { get; set; }
    }

    public class ColorSubdivider
    {
        public List<DivisionTreeNode> Nodes { get; private set; }

        SKBitmap bitmap = null;
        float maxErrorPerPixel = 0;

        public ColorSubdivider(SKBitmap bitmap, float maxErrorPerPixel)
        {
            this.bitmap = bitmap;
            this.maxErrorPerPixel = maxErrorPerPixel;

            Nodes = new List<DivisionTreeNode>();

            int initialSubdivisions = 8;

            float subdivisionWidth = (float)bitmap.Width / (float)initialSubdivisions;
            float subdivisionHeight = (float)bitmap.Height / (float)initialSubdivisions;

            for (int row = 0; row < initialSubdivisions; row++)
            {
                for (int col = 0; col < initialSubdivisions; col++)
                {
                    DivisionTreeNode node = new DivisionTreeNode
                    {
                        Bounds = new SKRectI((int)(row * subdivisionWidth), (int)(col * subdivisionHeight), (int)((row + 1) * subdivisionWidth), (int)((col + 1) * subdivisionHeight))
                    };

                    Nodes.Add(node);

                    CheckNode(node);
                }
            }
        }

        void CheckNode(DivisionTreeNode node)
        {
            float err = CalculateColorError(node, maxErrorPerPixel);

            if ((err > maxErrorPerPixel) && (node.Bounds.Width > 2) && (node.Bounds.Height > 2))
            {
                node.UL = new DivisionTreeNode
                {
                    Bounds = new SKRectI(node.Bounds.Left, node.Bounds.Top, node.Bounds.Left + (node.Bounds.Width / 2), node.Bounds.Top + (node.Bounds.Height / 2))
                };
                CheckNode(node.UL);

                node.UR = new DivisionTreeNode
                {
                    Bounds = new SKRectI(node.Bounds.Left + (node.Bounds.Width / 2),  node.Bounds.Top, node.Bounds.Right, node.Bounds.Top + (node.Bounds.Height / 2))
                };
                CheckNode(node.UR);

                node.LL = new DivisionTreeNode
                {
                    Bounds = new SKRectI(node.Bounds.Left, node.Bounds.Top + (node.Bounds.Height / 2), node.Bounds.Left + (node.Bounds.Width / 2), node.Bounds.Bottom)
                };
                CheckNode(node.LL);

                node.LR = new DivisionTreeNode
                {
                    Bounds = new SKRectI(node.Bounds.Left + (node.Bounds.Width / 2), node.Bounds.Top + (node.Bounds.Height / 2), node.Bounds.Right, node.Bounds.Bottom)
                };
                CheckNode(node.LR);
            }
        }

        float CalculateColorError(DivisionTreeNode node, float maxErrPerPixel)
        {
            node.Color = bitmap.GetPixel(node.Bounds.MidX, node.Bounds.MidY);

            float err = ImageUtil.GetColorError(bitmap, node.Bounds, node.Color, maxErrPerPixel);

            return err;
        }
    }
}
