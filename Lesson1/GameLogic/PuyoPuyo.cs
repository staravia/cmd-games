using CSAssignments.GameLogic.Structures;
using CSAssignments.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSAssignments.GameLogic
{
    class PuyoPuyo : Game
    {
        /// <summary>
        /// 
        /// </summary>
        private int TotalRows { get; } = 12;

        /// <summary>
        /// 
        /// </summary>
        private int TotalColumns { get; } = 6;

        /// <summary>
        /// 
        /// </summary>
        private int PocketSize { get; } = 64;

        /// <summary>
        /// 
        /// </summary>
        private int TargetScore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Char, PuyoInput> CharacterToInput { get; } = new Dictionary<char, PuyoInput>
        {
            { 'a', PuyoInput.MoveLeft },
            { 'd', PuyoInput.MoveRight },
            { 's', PuyoInput.HardDrop },
            { 'q', PuyoInput.RotateClockwise },
            { 'e', PuyoInput.RotateAntiClockwise }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Difficulty, int> DifficultyToTargetScore { get; } = new Dictionary<Difficulty, int>
        {
            { Difficulty.Beginner, 2500 },
            { Difficulty.Easy, 5000 },
            { Difficulty.Normal, 10000 },
            { Difficulty.Hard, 30000 },
            { Difficulty.Expert, 100000 }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<PuyoColor, ConsoleColor> IndexToActiveColor { get; } = new Dictionary<PuyoColor, ConsoleColor>
        {
            { PuyoColor.Red, ConsoleColor.Red },
            { PuyoColor.Yellow, ConsoleColor.Yellow },
            { PuyoColor.Green, ConsoleColor.Green },
            { PuyoColor.Blue, ConsoleColor.Cyan }
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<PuyoColor, ConsoleColor> IndexToGuideColor { get; } = new Dictionary<PuyoColor, ConsoleColor>
        {
            { PuyoColor.Red, ConsoleColor.DarkRed },
            { PuyoColor.Yellow, ConsoleColor.DarkYellow },
            { PuyoColor.Green, ConsoleColor.DarkGreen },
            { PuyoColor.Blue, ConsoleColor.DarkCyan }
        };

        /// <summary>
        /// Create and Initialize the Puyo Puyo game
        /// </summary>
        /// <param name="difficulty"></param>
        public PuyoPuyo(Difficulty difficulty)
        {
            TargetScore = DifficultyToTargetScore[difficulty];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override bool Update()
        {
            Thread.Sleep(100);
            TextManager.WriteLine("sleep");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void WriteInstructions()
        {
            TextManager.WriteLine("Objective: Your goal is to connect 4 circles and reach 10,000 points to win!");
            TextManager.WriteLine("How to play: (TODO) add info here later");
            TextManager.WriteLine();
            DrawPuyoBoard();
            TextManager.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GeneratePuyoPocket()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawPuyoBoard()
        {
            
        }
    }
}
