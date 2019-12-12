namespace AdventOfCode2019.Utilities
{
    public interface IHasX
    {
        int X { get; set; }

        IHasX InvertX { get; }
    }
}
