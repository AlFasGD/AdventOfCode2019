using System.Collections.Generic;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
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
}
