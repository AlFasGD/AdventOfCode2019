using System.IO;
using System.Linq;

namespace AdventOfCode2019
{
    public abstract class Problem
    {
        protected int CurrentTestCase;

        protected string FileContents => GetFileContents(CurrentTestCase);
        protected string[] FileLines => GetFileLines(CurrentTestCase);

        public int Day => int.Parse(GetType().Name.Substring("Day".Length));
        public int TestCaseFiles => Directory.GetFiles("AoC2019").Where(f => f.Replace('\\', '/').Split('/').Last().StartsWith($"{Day}T")).Count();

        private string GetFileContents(int testCase) => File.ReadAllText(GetFileLocation(testCase));
        private string[] GetFileLines(int testCase) => GetFileContents(testCase).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        private string GetFileLocation(int testCase) => $"AoC2019/{Day}{(testCase > 0 ? $"T{testCase}" : "")}.txt";
    }
    public abstract class Problem<T1, T2> : Problem
    {
        public abstract T1 RunPart1();
        public abstract T2 RunPart2();

        public T1 TestRunPart1(int testCase)
        {
            CurrentTestCase = testCase;
            T1 result = RunPart1();
            CurrentTestCase = 0;
            return result;
        }
        public T2 TestRunPart2(int testCase)
        {
            CurrentTestCase = testCase;
            T2 result = RunPart2();
            CurrentTestCase = 0;
            return result;
        }
    }
    public abstract class Problem<T> : Problem<T, T> { }
}
