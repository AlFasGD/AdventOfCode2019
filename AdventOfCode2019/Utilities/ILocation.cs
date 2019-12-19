namespace AdventOfCode2019.Utilities
{
    public interface ILocation
    {
        bool IsPositive { get; }
        bool IsNonNegative { get; }
        bool IsCenter { get; }
        int ValueSum { get; }
        int ManhattanDistanceFromCenter { get; }
    }

    public interface ILocation<T> : ILocation
    {
        T Absolute { get; }
        T Invert { get; }

        int ManhattanDistance(T other);
        T SignedDifferenceFrom(T other);
    }
}
