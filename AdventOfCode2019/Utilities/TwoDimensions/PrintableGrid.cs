using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public abstract class PrintableGrid<T>
    {
        private Dictionary<T, char> printableCharacters;

        protected T[,] Grid;

        public readonly int Width, Height;

        public PrintableGrid(int both) : this(both, both) { }
        public PrintableGrid(int width, int height)
        {
            printableCharacters = GetPrintableCharacters();
            Grid = new T[Width = width, Height = height];
        }

        public virtual void PrintGrid() => Console.WriteLine(ToString());

        protected abstract Dictionary<T, char> GetPrintableCharacters();

        public T this[int x, int y]
        {
            get => Grid[x, y];
            set => Grid[x, y] = value;
        }
        public T this[Location2D location]
        {
            get => Grid[location.X, location.Y];
            set => Grid[location.X, location.Y] = value;
        }

        public sealed override string ToString()
        {
            var builder = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    builder.Append(printableCharacters[Grid[x, y]]);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
