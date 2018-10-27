using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lesson1.GameLogic;
using Lesson1.GameLogic.Structures;
using Lesson1.Helpers;

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
        public static void Main(string[] args) => StartEngine();

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<int, string> IndexToGameMode { get; } = new Dictionary<int, string>
        {
            { 1, "Mine Sweeper" }
        };

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<int, Difficulty> IndexToDifficulty { get; } = new Dictionary<int, Difficulty>
        {
            { 1, Difficulty.Beginner },
            { 2, Difficulty.Easy },
            { 3, Difficulty.Normal },
            { 4, Difficulty.Hard },
            { 5, Difficulty.Expert }
        };

        /// <summary>
        /// Starts game "engine."
        /// </summary>
        private static void StartEngine()
        {
            // Set initial text color
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                try
                {
                    Update();
                }
                catch(Exception e)
                {
                    TextManager.WriteLine(e.Message, ConsoleColor.Red);
                    TextManager.WriteLine(e.StackTrace, ConsoleColor.Red);
                    break;
                }
            }

            // Write error message
            TextManager.WriteLine("The program has crashed!!!!!", ConsoleColor.Yellow);
            TextManager.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void Update()
        {
            // Gameplay variables
            int gameIndex;
            string gameName;
            int difficultyIndex;
            Difficulty difficulty;

            // Write instructions to Console.
            TextManager.WriteLine("Select a game: ");
            foreach (var index in IndexToGameMode) TextManager.WriteLine($"{index.Key}. {index.Value}");
            TextManager.WriteLine();
            TextManager.WriteLine("Enter a number to select game: \n", ConsoleColor.Green);

            // Get gamemode from user input
            while (true)
            {
                gameIndex = ParseInputToInt(TextManager.ReadLine());
                if (IndexToGameMode.ContainsKey(gameIndex))
                {
                    IndexToGameMode.TryGetValue(gameIndex, out gameName);
                    break;
                }
                TextManager.WriteLine("Invalid Input.", ConsoleColor.Red);
            }

            // Write instrutions to Console.
            TextManager.Clear();
            TextManager.WriteLine($"Game Selected: {gameName}");
            foreach (var index in IndexToDifficulty) TextManager.WriteLine($"{index.Key}. {index.Value}");
            TextManager.WriteLine();
            TextManager.WriteLine("Enter a number to select difficulty: \n", ConsoleColor.Green);

            // Get game difficulty from user input
            while (true)
            {
                difficultyIndex = ParseInputToInt(TextManager.ReadLine());
                if (IndexToDifficulty.ContainsKey(difficultyIndex) == true)
                {
                    IndexToDifficulty.TryGetValue(difficultyIndex, out difficulty);
                    break;
                }
                TextManager.WriteLine("Invalid Input.", ConsoleColor.Red);
            }

            // Write instructions to Console.
            TextManager.Clear();
            TextManager.WriteLine($"Game Selected: {gameName}");
            TextManager.WriteLine($"Difficulty Selected: {difficulty}");
            TextManager.WriteLine();
            TextManager.WriteLineBreak();


            // Start game depending on what the user had input.
            switch (gameIndex)
            {
                case 1:
                    CurrentGame = new MineSweeper(difficulty);
                    break;
                default:
                    throw new Exception("Invalid game mode has been declared.");
            }

            // Start Game
            CurrentGame.StartGame();
        }

        /// <summary>
        /// Parses user input to a positive integer.
        /// Otherwise just return -1.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static int ParseInputToInt(string input)
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
