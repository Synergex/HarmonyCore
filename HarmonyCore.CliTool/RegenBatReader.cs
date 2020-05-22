using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HarmonyCore.CliTool
{
    class RegenBatReader
    {
        public class SetLine
        {
            public SetLine(Match lineContents)
            {
                Key = lineContents.Groups[1].Value;
                Value = lineContents.Groups[2].Value;
            }
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public static Regex SetMatcher = new Regex("set\\s(\\w+)\\s?=\\s?([^\\v]+)");

        public static Match GetMatch(string lineContents)
        {
            var matches = SetMatcher.Match(lineContents);
            return matches;
        }
    }
}
