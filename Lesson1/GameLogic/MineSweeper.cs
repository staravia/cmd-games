using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lesson1.GameLogic.Structures;
using Lesson1.Helpers;

namespace Lesson1.GameLogic
{
    /// <summary>
    /// Mine Sweeper game for Assignment ___
    /// </summary>
    public class MineSweeper : Game
    {
        /// <summary>
        /// Radius of which that the mines won't be generated from the initial coordinate
        /// </summary>
        private static int SAFE_RADIUS = 2;

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
        private MineArea[,] MineFieldArea { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int[,] MineFieldLabels { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool[,] MineAreaChecked { get; set; }

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
            { 'o', 14 },
            { 'p', 15 },
            { 'q', 16 },
            { 'r', 17 },
            { 's', 18 },
            { 't', 19 }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Difficulty, float> DifficultyToMineDensity { get; } = new Dictionary<Difficulty, float>
        {
            { Difficulty.Beginner, 0.08f },
            { Difficulty.Easy, 0.15f },
            { Difficulty.Normal, 0.17f },
            { Difficulty.Hard, 0.19f },
            { Difficulty.Expert, 0.22f }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Difficulty, int> DifficultyToRows { get; } = new Dictionary<Difficulty, int>
        {
            { Difficulty.Beginner, 8 },
            { Difficulty.Easy, 8 },
            { Difficulty.Normal, 12 },
            { Difficulty.Hard, 16 },
            { Difficulty.Expert, 20 }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Difficulty, int> DifficultyToColumns { get; } = new Dictionary<Difficulty, int>
        {
            { Difficulty.Beginner, 8 },
            { Difficulty.Easy, 8 },
            { Difficulty.Normal, 16 },
            { Difficulty.Hard, 32 },
            { Difficulty.Expert, 50 }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, ConsoleColor> MineLabelToColor { get; } = new Dictionary<int, ConsoleColor>
        {
            { 0, ConsoleColor.Black },
            { 1, ConsoleColor.Blue },
            { 2, ConsoleColor.Green },
            { 3, ConsoleColor.Red },
            { 4, ConsoleColor.Yellow },
            { 5, ConsoleColor.White },
            { 6, ConsoleColor.Gray },
            { 7, ConsoleColor.Magenta },
            { 8, ConsoleColor.White },
        };

        /// <summary>
        /// Initialize Mine Sweeper Game
        /// </summary>
        public MineSweeper(Difficulty difficulty)
        {
            TotalRows = DifficultyToRows[difficulty];
            TotalColumns = DifficultyToColumns[difficulty];
            TotalMines = (int)(TotalRows * TotalColumns * DifficultyToMineDensity[difficulty]);
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void WriteInstructions()
        {
            TextManager.WriteLine("Objective: Your goal is to reveal every area in the mine field without triggering a mine");
            TextManager.WriteLine("How to play: Enter a coordinate to review an area.");
            //TextManager.WriteLine($"Pick a letter from A -> {ToUpper(CharToRowIndex.Keys.ElementAt(TotalColumns)}, followed by a number between 1 -> {TotalColumns}");
            TextManager.WriteLine("Examples: B2, F12", ConsoleColor.Gray);
            TextManager.WriteLine();
            TextManager.WriteLineBreak();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override bool Update()
        {
            // Write instructions
            TextManager.WriteLine("Enter a Coordinate: \n", ConsoleColor.Green);

            // Get Coordinate
            var coordinate = ParseInput(TextManager.ReadLine());
            TextManager.Clear();

            // Validate Coordinate
            if (coordinate == null)
            {
                if (MineFieldInitialized)
                {
                    DrawMineField(false);
                    TextManager.WriteLine();
                    TextManager.WriteLine();
                }

                TextManager.WriteLine("Invalid input.", ConsoleColor.Red);
                return true;
            }

            // Generate MineField if it hasn't been generated yet
            if (!MineFieldInitialized)
                GenerateMineFieldArea(coordinate);

            // If coordinate is already solved, remind user.
            var area = MineFieldArea[coordinate.Row, coordinate.Column];
            if (area == MineArea.Revealed || area == MineArea.Safe)
            {
                TextManager.WriteLine();
                TextManager.WriteLine();
                TextManager.WriteLine("Coordinate is already revealed.", ConsoleColor.Red);
                return true;
            }

            // Select MineField area and draw the mine field
            var failed = PlayMineArea(coordinate);
            var totalLeft = CheckTotalUnknownAreasLeft();
            DrawMineField(failed);

            // Write information
            TextManager.WriteLine();
            TextManager.WriteLine($"Total Area Remaining: {totalLeft}");
            TextManager.WriteLine();

            if (totalLeft == 0)
                EndGame();

            return !failed;
        }

        /// <summary>
        /// Select an area in a mine field. 
        /// - It will return true if user has touched a mine.
        /// - Otherwise it will reveal the area that the user selected
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        internal bool PlayMineArea(Coordinate select)
        {
            var currentSpot = MineFieldArea[select.Row, select.Column];
            if (currentSpot == MineArea.Mine)
                return true;

            // Check area around the current spot. If its safe, it will keep checking more areas around it.
            for (var i = 0; i < TotalRows; i++)
            {
                for (var j = 0; j < TotalColumns; j++)
                {
                    MineAreaChecked[i, j] = false;
                }
            }

            MineFieldArea[select.Row, select.Column] = MineArea.Safe;
            CheckForVisiblity(select.Row, select.Column);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal int CheckTotalUnknownAreasLeft()
        {
            var total = 0;
            // If there are still unknown areas in the map, the player has not won yet.
            foreach (var area in MineFieldArea)
                if (area == MineArea.Unknown)
                    total++;

            return total;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        internal void CheckForVisiblity(int row, int column)
        {
            if (row < 0 || row >= TotalRows) return;
            if (column < 0 || column >= TotalColumns) return;
            if (MineAreaChecked[row, column]) return;
            MineAreaChecked[row, column] = true;

            if (MineFieldLabels[row, column] == 0)
                MineFieldArea[row, column] = MineArea.Safe;
            else
                MineFieldArea[row, column] = MineArea.Revealed;

            if (MineFieldArea[row, column] == MineArea.Safe)
            {
                // up
                CheckForVisiblity(row - 1, column);

                // down
                CheckForVisiblity(row + 1, column);

                // left
                CheckForVisiblity(row, column - 1);

                // right
                CheckForVisiblity(row, column + 1);
            }

        }

        /// <summary>
        /// Generates Minefield, but it makes sure that ignores a specific coordinate.
        /// Mainly that ooordinate is the first coordinate that the user had input.
        /// </summary>
        /// <param name="ignore"></param>
        internal void GenerateMineFieldArea(Coordinate ignore)
        {
            // Generate available spaces for minefield
            // First dimension = row, Second dimension = column
            var rng = new Random();
            var available = new List<Queue<int>>();
            for (var row = 0; row < TotalRows; row++)
            {
                available.Add(new Queue<int>());

                // Add columns to the Shuffle list
                var shuffle = new List<int>();
                for (var column = 0; column < TotalColumns - 1; column++)
                {
                    var val = column;
                    shuffle.Add(val);
                }

                // Shuffle the Shuffle list
                //  - Uses Fisher-Yates shuffle algorithm
                for (var column = shuffle.Count() - 1; column > 0; column--)
                {
                    int random = rng.Next(shuffle.Count());
                    var val = shuffle[random];
                    shuffle[random] = column;
                    shuffle[column] = val;
                }

                // Add values from shuffle list to queue
                foreach (var column in shuffle)
                {
                    // Add column to shuffle list only if its outside the radius from the initial coordinate
                    if (Math.Abs(column - ignore.Column) > SAFE_RADIUS || Math.Abs(row - ignore.Row) > SAFE_RADIUS)
                        available[row].Enqueue(column);
                }
            }

            // Initialize Minefield array
            MineFieldArea = new MineArea[TotalRows, TotalColumns];
            MineAreaChecked = new bool[TotalRows, TotalColumns];
            MineFieldInitialized = true;

            // Start generating the Mine Field
            for (var i = 0; i < TotalMines; i++)
            {
                // Keep trying to generate mines until one has been generated
                while (true)
                {
                    var random = rng.Next(TotalRows);
                    if (available[random].Count > 0)
                    {
                        var val = available[random].Dequeue();
                        MineFieldArea[random, val] = MineArea.Mine;
                        break;
                    }
                }
            }

            // Generate MineField labels
            GenerateMineFieldLabels();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void GenerateMineFieldLabels()
        {
            MineFieldLabels = new int[TotalRows, TotalColumns];
            for (var row = 0; row < TotalRows; row++)
            {
                for (var column = 0; column < TotalColumns; column++)
                {
                    MineFieldLabels[row,column] = CountNearbyMines(row, column);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal int CountNearbyMines(int row, int column)
        {
            var count = 0;
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        var y = i + row;
                        var x = j + column;

                        if (y >= 0 && x >= 0 && y < TotalRows && x < TotalColumns)
                        {
                            if (MineFieldArea[y, x] == MineArea.Mine)
                            {
                                count++;
                            }
                        }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void DrawMineField(bool showMines = false)
        {
            // Draw labels on first row
            TextManager.WriteCharacter('_', ConsoleColor.Black);
            TextManager.WriteCharacter('_', ConsoleColor.Black);
            for (var i = 0; i < TotalColumns; i++)
            {
                var val = (i + 1) % 10;
                if (val == 0)
                    TextManager.WriteCharacter('0', ConsoleColor.Magenta);
                else
                    TextManager.WriteCharacter(val.ToString().First(), ConsoleColor.Cyan);
            }

            // Draw mine field
            for (var row = 0; row < TotalRows; row++)
            {
                for (var column = 0; column < TotalColumns; column++)
                {
                    // Draw appropriate vertical label
                    if (column == 0)
                    {
                        char letter = Char.ToUpper(CharToRowIndex.Keys.ElementAt(row));
                        TextManager.WriteLine();
                        TextManager.WriteCharacter(letter, ConsoleColor.Cyan);
                        TextManager.WriteCharacter('_', ConsoleColor.Black);
                    }

                    // Draw character on screen. It will depend on wether the mine is active or not.
                    var color = (column + 1) % 10 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                    switch (MineFieldArea[row, column])
                    {
                        case MineArea.Mine:
                            if (showMines)
                            {
                                TextManager.WriteCharacter('X', ConsoleColor.Red);
                                break;
                            }
                            TextManager.WriteCharacter('O', color);
                            break;
                        case MineArea.Revealed:
                            var label = MineFieldLabels[row, column];
                            TextManager.WriteCharacter(label.ToString().First(), MineLabelToColor[label]);
                            break;
                        case MineArea.Safe:
                            TextManager.WriteCharacter('_', ConsoleColor.Black);
                            break;
                        case MineArea.Unknown:
                            TextManager.WriteCharacter('O', color);
                            break;

                    }
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
            
            // Shift user input to match column index
            column--;

            // Check if row and column are within range.
            if (row < 0 || row >= TotalRows)
                return null;
            if (column < 0 || column >= TotalColumns)
                return null;

            // Return coordinate
            return new Coordinate(row, column);
        }
    }
}
