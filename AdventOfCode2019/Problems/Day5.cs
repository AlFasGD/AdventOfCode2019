using AdventOfCode2019.Utilities;

namespace AdventOfCode2019.Problems
{
    public class Day5 : Problem<int>
    {
        public override int RunPart1() => (int)new IntcodeComputer(FileContents).RunToHalt(null, 1);
        public override int RunPart2() => (int)new IntcodeComputer(FileContents).RunToHalt(null, 5);
    }
}