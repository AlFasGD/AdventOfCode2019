namespace AdventOfCode2019.Utilities.TwoDimensions
{
    public struct Rectangle
    {
        public Location2D TopLeft, TopRight, BottomLeft, BottomRight;

        public Rectangle(int minX, int maxX, int minY, int maxY)
        {
            TopLeft = (minX, maxY);
            TopRight = (maxX, maxY);
            BottomLeft = (minX, minY);
            BottomRight = (maxX, minY);
        }
        public Rectangle(Location2D topLeft, Location2D topRight, Location2D bottomLeft, Location2D bottomRight) => (TopLeft, TopRight, BottomLeft, BottomRight) = (topLeft, topRight, bottomLeft, bottomRight);
    }
}
