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

        private BigInteger[] staticMemory;
        private BigInteger[] memory = new BigInteger[100000];

        private List<BigInteger> buffer = new List<BigInteger>();
        private BigInteger lastOutput;

        private int bufferIndex = 0;
        private int relativeModeOffset = 0;
        private int executionPointer = 0;

        private VMState state = VMState.Standby;

        public bool IsStandby => state == VMState.Standby;
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
            staticMemory = new BigInteger[m.Length];
            for (int i = 0; i < m.Length; i++)
                staticMemory[i] = m[i];
        }
        public IntcodeComputer(BigInteger[] m) => staticMemory = m;
        public IntcodeComputer(string s)
        {
            var code = s.Split(',');
            staticMemory = new BigInteger[code.Length];
            for (int i = 0; i < code.Length; i++)
                staticMemory[i] = BigInteger.Parse(code[i]);
        }

        public BigInteger RunToHalt(BigInteger[] m = null, params BigInteger[] inputBuffer) => Run(false, m, inputBuffer);
        public BigInteger RunUntilOutput(BigInteger[] m = null, params BigInteger[] inputBuffer) => Run(true, m, inputBuffer);
        public BigInteger Run(bool pauseOnOutput = false, BigInteger[] m = null, params BigInteger[] inputBuffer)
        {
            buffer.AddRange(inputBuffer);

            if (!IsPaused)
            {
                if (m == null)
                    m = staticMemory;
                for (int k = 0; k < m.Length; k++)
                    memory[k] = m[k];
            }

            state = VMState.Running;

            while (true)
            {
                var opcode = (Opcode)(int)(memory[executionPointer] % 100);
                if (opcode == Opcode.Halt)
                    break;

                var parameterModes = new ParameterMode[MaxParameterCount];
                int d = 100;
                for (int a = 0; a < MaxParameterCount; a++, d *= 10)
                    parameterModes[a] = (ParameterMode)(int)(memory[executionPointer] / d % 10);

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
                BigInteger GetArgument(int index) => memory[GetAddressFromArgument(index)];
                void WriteResult(int index, BigInteger result) => memory[GetAddressFromArgument(index)] = result;
                int GetAddressFromArgument(int index)
                {
                    int offset = executionPointer + index + 1;
                    return parameterModes[index] switch
                    {
                        ParameterMode.Position => (int)memory[offset],
                        ParameterMode.Intermediate => offset,
                        ParameterMode.Relative => relativeModeOffset + (int)memory[offset],
                    };
                }
                int GetPointerIncrement() => argumentCounts[opcode] + 1;
            }

            state = VMState.Halted;

            return lastOutput;
        }

        public void Reset()
        {
            buffer.Clear();
            bufferIndex = 0;
            relativeModeOffset = 0;
            executionPointer = 0;
            state = VMState.Standby;
        }

        public void BufferInput(BigInteger input) => buffer.Add(input);
        public void BufferInput(params BigInteger[] input) => buffer.AddRange(input);

        public BigInteger GetMemoryAt(int address) => memory[address];
        public void SetMemoryAt(int address, BigInteger value) => memory[address] = value;
        public BigInteger GetStaticMemoryAt(int address) => staticMemory[address];
        public void SetStaticMemoryAt(int address, BigInteger value) => staticMemory[address] = value;

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
        Standby,
        Running,
        Paused,
        Halted,
    }
}
