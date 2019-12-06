using System.IO;

namespace AdventOfCode2019
{
    public abstract class Problem<T>
    {
        protected readonly string FileContents;

        public int Day => int.Parse(GetType().Name.Substring("Day".Length));

        public Problem() => FileContents = File.ReadAllText($"AoC2019/{Day}.txt");

        public abstract T RunPart1();
        public abstract T RunPart2();
    }
}
