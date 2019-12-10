using System;

namespace AdventOfCode2019.Utilities
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class ArgumentCountAttribute : Attribute
    {
        public int ArgumentCount;

        public ArgumentCountAttribute(int argumentCount) => ArgumentCount = argumentCount;
    }
}
