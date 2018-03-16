using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.ImageSharp.Processing.Transforms;
using System.Collections.Generic;
using SixLabors.Primitives;

namespace Composition110110
{
    internal class Composition
    {
        static Random r = new Random();
        private Image<Rgba32> image;
        Rgba32[] paintColors = new Rgba32[]
        {
            new Rgba32(200, 0, 0),
            new Rgba32(50, 200, 0),
            new Rgba32(30, 50, 200),
            new Rgba32(230, 250, 0),
        };

        public Composition(Image<Rgba32> image)
        {
            this.image = image;
        }

        internal static void ProcessRandomFile(string directoryPath, string searchPattern)
        {
            var files = Directory.EnumerateFiles(directoryPath, searchPattern).ToList();
            Directory.CreateDirectory(Path.Combine(directoryPath, "out"));

            for (int i = 0; i < 250; i++)
            {
                Console.WriteLine($"Composition {i}");
                var file = files[r.Next(0, files.Count)];
                var image = Image.Load(file);
                var composition = new Composition(image);
                //composition.MakeBlackAndWhite(); // TODO: just get nice black and white images
                composition.Crop(400);
                composition.PaintMany(6);
                composition.Save(Path.Combine(directoryPath, "out", $"{i}.png"));
            }
        }

        private void Crop(int dimension)
        {
            this.image.Mutate(n => n.Crop(new Rectangle(
                x: r.Next(0, image.Width - dimension),
                y: r.Next(0, image.Height - dimension),
                width: dimension,
                height: dimension)));
        }

        private void PaintMany(int amount)
        {
            for (int i = 0; i < amount; i++)
                Paint(i % paintColors.Length);
        }

        private void Paint(int colorIndex)
        {
            var x = r.Next(0, this.image.Width);
            var y = r.Next(0, this.image.Height);
            var pixel = this.image[x, y];
            if (pixel.B + pixel.G + pixel.R > 600)
            {
                // it's more or less white. go with this point
                floodFill(x, y, paintColors[colorIndex]);
            }
            else
            {
                // Look for another point
                Paint(colorIndex);
            }
        }

        Stack<Point> toFill;
        private void floodFill(int x, int y, Rgba32 color)
        {
            toFill = new Stack<Point>();
            int pixelsPainted = 0;
            toFill.Push(new Point(x, y));
            do
            {
                var location = toFill.Pop();
                var pixel = this.image[location.X, location.Y];
                if (pixel.B + pixel.G + pixel.R > 600)
                {
                    pixelsPainted++;
                    this.image[location.X, location.Y] = color;
                    if (location.X > 0 && location.X < image.Width - 1 && location.Y > 0 && location.Y < image.Height - 1)
                    {
                        toFill.Push(new Point(location.X + 1, location.Y));
                        toFill.Push(new Point(location.X - 1, location.Y));
                        toFill.Push(new Point(location.X, location.Y + 1));
                        toFill.Push(new Point(location.X, location.Y - 1));
                    }
                }
            } while (toFill.Count > 0);
            System.Diagnostics.Debug.WriteLine($"Painted {pixelsPainted} pixels");
        }

        private void MakeBlackAndWhite()
        {
            this.image.Mutate(x => x.BlackWhite().Contrast(50).Brightness(30));
        }

        private void Save(string path)
        {
            image.SaveAsPng(new FileStream(path, FileMode.Create));
        }
    }
}