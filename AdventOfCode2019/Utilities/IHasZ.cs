namespace AdventOfCode2019.Utilities
{
    public interface IHasZ
    {
        int Z { get; set; }

        IHasZ InvertZ { get; }
    }
}
