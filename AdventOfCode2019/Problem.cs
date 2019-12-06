using System.IO;

namespace AdventOfCode2019
{
    public abstract class Problem<T1, T2>
    {
        protected readonly string FileContents;
        protected string[] FileLines => FileContents.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        public int Day => int.Parse(GetType().Name.Substring("Day".Length));

        public Problem() => FileContents = File.ReadAllText($"AoC2019/{Day}.txt");

        public abstract T1 RunPart1();
        public abstract T2 RunPart2();
    }
    public abstract class Problem<T> : Problem<T, T> { }
}
