using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2019.Utilities
{
    // This fucking shit was hell of a pain in the ass to deal with
    public class IntcodeComputer
    {
        private static Dictionary<Opcode, int> argumentCounts = new Dictionary<Opcode, int>();

        public const int MaxParameterCount = 3;

        private BigInteger[] memory;

        private List<BigInteger> buffer = new List<BigInteger>();
        private int bufferIndex = 0;
        private int relativeModeOffset = 0;
        private BigInteger[] numbers = new BigInteger[100000];
        private int executionPointer = 0;
        private BigInteger lastOutput;

        private VMState state;

        public bool IsRunning => state == VMState.Running;
        public bool IsPaused => state == VMState.Paused;
        public bool IsHalted => state == VMState.Halted;

        public event InputReader InputRequested;
        public event OutputReader OutputWritten;

        static IntcodeComputer()
        {
            // What the fuck went wrong with reflection
            foreach (var v in typeof(Opcode).GetEnumValues())
                argumentCounts.Add((Opcode)v, (typeof(Opcode).GetMember(v.ToString()).First().GetCustomAttributes(typeof(ArgumentCountAttribute), false).First() as ArgumentCountAttribute).ArgumentCount);
        }

        public IntcodeComputer() { }
        public IntcodeComputer(int[] m)
        {
            memory = new BigInteger[m.Length];
            for (int i = 0; i < m.Length; i++)
                memory[i] = m[i];
        }
        public IntcodeComputer(BigInteger[] m) => memory = m;
        public IntcodeComputer(string s)
        {
            var code = s.Split(',');
            var numbers = new BigInteger[code.Length];
            for (int i = 0; i < code.Length; i++)
                numbers[i] = BigInteger.Parse(code[i]);
            memory = numbers;
        }

        public BigInteger RunToHalt(BigInteger[] m = null, params BigInteger[] inputBuffer) => Run(false, m, inputBuffer);
        public BigInteger RunUntilOutput(BigInteger[] m = null, params BigInteger[] inputBuffer) => Run(true, m, inputBuffer);
        public BigInteger Run(bool pauseOnOutput = false, BigInteger[] m = null, params BigInteger[] inputBuffer)
        {
            buffer.AddRange(inputBuffer);

            if (!IsPaused)
            {
                if (m == null)
                    m = memory;
                for (int k = 0; k < m.Length; k++)
                    numbers[k] = m[k];
            }

            state = VMState.Running;

            while (true)
            {
                var opcode = (Opcode)(int)(numbers[executionPointer] % 100);
                if (opcode == Opcode.Halt)
                    break;

                var parameterModes = new ParameterMode[MaxParameterCount];
                int d = 100;
                for (int a = 0; a < MaxParameterCount; a++, d *= 10)
                    parameterModes[a] = (ParameterMode)(int)(numbers[executionPointer] / d % 10);

                bool pointerChanged = false;
                bool shouldReturn = false;

                ExecuteOperation();
                if (!pointerChanged)
                    executionPointer += GetPointerIncrement();
                if (shouldReturn)
                {
                    state = VMState.Paused;
                    return lastOutput;
                }

                void ExecuteOperation()
                {
                    var result = GetResult();
                    switch (opcode)
                    {
                        case Opcode.Add:
                        case Opcode.Multiply:
                        case Opcode.LessThan:
                        case Opcode.EqualTo:
                            WriteResult(2, result);
                            break;
                        case Opcode.Read:
                            WriteResult(0, result);
                            break;
                        case Opcode.Write:
                            lastOutput = result;
                            shouldReturn = pauseOnOutput;
                            WriteOutput();
                            break;
                        case Opcode.JumpIfNotZero:
                        case Opcode.JumpIfZero:
                            if (pointerChanged = result > -1)
                                executionPointer = (int)result;
                            break;
                        case Opcode.SetRelativeOffset:
                            relativeModeOffset = (int)result;
                            break;
                    }
                }
                BigInteger GetResult()
                {
                    return opcode switch
                    {
                        Opcode.Add               => GetArgument(0) + GetArgument(1),
                        Opcode.Multiply          => GetArgument(0) * GetArgument(1),
                        Opcode.Read              => ReadInput(),
                        Opcode.Write             => GetArgument(0),
                        Opcode.JumpIfNotZero     => GetArgument(0) != 0 ? GetArgument(1) : -1,
                        Opcode.JumpIfZero        => GetArgument(0) == 0 ? GetArgument(1) : -1,
                        Opcode.LessThan          => GetArgument(0) < GetArgument(1) ? 1 : 0,
                        Opcode.EqualTo           => GetArgument(0) == GetArgument(1) ? 1 : 0,
                        Opcode.SetRelativeOffset => relativeModeOffset + GetArgument(0),
                    };
                }
                BigInteger GetArgument(int index) => numbers[GetAddressFromArgument(index)];
                void WriteResult(int index, BigInteger result) => numbers[GetAddressFromArgument(index)] = result;
                int GetAddressFromArgument(int index)
                {
                    int offset = executionPointer + index + 1;
                    return parameterModes[index] switch
                    {
                        ParameterMode.Position => (int)numbers[offset],
                        ParameterMode.Intermediate => offset,
                        ParameterMode.Relative => relativeModeOffset + (int)numbers[offset],
                    };
                }
                int GetPointerIncrement() => argumentCounts[opcode] + 1;
            }

            state = VMState.Halted;

            return lastOutput;
        }

        public void BufferInput(BigInteger input) => buffer.Add(input);

        public BigInteger GetMemoryAt(int address) => numbers[address];

        private BigInteger ReadInput()
        {
            if (bufferIndex < buffer.Count)
            {
                bufferIndex++;
                return buffer[bufferIndex - 1];
            }
            return InputRequested?.Invoke() ?? default;
        }
        private void WriteOutput() => OutputWritten?.Invoke(lastOutput);

        public enum ParameterMode : byte
        {
            Position,
            Intermediate,
            Relative,
        }
        public enum Opcode : byte
        {
            [ArgumentCount(3)]
            Add = 1,
            [ArgumentCount(3)]
            Multiply = 2,
            [ArgumentCount(1)]
            Read = 3,
            [ArgumentCount(1)]
            Write = 4,
            [ArgumentCount(2)]
            JumpIfNotZero = 5,
            [ArgumentCount(2)]
            JumpIfZero = 6,
            [ArgumentCount(3)]
            LessThan = 7,
            [ArgumentCount(3)]
            EqualTo = 8,
            [ArgumentCount(1)]
            SetRelativeOffset = 9,

            [ArgumentCount(0)]
            Halt = 99,
        }
    }

    public delegate BigInteger InputReader();
    public delegate void OutputReader(BigInteger output);

    public enum VMState
    {
        Running,
        Paused,
        Halted,
    }
}
