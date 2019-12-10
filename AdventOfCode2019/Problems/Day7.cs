using AdventOfCode2019.Utilities;
using System;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2019.Problems
{
    public class Day7 : Problem<int>
    {
        public override int RunPart1() => RunPart(Part1GeneralRunner);
        public override int RunPart2() => RunPart(Part2GeneralRunner);

        private int Part1GeneralRunner(BigInteger[] numbers)
        {
            int[] phaseIndices = new int[5];
            BigInteger max = 0;
            while (phaseIndices[0] < 5)
            {
                int[] phaseStages = new int[5];
                var available = Enumerable.Range(0, 5).ToList();
                for (int i = 0; i < 5; i++)
                {
                    phaseStages[i] = available[phaseIndices[i]];
                    available.RemoveAt(phaseIndices[i]);
                }

                BigInteger output = 0;
                for (int i = 0; i < 5; i++)
                {
                    var computer = new IntcodeComputer();
                    computer.OutputWritten += o => output = o;
                    computer.RunToHalt(numbers, phaseStages[i], output);
                }
                if (output > max)
                    max = output;

                phaseIndices[3]++;
                bool needsFixing = true;
                for (int i = 3; i > 0 && needsFixing; i--)
                    if (needsFixing = phaseIndices[i] > 4 - i)
                    {
                        phaseIndices[i] = 0;
                        phaseIndices[i - 1]++;
                    }
            }

            return (int)max;
        }
        private int Part2GeneralRunner(BigInteger[] numbers)
        {
            int[] phaseIndices = new int[5];
            BigInteger max = 0;
            while (phaseIndices[0] < 5)
            {
                int[] phaseStages = new int[5];
                var available = Enumerable.Range(5, 5).ToList();
                for (int i = 0; i < 5; i++)
                {
                    phaseStages[i] = available[phaseIndices[i]];
                    available.RemoveAt(phaseIndices[i]);
                }

                BigInteger currentInput = 0;
                BigInteger largestThrusterSignal = 0;
                bool[] givenPhaseInput = new bool[5];
                int currentAmplifier = 0;
                var instances = new IntcodeComputer[5];
                for (int i = 0; i < 5; i++)
                    instances[i] = new IntcodeComputer(numbers);
                
                for (int i = 0; i < 5; i++)
                {
                    instances[i].InputRequested += GenerateInput;
                    instances[i].OutputWritten += ProcessOutput;
                }
                while (!instances[currentAmplifier].IsHalted)
                    instances[currentAmplifier].RunUntilOutput();

                if (largestThrusterSignal > max)
                    max = largestThrusterSignal;

                phaseIndices[3]++;
                bool needsFixing = true;
                for (int i = 3; i > 0 && needsFixing; i--)
                    if (needsFixing = phaseIndices[i] > 4 - i)
                    {
                        phaseIndices[i] = 0;
                        phaseIndices[i - 1]++;
                    }

                BigInteger GenerateInput()
                {
                    if (givenPhaseInput[currentAmplifier])
                        return currentInput;
                    givenPhaseInput[currentAmplifier] = true;
                    return phaseStages[currentAmplifier];
                }
                void ProcessOutput(BigInteger o)
                {
                    currentInput = o;
                    if (currentAmplifier == 4)
                    {
                        if (o > largestThrusterSignal)
                            largestThrusterSignal = o;
                    }
                    currentAmplifier = (currentAmplifier + 1) % 5;
                }
            }

            return (int)max;
        }

        public T RunPart<T>(GeneralRunner<T> runner)
        {
            var code = FileContents.Split(',');
            var numbers = new BigInteger[code.Length];
            for (int i = 0; i < code.Length; i++)
                numbers[i] = BigInteger.Parse(code[i]);

            return runner(numbers);
        }

        public delegate T GeneralRunner<T>(BigInteger[] numbersOriginal);
    }
}