using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lesson1.GameLogic;
using Lesson1.GameLogic.Structures;

namespace Lesson1
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        private static Game CurrentGame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) => StartEngine();

        /// <summary>
        /// Starts game "engine."
        /// </summary>
        static void StartEngine()
        {
            while (true)
            {
                // Write to console.
                Console.WriteLine("Select a game: ");
                Console.WriteLine("1. MineSweeper \n");
                Console.WriteLine("Enter a number to select game: \n");

                // Get gamemode from input
                int mode;
                while (true)
                {
                    mode = ParseInputToInt(Console.ReadLine());
                    if (mode == 1)
                        break;
                }

                // Start game depending on what the user had input.
                switch (mode)
                {
                    case 1:
                        CurrentGame = new MineSweeper(12, 12, Difficulty.Normal);
                        break;
                    default:
                        throw new Exception("Invalid game mode has been declared.");
                }

                CurrentGame.StartGame();
            }
        }

        /// <summary>
        /// Parses user input to a positive integer.
        /// Otherwise just return -1.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static private int ParseInputToInt(string input)
        {
            int number;
            Int32.TryParse(input, out number);

            // Validate that the number is greater than 0
            // Or else just return -1.
            if (number < 0)
                return -1;
            else
                return number;
        }
    }
}
