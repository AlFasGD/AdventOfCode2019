using AdventOfCode2019.Utilities;
using System.Numerics;

namespace AdventOfCode2019.Problems
{
    public class Day9 : Problem<BigInteger>
    {
        public override BigInteger RunPart1() => new IntcodeComputer(FileContents).RunToHalt(null, 1);
        public override BigInteger RunPart2() => new IntcodeComputer(FileContents).RunToHalt(null, 2);
    }
}