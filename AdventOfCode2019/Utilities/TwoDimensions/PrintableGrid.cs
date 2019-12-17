using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public abstract class PrintableGrid<T>
    {
        private Dictionary<T, char> printableCharacters;

        protected T[,] Grid;

        public readonly ValueCounterDictionary<T> ValueCounters;
        public readonly int Width, Height;

        public PrintableGrid(int both) : this(both, both) { }
        public PrintableGrid(int width, int height)
        {
            printableCharacters = GetPrintableCharacters();
            ValueCounters = new ValueCounterDictionary<T>(Grid = new T[Width = width, Height = height]);
        }
        public PrintableGrid(PrintableGrid<T> other)
            : this(other.Width, other.Height)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Grid[x, y] = other.Grid[x, y];
            foreach (var v in other.ValueCounters)
                ValueCounters.Add(v);
        }

        public virtual void PrintGrid() => Console.WriteLine(ToString());

        protected abstract Dictionary<T, char> GetPrintableCharacters();

        public T this[int x, int y]
        {
            get => Grid[x, y];
            set
            {
                ValueCounters.AdjustValue(Grid[x, y], value);
                Grid[x, y] = value;
            }
        }
        public T this[Location2D location]
        {
            get => this[location.X, location.Y];
            set => this[location.X, location.Y] = value;
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
