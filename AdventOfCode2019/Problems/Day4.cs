using static System.Convert;

namespace AdventOfCode2019.Problems
{
    public class Day4 : Problem<int>
    {
        public override int Day => 4;

        public override int RunPart1() => General(Part1Criteria);
        public override int RunPart2() => General(Part2Criteria);

        private bool Part1Criteria(int n) => CriteriaCalculator(n, Part1GeneralCriteria);
        private bool Part2Criteria(int n) => CriteriaCalculator(n, Part2GeneralCriteria);
        private bool CriteriaCalculator(int n, Criteria c)
        {
            var s = n.ToString();
            int multipleDigits = 1;
            bool hasDouble = false;
            for (int i = 1; i < s.Length; i++)
                if (!c(s[i], s[i - 1], ref multipleDigits, ref hasDouble))
                    return false;
            return hasDouble |= multipleDigits == 2;
        }
        private bool Part1GeneralCriteria(char currentChar, char previousChar, ref int multipleDigits, ref bool hasDouble)
        {
            if (currentChar > previousChar)
                return false;
            hasDouble |= currentChar == previousChar;
            return true;
        }
        private bool Part2GeneralCriteria(char currentChar, char previousChar, ref int multipleDigits, ref bool hasDouble)
        {
            if (previousChar > currentChar)
                return false;
            if (previousChar == currentChar)
                multipleDigits++;
            else
            {
                if (multipleDigits == 2)
                    hasDouble = true;
                multipleDigits = 1;
            }
            return true;
        }

        private int General(MeetsCriteriaFunction meetsCriteria)
        {
            var values = FileContents.Split('-');
            int start = ToInt32(values[0]);
            int end = ToInt32(values[1]);

            int count = 0;

            for (int i = start; i <= end; i++)
                if (meetsCriteria(i))
                    count++;

            return count;
        }

        private delegate bool MeetsCriteriaFunction(int n);
        private delegate bool Criteria(char currentChar, char previousChar, ref int multipleDigits, ref bool hasDouble);
    }
}
