using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Utilities
{
    public interface ILocation
    {
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
