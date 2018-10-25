using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lesson1.GameLogic.Structures;

namespace Lesson1.GameLogic
{
    /// <summary>
    /// Mine Sweeper game for Assignment ___
    /// </summary>
    public class MineSweeper : Game
    {
        /// <summary>
        ///     MAX total number of rows the user can set
        /// </summary>
        private static int MAX_TOTAL_ROWS = 14;

        /// <summary>
        /// 
        /// </summary>
        private static int MIN_TOTAL_ROWS = 9;

        /// <summary>
        ///     MAX total number of columns the user can set
        /// </summary>
        private static int MAX_TOTAL_COLUMNS = 26;

        /// <summary>
        /// 
        /// </summary>
        private static int MIN_TOTAL_COLUMNS = 9;

        /// <summary>
        /// 
        /// </summary>
        private int TotalRows { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int TotalColumns { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int TotalMines { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool[,] MineField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool MineFieldInitialized { get; set; }

        /// <summary>
        /// Used to convert user input to row.
        /// </summary>
        private Dictionary<char, int> CharToRowIndex { get; } = new Dictionary<char, int>
        {
            { 'a', 0 },
            { 'b', 1 },
            { 'c', 2 },
            { 'd', 3 },
            { 'e', 4 },
            { 'f', 5 },
            { 'g', 6 },
            { 'h', 7 },
            { 'i', 8 },
            { 'j', 9 },
            { 'k', 10 },
            { 'l', 11 },
            { 'm', 12 },
            { 'n', 13 },
            { 'o', 14 }
        };

        private Dictionary<Difficulty, float> DifficultyToMineRatio { get; } = new Dictionary<Difficulty, float>
        {
            { Difficulty.Easy, 0.10f },
            { Difficulty.Normal, 0.20f },
            { Difficulty.Hard, 0.30f },
            { Difficulty.Expert, 0.40f }
        };

        /// <summary>
        /// Initialize Mine Sweeper Game
        /// </summary>
        public MineSweeper(int rows, int columns, Difficulty difficulty)
        {
            TotalRows = rows;
            TotalColumns = columns;
            TotalMines = (int)(TotalRows * TotalColumns * DifficultyToMineRatio[difficulty]);
        }

        /// <summary>
        /// Starts the Mine Sweeper game.
        /// </summary>
        internal override void StartGame()
        {
            base.StartGame();
        }

        /// <summary>
        /// Ends the mine Sweeper game.
        /// </summary>
        internal override void EndGame()
        {
            base.EndGame();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override bool Update()
        {
            // Write instructions
            Console.WriteLine("Enter Coordinates: ");
            Console.WriteLine(" - Example: 'A6', 'F14' \n");

            // Get Coordinate
            var coordinate = ParseInput(Console.ReadLine());
            Console.Clear();

            // Validate Coordinate
            if (coordinate == null)
                Console.Out.WriteLine("Invalid input.");
            else
                Console.Out.WriteLine("Coordinate: " + coordinate.Column + ", " + coordinate.Row);

            // Generate MineField if it hasn't been generated yet
            if (!MineFieldInitialized)
                GenerateMineField(coordinate);

            return true;
        }

        /// <summary>
        /// Generates Minefield, but it makes sure that ignores a specific coordinate.
        /// Mainly that ooordinate is the first coordinate that the user had input.
        /// </summary>
        /// <param name="ignore"></param>
        internal void GenerateMineField(Coordinate ignore)
        {
            // Generate available spaces for minefield
            var rng = new Random();
            var available = new List<Queue<int>>();
            for (var i = 0; i < TotalColumns; i++)
            {
                available.Add(new Queue<int>());

                // Add rows to the Shuffle list
                var shuffle = new List<int>();
                for (var j = 0; j < TotalRows - 1; j++)
                    shuffle.Add(j);

                // Shuffle the Shuffle list
                //  - Uses Fisher-Yates shuffle algorithm
                for (var j = 0; j < TotalRows - 1; j++)
                {
                    int random = rng.Next(j + 1);

                    var val = shuffle[random];
                    shuffle[random] = j;
                    shuffle[j] = random;
                }

                // Add values from shuffle list to queue
                foreach (var ob in shuffle)
                    available[i].Enqueue(ob);
            }

            // Start generating the Mine Field
            MineField = new bool[TotalColumns, TotalRows];
            MineFieldInitialized = true;

            for (var i = 0; i < TotalMines; i++)
            {
                var random = rng.Next(TotalColumns);
                if (available[random].Count > 0)
                {
                    var val = available[random].Dequeue();
                    if (random != ignore.Column && val != ignore.Row)
                        MineField[random, val] = true;
                }
            }
        }

        /// <summary>
        /// Parse the user's input and returns a coordinate if viable
        /// Examples of user input this method looks for: "B5", "G12"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Coordinate ParseInput(string input)
        {
            // Convert input to lowercase
            input = input.ToLower();

            // Input length must be either two or three characters long.
            if (input.Length < 2 || input.Length > 3)
                return null;

            // First character of input must be a letter (to specify row.)
            if (!CharToRowIndex.ContainsKey(input.First()))
                return null;

            // Get row and column index
            var row = CharToRowIndex[input.First()];
            var column = -1;
            Int32.TryParse(input.Substring(1), out column);

            // Check if row and column are within range.
            if (row < 0 || row > TotalRows - 1)
                return null;
            if (column < 0 && column > TotalColumns)
                return null;

            // Return coordinate
            return new Coordinate(row, column);
        }
    }
}
