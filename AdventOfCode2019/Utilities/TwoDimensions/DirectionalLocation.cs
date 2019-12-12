using System.Collections.Generic;

namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public struct DirectionalLocation
    {
        private static Dictionary<Direction, Location2D> locations = new Dictionary<Direction, Location2D>
        {
            { Direction.Up, (0, 1) },
            { Direction.Down, (0, -1) },
            { Direction.Left, (-1, 0) },
            { Direction.Right, (1, 0) },
        };

        public Direction Direction;
        public Location2D LocationOffset
        {
            get
            {
                var l = locations[Direction];
                if (InvertX)
                    l = l.InvertX;
                if (InvertY)
                    l = l.InvertY;
                return l;
            }
        }
        
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public DirectionalLocation(Direction d, bool invertX = false, bool invertY = false)
        {
            Direction = d;
            InvertX = invertX;
            InvertY = invertY;
        }

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
