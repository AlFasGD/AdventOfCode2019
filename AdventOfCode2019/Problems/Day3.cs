using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static System.Math;

namespace AdventOfCode2019.Problems
{
    public class Day3 : Problem<int>
    {
        public override int RunPart1() => General(Part1GeneralFunction);
        public override int RunPart2() => General(Part2GeneralFunction);

        private void Part1GeneralFunction(ref int min, Line l0, Line l1)
        {
            Location? intersection;
            if ((intersection = l0.GetIntersectionWith(l1)).HasValue && intersection.Value.ManhattanDistanceFromCenter < min)
                min = intersection.Value.ManhattanDistanceFromCenter;
        }
        private void Part2GeneralFunction(ref int min, Line l0, Line l1)
        {
            Location? intersection;
            if ((intersection = l0.GetIntersectionWith(l1)).HasValue)
            {
                int totalSteps = l0.GetTotalStepsToLocation(intersection.Value) + l1.GetTotalStepsToLocation(intersection.Value);
                if (totalSteps < min)
                    min = totalSteps;
            }
        }

        private int General(GeneralFunction generalFunction)
        {
            var paths = FileLines;

            var lines0 = InitializeLines(0);
            var lines1 = InitializeLines(1);

            int min = int.MaxValue;

            foreach (var l0 in lines0)
                foreach (var l1 in lines1)
                    generalFunction(ref min, l0, l1);

            return min;

            Line[] InitializeLines(int index)
            {
                var lines = paths[index].Split(',').Select(l => Line.Parse(l)).ToArray();
                for (int i = 1; i < lines.Length; i++)
                    lines[i].GetLocationFromPreviousLine(lines[i - 1]);
                return lines;
            }
        }

        private delegate void GeneralFunction(ref int min, Line l0, Line l1);

        public struct Rectangle
        {
            public Location TopLeft, TopRight, BottomLeft, BottomRight;

            public Rectangle(int minX, int maxX, int minY, int maxY)
            {
                TopLeft = (minX, maxY);
                TopRight = (maxX, maxY);
                BottomLeft = (minX, minY);
                BottomRight = (maxX, minY);
            }
            public Rectangle(Location topLeft, Location topRight, Location bottomLeft, Location bottomRight) => (TopLeft, TopRight, BottomLeft, BottomRight) = (topLeft, topRight, bottomLeft, bottomRight);
        }
        public struct Location
        {
            public int X, Y;

            public bool IsCenter => (X | Y) == 0;
            public int ManhattanDistanceFromCenter => Abs(X) + Abs(Y);

            public Location(int x, int y) => (X, Y) = (x, y);
            public Location((int, int) point) => (X, Y) = point;

            public int ManhattanDistance(Location other) => Abs(X - other.X) + Abs(Y - other.Y);

            public void Deconstruct(out int x, out int y) => (x, y) = this;

            public static implicit operator Location((int X, int Y) point) => new Location(point);
            public static implicit operator (int X, int Y)(Location point) => (point.X, point.Y);

            public static Location operator +(Location left, Location right) => (left.X + right.X, left.Y + right.Y);
            public static Location operator *(Location left, int right) => (left.X * right, left.Y * right);
            public static Location operator *(int left, Location right) => (left * right.X, left * right.Y);
            public static Location operator *(Location left, Location right) => (left.X * right.X, left.Y * right.Y);
            public static bool operator ==(Location left, Location right) => left.X == right.X && left.Y == right.Y;
            public static bool operator !=(Location left, Location right) => left.X != right.X || left.Y != right.Y;

            public override string ToString() => $"({X}, {Y})";
        }
        public struct DirectionalLocation
        {
            private static Dictionary<Direction, Location> locations = new Dictionary<Direction, Location>
            {
                { Direction.Up, (0, 1) },
                { Direction.Down, (0, -1) },
                { Direction.Left, (-1, 0) },
                { Direction.Right, (1, 0) },
            };

            public Direction Direction;
            public Location LocationOffset => locations[Direction];

            public DirectionalLocation(Direction d) => Direction = d;

            public static DirectionalLocation Parse(char direction)
            {
                return new DirectionalLocation(direction switch
                {
                    'U' => Direction.Up,
                    'D' => Direction.Down,
                    'L' => Direction.Left,
                    'R' => Direction.Right,
                    _ => default,
                });
            }

            public override string ToString() => Direction.ToString();
        }
        public class Line
        {
            public Location Location;
            public DirectionalLocation Direction;
            public int Movement;

            public Line PreviousLine;

            public Location LocationOffset => Movement * Direction.LocationOffset;
            public Location EndingLocation => Location + LocationOffset;

            public Location ConstantDimension => (LocationOffset.X == 0 ? Location.X : 0, LocationOffset.Y == 0 ? Location.Y : 0);

            public Line(DirectionalLocation direction, int movement, Location location = default) => (Location, Direction, Movement) = (location, direction, movement);

            public void GetLocationFromPreviousLine(Line previousLine) => Location = (PreviousLine = previousLine).EndingLocation;
            public int GetTotalStepsToStart() => PreviousLine?.GetTotalStepsToEnd() ?? 0;
            public int GetTotalStepsToEnd() => (PreviousLine?.GetTotalStepsToEnd() ?? 0) + Movement;
            public int GetTotalStepsToLocation(Location location) => GetTotalStepsToStart() + Location.ManhattanDistance(location);

            public bool IntersectsWith(Line other)
            {
                if (Location.IsCenter || other.Location.IsCenter)
                    return false;
                if (LocationOffset * other.LocationOffset != (0, 0))
                    return false;
                if (LocationOffset.X != 0)
                    return IsWithinX(other) && other.IsWithinY(this);
                return IsWithinY(other) && other.IsWithinX(this);
            }
            public Location? GetIntersectionWith(Line other)
            {
                if (IntersectsWith(other))
                    return ConstantDimension + other.ConstantDimension;
                return null;
            }

            public bool IsWithinX(Line other)
            {
                if (LocationOffset.X > 0)
                    return Location.X < other.Location.X && other.Location.X < EndingLocation.X;
                return EndingLocation.X < other.Location.X && other.Location.X < Location.X;
            }
            public bool IsWithinY(Line other)
            {
                if (LocationOffset.Y > 0)
                    return Location.Y < other.Location.Y && other.Location.Y < EndingLocation.Y;
                else
                    return EndingLocation.Y < other.Location.Y && other.Location.Y < Location.Y;
            }

            public static Line Parse(string line)
            {
                var direction = DirectionalLocation.Parse(line[0]);
                int movement = int.Parse(line.Substring(1));
                return new Line(direction, movement);
            }

            public override string ToString() => $"{Location} {Direction.ToString()[0]}{Movement}";
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
