using System;
using static System.Math;
using static AdventOfCode2019.Functions.MathExtensions;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public struct Location
    {
        public int X, Y;

        public bool IsCenter => (X | Y) == 0;
        public int ManhattanDistanceFromCenter => Abs(X) + Abs(Y);

        public Location InvertX => (-X, Y);
        public Location InvertY => (X, -Y);
        public Location Transpose => (Y, X);

        public Location(int x, int y) => (X, Y) = (x, y);
        public Location((int, int) point) => (X, Y) = point;

        public int ManhattanDistance(Location other) => Abs(X - other.X) + Abs(Y - other.Y);

        public Location GetSlopeAsFraction(Location other)
        {
            int xDelta = other.X - X;
            int yDelta = other.Y - Y;
            SimplifyFraction(ref xDelta, ref yDelta);
            return (xDelta, yDelta);
        }
        public double GetSlopeRadians(Location other)
        {
            var slope = GetSlopeAsFraction(other);
            return AddRadians(HalfCircleRadians, Atan2(slope.Y, slope.X));
        }
        public double GetSlopeDegrees(Location other) => ToDegrees(GetSlopeRadians(other));

        public void Forward(Direction d, bool invertX = false, bool invertY = false) => Forward(d, 1, invertX, invertY);
        public void Forward(Direction d, int moves, bool invertX = false, bool invertY = false) => this += new DirectionalLocation(d, invertX, invertY).LocationOffset * moves;

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public static implicit operator Location((int X, int Y) point) => new Location(point);
        public static implicit operator (int X, int Y)(Location point) => (point.X, point.Y);

        public static Location operator -(Location location) => (-location.X, -location.Y);
        public static Location operator +(Location left, Location right) => (left.X + right.X, left.Y + right.Y);
        public static Location operator -(Location left, Location right) => left + -right;
        public static Location operator *(Location left, int right) => (left.X * right, left.Y * right);
        public static Location operator *(int left, Location right) => (left * right.X, left * right.Y);
        public static Location operator *(Location left, Location right) => (left.X * right.X, left.Y * right.Y);
        public static bool operator ==(Location left, Location right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Location left, Location right) => left.X != right.X || left.Y != right.Y;

        public override string ToString() => $"({X}, {Y})";
    }
}
