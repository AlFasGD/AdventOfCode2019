namespace AdventOfCode2019.Utilities
{
    public interface IHasY
    {
        int Y { get; set; }

        IHasY InvertY { get; }
    }
}
