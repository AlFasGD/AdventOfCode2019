using System.Numerics;

namespace AdventOfCode2019.Problems
{
    public class Day9 : Problem<BigInteger>
    {
        public override BigInteger RunPart1() => RunPart(Part1GeneralRunner);
        public override BigInteger RunPart2() => RunPart(Part2GeneralRunner);

        private BigInteger Part1GeneralRunner(BigInteger[] numbers) => General(numbers, 1);
        private BigInteger Part2GeneralRunner(BigInteger[] numbers) => General(numbers, 2);

        private BigInteger General(BigInteger[] memory, BigInteger input)
        {
            BigInteger output = 0;
            int relativeModeOffset = 0;
            var numbers = new BigInteger[100000];
            for (int k = 0; k < memory.Length; k++)
                numbers[k] = memory[k];
            int i = 0;
            while (true)
            {
                int opcode = (int)(numbers[i] % 100);
                if (opcode == 99)
                    break;

                var parameterModes = new ParameterMode[3]; // so far there's only a max of 3 parameters per operation
                int d = 100;
                for (int a = 0; a < 3; a++, d *= 10)
                    parameterModes[a] = (ParameterMode)(int)(numbers[i] / d % 10);

                bool pointerChanged = false;

                ExecuteOperation();
                if (!pointerChanged)
                    i += GetPointerIncrement();

                void ExecuteOperation()
                {
                    var result = GetResult();
                    switch (opcode)
                    {
                        case 1:
                        case 2:
                        case 7:
                        case 8:
                            WriteResult(2, result);
                            break;
                        case 3:
                            WriteResult(0, result);
                            break;
                        case 4:
                            output = result;
                            break;
                        case 5:
                        case 6:
                            if (pointerChanged = result > -1)
                                i = (int)result;
                            break;
                        case 9:
                            relativeModeOffset = (int)result;
                            break;
                    }
                }
                BigInteger GetResult()
                {
                    return opcode switch
                    {
                        1 => GetArgument(0) + GetArgument(1),
                        2 => GetArgument(0) * GetArgument(1),
                        3 => input,
                        4 => GetArgument(0),
                        5 => GetArgument(0) != 0 ? GetArgument(1) : -1,
                        6 => GetArgument(0) == 0 ? GetArgument(1) : -1,
                        7 => GetArgument(0) < GetArgument(1) ? 1 : 0,
                        8 => GetArgument(0) == GetArgument(1) ? 1 : 0,
                        9 => relativeModeOffset + GetArgument(0),
                    };
                }
                BigInteger GetArgument(int index) => numbers[GetAddressFromArgument(index)];
                void WriteResult(int index, BigInteger result) => numbers[GetAddressFromArgument(index)] = result;
                int GetAddressFromArgument(int index)
                {
                    int offset = i + index + 1;
                    return parameterModes[index] switch
                    {
                        ParameterMode.Position => (int)numbers[offset],
                        ParameterMode.Intermediate => offset,
                        ParameterMode.Relative => relativeModeOffset + (int)numbers[offset],
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
                        case 9:
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
        public T RunPart<T>(GeneralRunner<T> runner)
        {
            var code = FileContents.Split(',');
            var numbers = new BigInteger[code.Length];
            for (int i = 0; i < code.Length; i++)
                numbers[i] = BigInteger.Parse(code[i]);

            return runner(numbers);
        }

        public delegate T GeneralRunner<T>(BigInteger[] numbersOriginal);

        public enum ParameterMode : byte
        {
            Position,
            Intermediate,
            Relative,
        }
    }
}