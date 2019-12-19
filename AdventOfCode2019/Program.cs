using AdventOfCode2019.Problems;
using System;

namespace AdventOfCode2019
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            RunProblem(new Day19());
        }

        private static void RunProblem<T1, T2>(Problem<T1, T2> instance)
        {
            int testCases = instance.TestCaseFiles;
            for (int i = 1; i <= testCases; i++)
            {
                Console.WriteLine($"Running test case {i}");
                Console.WriteLine(instance.TestRunPart1(i));
                Console.WriteLine(instance.TestRunPart2(i));
                Console.WriteLine();
            }
            Console.WriteLine("Running problem");
            Console.WriteLine(instance.RunPart1());
            Console.WriteLine(instance.RunPart2());
        }
    }
}