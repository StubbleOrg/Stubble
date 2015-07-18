using System;

namespace Stubble.Core.Performance
{
    public static class ConsoleExtensions
    {
        public static void WriteLineGreen(string line, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line, args);
            Console.ResetColor();
        }

        public static void WriteLineColor(ConsoleColor color, string line, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line, args);
            Console.ResetColor();
        }
    }
}