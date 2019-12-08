using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using static System.Convert;

namespace AdventOfCode2019.Problems
{
    public class Day8 : Problem<int, string>
    {
        public override int RunPart1() => General(Part1CountCalculator);
        public override string RunPart2() => General(Part2CountCalculator);

        private int Part1CountCalculator(ImageLayer[] layers)
        {
            ImageLayer layer = null;
            int minZeroes = int.MaxValue;
            foreach (var l in layers)
            {
                if (l.GetPixelCount(0) < minZeroes)
                {
                    layer = l;
                    minZeroes = l.GetPixelCount(0);
                }
            }
            return layer.GetPixelCount(1) * layer.GetPixelCount(2);
        }
        private string Part2CountCalculator(ImageLayer[] layers)
        {
            var result = new ImageLayer(layers[0]);
            for (int i = 0; i < layers.Length; i++)
                result = result.RenderAbove(layers[i]);
            return result.AsDisplayableString(25, 6).Replace('0', '_').Replace('1', 'H').Replace('2', ' ');
        }

        private T General<T>(ImageProcessor<T> processor)
        {
            var chars = FileContents.ToCharArray();
            const int width = 25;
            const int height = 6;
            const int layerSize = width * height;

            var layers = new ImageLayer[chars.Length / layerSize];
            for (int i = 0; i < chars.Length; i += layerSize)
                layers[i / layerSize] = new ImageLayer(chars, i, layerSize);

            return processor(layers);
        }

        private delegate T ImageProcessor<T>(ImageLayer[] layers);

        private class ImageLayer
        {
            private char[] pixels;
            private Dictionary<int, int> pixelCounts = new Dictionary<int, int>(3);

            public int Length => pixels.Length;

            public ImageLayer(char[] c) : this(c, 0, c.Length) { }
            public ImageLayer(char[] c, int offset, int length)
            {
                pixels = new char[length];
                for (int i = offset; i < offset + length; i++)
                    pixels[i - offset] = c[i];
                for (int i = 0; i < 3; i++)
                    pixelCounts.Add(i, GetPixelCountPrivate(i));
            }
            public ImageLayer(ImageLayer other)
            {
                pixels = other.pixels;
                pixelCounts = new Dictionary<int, int>(pixelCounts);
            }

            public int GetPixelCount(int value) => pixelCounts[value];

            public ImageLayer RenderAbove(ImageLayer other)
            {
                char[] rendered = new char[Length];
                for (int i = 0; i < Length; i++)
                    rendered[i] = (pixels[i] == '2' ? other.pixels : pixels)[i];
                return new ImageLayer(rendered);
            }

            public string AsString() => new string(pixels);
            public string AsDisplayableString(int width, int height)
            {
                var builder = new StringBuilder();
                for (int i = 0; i < height; i++)
                {
                    char[] rowPixels = new char[width];
                    for (int j = 0; j < width; j++)
                        rowPixels[j] = pixels[i * width + j];
                    builder.Append(new string(rowPixels)).Append('\n');
                }
                return builder.Remove(builder.Length - 1, 1).ToString();
            }

            private int GetPixelCountPrivate(int value)
            {
                char c = (char)(value + '0');
                int count = 0;
                foreach (var p in pixels)
                    if (p == c)
                        count++;
                return count;
            }
        }
    }
}
