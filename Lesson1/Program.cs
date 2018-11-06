using System;
using System.Collections.Generic;
using System.Threading;
using CSAssignments.GameLogic;
using CSAssignments.GameLogic.Structures;
using CSAssignments.Helpers;

namespace CSAssignments
{
    /// <summary>
    /// This is the main Class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The current game that the user selected.
        /// </summary>
        private static Game CurrentGame { get; set; }

        /// <summary>
        /// Main method. (First Method that gets called when the program starts.)
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => StartEngine();

        /// <summary>
        /// Used to display appropriate text on screen depending on what game the user selected.
        /// </summary>
        private static Dictionary<int, string> IndexToGameMode { get; } = new Dictionary<int, string>
        {
            { 1, "Mine Sweeper" },
            { 2, "Puyo Puyo" }
        };

        /// <summary>
        /// Referenced to generate and display difficulty that the user selected.
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
        /// Starts the Game "Engine."
        /// </summary>
        private static void StartEngine()
        {
            // Set initial text color
            Console.ForegroundColor = ConsoleColor.White;

            // Start main game loop.
            // Loop will end on exception.
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
        /// This method will keep looping until a bug is found, or if the player closes the program.
        /// </summary>
        private static void Update()
        {
            #region Game-Diff-Selection
            // Write instructions + information to Console.
            TextManager.WriteLine("Select a game: ");
            foreach (var index in IndexToGameMode) TextManager.WriteLine($"{index.Key}. {index.Value}");
            TextManager.WriteLine();
            TextManager.WriteLine("Enter a number to select game: \n", ConsoleColor.Green);

            // Get gamemode from user input
            var gameIndex = GetIndexFromUserInput(IndexToGameMode.Count);
            var gameName = IndexToGameMode[gameIndex];

            // Write instrutions + information to Console.
            TextManager.Clear();
            TextManager.WriteLine($"Game Selected: {gameName}");
            foreach (var index in IndexToDifficulty) TextManager.WriteLine($"{index.Key}. {index.Value}");
            TextManager.WriteLine();
            TextManager.WriteLine("Enter a number to select difficulty: \n", ConsoleColor.Green);

            // Get game difficulty from user input
            var difficultyIndex = GetIndexFromUserInput(IndexToDifficulty.Count);
            var difficulty = IndexToDifficulty[difficultyIndex];

            // Write instructions + information to Console.
            TextManager.Clear();
            TextManager.WriteLine($"Game Selected: {gameName}");
            TextManager.WriteLine($"Difficulty Selected: {difficulty}");
            TextManager.WriteLineBreak();
            TextManager.WriteLine();
            #endregion

            // Start game depending on what the user had input.
            switch (gameIndex)
            {
                case 1:
                    CurrentGame = new MineSweeper(difficulty);
                    break;
                case 2:
                    CurrentGame = new PuyoPuyo(difficulty);
                    break;
                default:
                    throw new Exception("Invalid game mode has been declared.");
            }

            // Start the game once the user has selected a game and difficulty.
            CurrentGame.StartGame();
        }

        /// <summary>
        /// Get Index from user input. Index is a positive value that starts at 1.
        /// </summary>
        /// <param name="totalModes"></param>
        /// <returns></returns>
        private static int GetIndexFromUserInput(int totalModes)
        {
            // Make sure input is equal or between two values
            int number;
            Int32.TryParse(TextManager.ReadLine(), out number);

            if (number < 1 || number > totalModes)
            {
                TextManager.WriteLine("Invalid Input.", ConsoleColor.Red);
                return GetIndexFromUserInput(totalModes);
            }

            // Return input if its between the two values
            return number;
        }
    }
}
