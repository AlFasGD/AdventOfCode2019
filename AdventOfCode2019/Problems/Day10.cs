using AdventOfCode2019.Utilities.TwoDimensions;
using System;
using System.Collections.Generic;
using static AdventOfCode2019.Functions.MathExtensions;
using static System.Math;

namespace AdventOfCode2019.Problems
{
    public class Day10 : Problem<int>
    {
        public override int RunPart1() => General(Part1GeneralFunction);
        public override int RunPart2() => General(Part2GeneralFunction);

        private int Part1GeneralFunction(bool[,] asteroids, bool[,] bestSolution, int asteroidCount, int maxVisibleAsteroids, Location bestLocation, int width, int height)
        {
            PrintGrid(bestSolution, bestLocation, width, height);
            return maxVisibleAsteroids;
        }
        private int Part2GeneralFunction(bool[,] asteroids, bool[,] bestSolution, int asteroidCount, int maxVisibleAsteroids, Location bestLocation, int width, int height)
        {
            PrintGrid(asteroids, bestLocation, width, height);
            if (asteroidCount < 200)
                return 0;

            asteroids[bestLocation.X, bestLocation.Y] = false;

            var sorted = new List<SlopedLocation>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (asteroids[x, y])
                    {
                        var location = new Location(x, y);
                        var degrees = AddDegrees(bestLocation.GetSlopeDegrees(location), -90);

                        var slopedLocation = new SlopedLocation(location, degrees, bestLocation);

                        sorted.Add(slopedLocation);
                    }

            sorted.Sort();
            int consecutive = 1;
            for (int i = 1; i < sorted.Count; i++)
            {
                if (sorted[i].HasEqualAbsoluteAngle(sorted[i - 1]))
                    sorted[i].AddFullCircleRotations(consecutive++);
                else
                    consecutive = 1;
            }

            sorted.Sort();
            var l = sorted[199].Location;
            return l.X * 100 + l.Y;
        }

        private int General(GeneralFunction generalFunction)
        {
            var lines = FileLines;

            int height = lines.Length;
            int width = lines[0].Length;

            var asteroids = new bool[width, height];
            int asteroidCount = 0;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (asteroids[x, y] = lines[y][x] == '#')
                        asteroidCount++;

            int maxVisibleAsteroids = 0;

            bool[,] bestSolution = new bool[0, 0];
            int bestSolutionX = 0;
            int bestSolutionY = 0;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (!asteroids[x, y])
                        continue;

                    int visibleAsteroidCount = asteroidCount - 1;

                    var currentlyVisibleAsteroids = new bool[width, height];
                    for (int x0 = 0; x0 < width; x0++)
                        for (int y0 = 0; y0 < height; y0++)
                            currentlyVisibleAsteroids[x0, y0] = asteroids[x0, y0];

                    currentlyVisibleAsteroids[x, y] = false;

                    for (int x0 = 0; x0 < width; x0++)
                        for (int y0 = 0; y0 < height; y0++)
                        {
                            if (!currentlyVisibleAsteroids[x0, y0])
                                continue;

                            int xDelta = x0 - x;
                            int yDelta = y0 - y;
                            SimplifyFraction(ref xDelta, ref yDelta);

                            int multiplier = 1;
                            int x1, y1;
                            bool foundFirst = false;
                            while (IsValidIndex(x1 = x + multiplier * xDelta, width) && IsValidIndex(y1 = y + multiplier * yDelta, height))
                            {
                                if (foundFirst && currentlyVisibleAsteroids[x1, y1])
                                {
                                    visibleAsteroidCount--;
                                    currentlyVisibleAsteroids[x1, y1] = false;
                                }
                                foundFirst |= currentlyVisibleAsteroids[x1, y1];
                                multiplier++;
                            }
                        }

                    if (visibleAsteroidCount > maxVisibleAsteroids)
                    {
                        bestSolution = currentlyVisibleAsteroids;
                        bestSolutionX = x;
                        bestSolutionY = y;
                        maxVisibleAsteroids = visibleAsteroidCount;
                    }
                }

            return generalFunction(asteroids, bestSolution, asteroidCount, maxVisibleAsteroids, (bestSolutionX, bestSolutionY), width, height);
        }

        private static void PrintGrid(bool[,] grid, Location bestSolution, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((x, y) == bestSolution)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('#');
                        Console.ResetColor();
                        continue;
                    }
                    Console.Write(grid[x, y] ? '#' : '.');
                }
                Console.WriteLine();
            }
        }
        private static bool IsValidIndex(int value, int upperBound) => value >= 0 && value < upperBound;

        private delegate int GeneralFunction(bool[,] asteroids, bool[,] bestSolution, int asteroidCount, int maxVisibleAsteroids, Location bestLocation, int width, int height);

        private class SlopedLocation : IComparable<SlopedLocation>
        {
            private const double epsilon = 0.0000001;

            public Location Location { get; }
            public double Angle { get; private set; }
            public int ManhattanDistance { get; }

            public double AbsoluteAngle => Angle % FullCircleDegrees;

            public SlopedLocation(Location location, double angle, int manhattanDistance) => (Location, Angle, ManhattanDistance) = (location, angle, manhattanDistance);
            public SlopedLocation(Location location, double angle, Location other) : this(location, angle, location.ManhattanDistance(other)) { }

            public void AddFullCircleRotations(int count)
            {
                Angle += FullCircleDegrees * count;
            }

            public bool HasEqualAbsoluteAngle(SlopedLocation other) => Abs(AbsoluteAngle - other.AbsoluteAngle) < epsilon;

            public int CompareTo(SlopedLocation other)
            {
                int result = Angle.CompareTo(other.Angle);
                if (result == 0)
                    return ManhattanDistance.CompareTo(other.ManhattanDistance);
                return result;
            }

            public override string ToString() => $"{Location} - {Angle}° - {ManhattanDistance}";
        }
    }
}
