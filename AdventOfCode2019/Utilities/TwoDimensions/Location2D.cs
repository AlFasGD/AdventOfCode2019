﻿using static AdventOfCode2019.Functions.MathExtensions;
using static System.Math;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public struct Location2D : ILocation<Location2D>, IHasX, IHasY
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsCenter => (X | Y) == 0;
        public int ValueSum => X + Y;
        public int ManhattanDistanceFromCenter => Absolute.ValueSum;

        public Location2D Absolute => (Abs(X), Abs(Y));
        public Location2D Invert => (-X, -Y);

        public Location2D InvertX => (-X, Y);
        public Location2D InvertY => (X, -Y);
        public Location2D Transpose => (Y, X);

        IHasX IHasX.InvertX => InvertX;
        IHasY IHasY.InvertY => InvertY;

        public Location2D(int a) => X = Y = a;
        public Location2D(int x, int y) => (X, Y) = (x, y);
        public Location2D((int, int) point) => (X, Y) = point;

        public int ManhattanDistance(Location2D other) => Abs(X - other.X) + Abs(Y - other.Y);

        public Location2D SignedDifferenceFrom(Location2D other)
        {
            var (x, y) = this - other;
            return (Sign(x), Sign(y));
        }

        public Location2D GetSlopeAsFraction(Location2D other)
        {
            int xDelta = other.X - X;
            int yDelta = other.Y - Y;
            SimplifyFraction(ref xDelta, ref yDelta);
            return (xDelta, yDelta);
        }
        public double GetSlopeRadians(Location2D other)
        {
            var slope = GetSlopeAsFraction(other);
            return AddRadians(HalfCircleRadians, Atan2(slope.Y, slope.X));
        }
        public double GetSlopeDegrees(Location2D other) => ToDegrees(GetSlopeRadians(other));

        public void Forward(Direction d, bool invertX = false, bool invertY = false) => Forward(d, 1, invertX, invertY);
        public void Forward(Direction d, int moves, bool invertX = false, bool invertY = false) => this += new DirectionalLocation(d, invertX, invertY).LocationOffset * moves;

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public static implicit operator Location2D((int X, int Y) point) => new Location2D(point);
        public static implicit operator (int X, int Y)(Location2D point) => (point.X, point.Y);

        public static Location2D operator -(Location2D location) => location.Invert;
        public static Location2D operator +(Location2D left, Location2D right) => (left.X + right.X, left.Y + right.Y);
        public static Location2D operator -(Location2D left, Location2D right) => left + -right;
        public static Location2D operator *(Location2D left, int right) => (left.X * right, left.Y * right);
        public static Location2D operator *(int left, Location2D right) => (left * right.X, left * right.Y);
        public static Location2D operator *(Location2D left, Location2D right) => (left.X * right.X, left.Y * right.Y);
        public static bool operator ==(Location2D left, Location2D right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Location2D left, Location2D right) => left.X != right.X || left.Y != right.Y;

        public override string ToString() => $"({X}, {Y})";
    }
}
