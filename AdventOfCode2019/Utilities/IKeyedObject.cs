using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Utilities
{
    public interface IKeyedObject<T>
    {
        T Key { get; }
    }
}
