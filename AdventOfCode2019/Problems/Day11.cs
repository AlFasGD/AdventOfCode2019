using AdventOfCode2019.Utilities;
using AdventOfCode2019.Utilities.TwoDimensions;
using System.Numerics;
using System.Text;

namespace AdventOfCode2019.Problems
{
    public class Day11 : Problem<int, string>
    {
        private const int gridSize = 149;
        private Direction[] orderedDirections =
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left,
        };

        public override int RunPart1() => General(Part1GeneralFunction, Part1Returner);
        public override string RunPart2() => General(Part2GeneralFunction, Part2Returner);

        private void Part1GeneralFunction(PanelColor[,] grid, Location2D startingLocation) { }
        private void Part2GeneralFunction(PanelColor[,] grid, Location2D startingLocation)
        {
            var (x, y) = startingLocation;
            grid[x, y] = PanelColor.White;
        }

        private int Part1Returner(int paintedPanels, PanelColor[,] grid) => paintedPanels;
        private string Part2Returner(int paintedPanels, PanelColor[,] grid)
        {
            var builder = new StringBuilder();
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                    builder.Append(grid[x, y] == PanelColor.White ? '#' : '.');
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private T General<T>(GeneralFunction beforeOperation, Returner<T> returner)
        {
            var grid = new PanelColor[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++)
                for (int y = 0; y < gridSize; y++)
                    grid[x, y] = PanelColor.Untouched;

            var currentLocation = new Location2D(gridSize / 2, gridSize / 2);
            var currentDirection = Direction.Up;
            int currentDirectionIndex = 0;
            int paintedPanels = 0;
            bool givenFirstOutput = false;

            beforeOperation(grid, currentLocation);

            var computer = new IntcodeComputer(FileContents);
            computer.InputRequested += InputRequested;
            computer.OutputWritten += OutputWritten;
            computer.RunToHalt();

            return returner(paintedPanels, grid);

            BigInteger InputRequested()
            {
                givenFirstOutput = false;
                var (x, y) = currentLocation;
                return (int)grid[x, y] & 1;
            }
            void OutputWritten(BigInteger output)
            {
                if (givenFirstOutput)
                {
                    AddDirectionIndex(GetDirectionIndexOffset((int)output));
                    currentLocation.Forward(currentDirection, true, true);
                }
                else
                    PaintPanel(currentLocation, (PanelColor)(int)output);

                givenFirstOutput = true;
            }

            int GetDirectionIndexOffset(int output) => output == 0 ? 1 : -1;
            void AddDirectionIndex(int offset)
            {
                currentDirectionIndex = (currentDirectionIndex + offset + 4) % 4;
                currentDirection = orderedDirections[currentDirectionIndex];
            }
            void PaintPanel(Location2D location, PanelColor color)
            {
                var (x, y) = location;
                if (grid[x, y].HasFlag(PanelColor.Untouched))
                    paintedPanels++;
                grid[x, y] = color;
            }
        }

        private enum PanelColor : byte
        {
            Black = 0,
            White = 1,
            Untouched = 1 << 2,
        }

        private delegate void GeneralFunction(PanelColor[,] grid, Location2D startingLocation);
        private delegate T Returner<T>(int paintedPanels, PanelColor[,] grid);
    }
}