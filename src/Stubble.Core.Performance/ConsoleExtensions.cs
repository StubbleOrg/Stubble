using System;

namespace Stubble.Core.Performance
{
    public static class ConsoleExtensions
    {
        public static void WriteLineGreen(string line, params object[] args)
        {
            WriteLineWithTime(ConsoleColor.Green, line, args);
        }

        public static void WriteLineColor(ConsoleColor foreColor, string line, params object[] args)
        {
            WriteLineColor(foreColor, ConsoleColor.Black, line, args);
        }

        public static void WriteLineColor(ConsoleColor foreColor, ConsoleColor backColor, string line, params object[] args)
        {
            WriteLineWithTime(foreColor, backColor, line, args);
        }

        public static void WriteLineWithTime(ConsoleColor foreColor, string line, params object[] args)
        {
            WriteLineWithTime(foreColor, ConsoleColor.Black, line, args);
        }

        public static void WriteLineWithTime(ConsoleColor foreColor, ConsoleColor backColor, string line, params object[] args)
        {
            if (Program.GlobalStopwatch != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(@"{0:hh\:mm\:ss}", Program.GlobalStopwatch.Elapsed);
                Console.ResetColor();
                Console.Write(" : ");
            }
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(line, args);
            Console.ResetColor();
        }

        public static void WriteLine(string line, params object[] args)
        {
            WriteLineWithTime(ConsoleColor.White, ConsoleColor.Black, line, args);
        }
    }
}