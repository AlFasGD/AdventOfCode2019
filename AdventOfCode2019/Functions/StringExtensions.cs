using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Functions
{
    public static class StringExtensions
    {
        public static string[] GetLines(this string s) => s.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
    }
}
