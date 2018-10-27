﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson1.Helpers
{
    /// <summary>
    /// This class will be managing text that gets displayed on the Ccnsole.
    /// </summary>
    public static class TextManager
    {
        /// <summary>
        /// Writes a single character to the console, and also applies a color.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="color"></param>
        public static void WriteCharacter(char output, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(output);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Write empty line to the console.
        /// </summary>
        public static void WriteLine() => Console.WriteLine();

        /// <summary>
        /// Write a line of white text to the console.
        /// </summary>
        /// <param name="output"></param>
        public static void WriteLine(string output) => Console.WriteLine(output);

        /// <summary>
        /// Write a line of colored text to the console.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="color"></param>
        public static void WriteLine(string output, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Writes a white line break to the console.
        /// </summary>
        public static void WriteLineBreak()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------");
        }

        /// <summary>
        /// Read user input and set text color for user input.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ReadLine(ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            var input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            return input;
        }

        /// <summary>
        /// Clear all the text on the console.
        /// </summary>
        public static void Clear() => Console.Clear();
    }
}