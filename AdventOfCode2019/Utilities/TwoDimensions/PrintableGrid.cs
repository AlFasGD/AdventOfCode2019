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

        public Location2D GetUniqueElementLocation(T element)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (Grid[x, y].Equals(element))
                        return (x, y);
            return (-1, -1);
        }
        public void ReplaceWithIntersectionValues(T original, T intersection)
        {
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                    if (Grid[x, y].Equals(original) &&
                        Grid[x, y].Equals(Grid[x, y - 1]) &&
                        Grid[x, y].Equals(Grid[x, y + 1]) &&
                        Grid[x, y].Equals(Grid[x - 1, y]) &&
                        Grid[x, y].Equals(Grid[x + 1, y]))
                        Grid[x, y] = intersection;
        }

        public int GetMedianXOfFirstRegion(int y, T regionValue)
        {
            int x0 = -1;
            int x1 = -1;

            for (int x = 0; x0 == -1 && x < Width; x++)
                if (Grid[x, y].Equals(regionValue))
                    x0 = x;

            if (x0 == -1)
                return -1;

            for (int x = x0; x1 == -1 && x < Width; x++)
                if (!Grid[x, y].Equals(regionValue))
                    x1 = x - 1;

            return (x0 + x1) / 2;
        }

        public bool IsValidLocation(Location2D location) => location.IsNonNegative && location.X < Width && location.Y < Height;

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
