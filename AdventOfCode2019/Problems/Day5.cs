using System;
using static System.Convert;

namespace AdventOfCode2019.Problems
{
    public class Day5 : Problem<int>
    {
        public override int RunPart1() => RunPart(Part1GeneralRunner);
        public override int RunPart2() => RunPart(Part2GeneralRunner);

        private int Part1GeneralRunner(int[] numbers) => General(numbers, 1);
        private int Part2GeneralRunner(int[] numbers) => General(numbers, 5);

        private int General(int[] numbers, int input)
        {
            int output = 0;
            for (int i = 0; i < numbers.Length; )
            {
                int opcode = numbers[i] % 100;
                if (opcode == 99)
                    break;

                var parameterModes = new ParameterMode[3]; // so far there's only a max of 3 parameters per operation
                int d = 100;
                for (int a = 0; a < 3; a++, d *= 10)
                    parameterModes[a] = (ParameterMode)(numbers[i] / d % 10);

                bool pointerChanged = false;

                ExecuteOperation();
                if (!pointerChanged)
                    i += GetPointerIncrement();

                void ExecuteOperation()
                {
                    int result = GetResult();
                    switch (opcode)
                    {
                        case 1:
                        case 2:
                        case 7:
                        case 8:
                            numbers[numbers[i + 3]] = result;
                            break;
                        case 3:
                            numbers[numbers[i + 1]] = result;
                            break;
                        case 4:
                            output = result;
                            break;
                        case 5:
                        case 6:
                            if (pointerChanged = result > -1)
                                i = result;
                            break;
                    }
                }
                int GetResult()
                {
                    return opcode switch
                    {
                        1 => GetArgument(0) + GetArgument(1),
                        2 => GetArgument(0) * GetArgument(1),
                        3 => input,
                        4 => numbers[numbers[i + 1]],
                        5 => GetArgument(0) != 0 ? GetArgument(1) : -1,
                        6 => GetArgument(0) == 0 ? GetArgument(1) : -1,
                        7 => GetArgument(0) < GetArgument(1) ? 1 : 0,
                        8 => GetArgument(0) == GetArgument(1) ? 1 : 0,
                    };
                }
                int GetArgument(int index)
                {
                    return parameterModes[index] switch
                    {
                        ParameterMode.Position => numbers[numbers[i + index + 1]],
                        ParameterMode.Intermediate => numbers[i + index + 1],
                    };
                }
                int GetPointerIncrement()
                {
                    switch (opcode)
                    {
                        case 1:
                        case 2:
                        case 7:
                        case 8:
                            return 4;
                        case 3:
                        case 4:
                            return 2;
                        case 5:
                        case 6:
                            return 3;
                    }
                    return 1;
                }
            }
            return output;
        }
        public int RunPart(GeneralRunner runner)
        {
            var code = FileContents.Split(',');
            var numbers = new int[code.Length];
            for (int i = 0; i < code.Length; i++)
                numbers[i] = ToInt32(code[i]);

            return runner(numbers);
        }

        public delegate int GeneralRunner(int[] numbersOriginal);
        public delegate int PostRunner(int noun, int output);

        public enum ParameterMode : byte
        {
            Position,
            Intermediate,
        }
    }
}