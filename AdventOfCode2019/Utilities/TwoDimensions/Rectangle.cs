namespace AdventOfCode2019.Utilities.TwoDimensions
{
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
}
