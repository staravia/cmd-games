using CSAssignments.GameLogic.Structures;
using CSAssignments.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSAssignments.GameLogic
{
    class PuyoPuyo : Game
    {
        /// <summary>
        /// 
        /// </summary>
        private const int INPUT_CHECK_DELTA_MS = 75;

        /// <summary>
        /// 
        /// </summary>
        private const int PUYO_GROUND_TOLERANCE_MS = 800;
        
        /// <summary>
        /// 
        /// </summary>
        private const int PUYO_POCKET_SIZE = 2;
        
        /// <summary>
        /// 
        /// </summary>
        private int TotalColors { get; }

        /// <summary>
        /// 
        /// </summary>
        private PuyoOrientation CurrentOrientation { get; set; } = PuyoOrientation.Up;

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
        private PuyoColor[,] Playfield { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private PuyoPair CurrentPair { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private int[,] CurrentPosition { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private Stack<PuyoPair> CurrentPocket { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int TargetScore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int GameTickMS { get; set; } = 800;
        
        /// <summary>
        /// 
        /// </summary>
        private int CurrentDelta { get; set; }

        private Dictionary<ConsoleKey, PuyoInput> KeyToInput { get; } = new Dictionary<ConsoleKey, PuyoInput>()
        {
            {ConsoleKey.LeftArrow, PuyoInput.MoveLeft},
            {ConsoleKey.RightArrow, PuyoInput.MoveRight},
            {ConsoleKey.DownArrow, PuyoInput.SoftDrop},
            {ConsoleKey.UpArrow, PuyoInput.RotateClockwise},
            {ConsoleKey.A, PuyoInput.RotateClockwise},
            {ConsoleKey.S, PuyoInput.RotateAntiClockwise}
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

        private Dictionary<Difficulty, int> DifficultyToColors { get; } = new Dictionary<Difficulty, int>
        {
            { Difficulty.Beginner, 3},
            { Difficulty.Easy, 3},
            { Difficulty.Normal, 4},
            { Difficulty.Hard, 4},
            { Difficulty.Expert, 5}
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<PuyoColor, ConsoleColor> IndexToActiveColor { get; } = new Dictionary<PuyoColor, ConsoleColor>
        {
            { PuyoColor.Red, ConsoleColor.Red },
            { PuyoColor.Yellow, ConsoleColor.Yellow },
            { PuyoColor.Green, ConsoleColor.Green },
            { PuyoColor.Blue, ConsoleColor.Cyan },
            { PuyoColor.Purple, ConsoleColor.Magenta},
            { PuyoColor.None, ConsoleColor.Black}
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
            // Initialize Game Variables
            TargetScore = DifficultyToTargetScore[difficulty];
            Playfield = new PuyoColor[TotalRows, TotalColumns];
            TotalColors = DifficultyToColors[difficulty];
            
            // Populate Playfield
            for (var i = 0; i < TotalRows; i ++)
            for (var j = 0; j < TotalColumns; j++)
                Playfield[i, j] = PuyoColor.None;
            
            // Generate Initial Puyo Pieces
            GeneratePuyoPocket();
            SpawnNextPuyo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GeneratePuyoPocket()
        {
            var total = (int)Math.Pow(TotalColors * 2,2);
            var pairs = new List<PuyoPair>();
            CurrentPocket = new Stack<PuyoPair>();

            for (var h = 0; h < PUYO_POCKET_SIZE; h++)
            {
                for (var i = 0; i < TotalColors; i++)
                {
                    for (var j = 0; j < TotalColors; j++)
                    {
                        pairs.Add(new PuyoPair((PuyoColor) i, (PuyoColor) j));
                    }
                }

                for (var i = 0; i < pairs.Count; i++)
                {
                    var rand = pairs[RandomHelper.RandomInt(0, pairs.Count - 1)];
                    CurrentPocket.Push(rand);
                    pairs.Remove(rand);
                }
            }
        }

        private void SpawnNextPuyo()
        {
            CurrentPair = CurrentPocket.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override bool Update()
        {
            // Sleep
            Thread.Sleep(INPUT_CHECK_DELTA_MS);
            
            // Handle Input
            HandleKeyPress();
            
            // Handle Tick Iterations (Check for x iterations per tick)
            CurrentDelta += INPUT_CHECK_DELTA_MS;

            if (CurrentDelta >= GameTickMS)
            {
                CurrentDelta = 0;
                HandleGameTick();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleKeyPress()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                
                foreach (var k in KeyToInput)
                {
                    if (k.Key == key.Key)
                    {
                        HandleGameInput(k.Value);
                    }
                }
                DrawPlayfield();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        private void HandleGameInput(PuyoInput input)
        {
            switch (input)
            {
                case PuyoInput.MoveLeft:
                    return;
                
                case PuyoInput.MoveRight:
                    return;
                
                case PuyoInput.HardDrop:
                    return;
                
                case PuyoInput.SoftDrop:
                    return;
                
                case PuyoInput.RotateClockwise:
                    return;
                
                case PuyoInput.RotateAntiClockwise:
                    return;
            }
            
            Console.WriteLine(input);
        }

        private void RotatePuyo(bool clockwise)
        {
            var rot = clockwise ? (int)CurrentOrientation + 1 : (int)CurrentOrientation - 1;
            
            if (rot > 4) rot = 4;
            else if (rot < 0) rot = 0;

            CurrentOrientation = (PuyoOrientation) rot;
        }

        private void HandleGameTick()
        {
            if (CurrentPocket.Count == 0)
            {
                GeneratePuyoPocket();
            }
            
            DrawPlayfield();
        }

        private void DrawPlayfield()
        {
            TextManager.Clear();
            
            for (var i = 0; i < TotalRows; i++)
            {
                for (var j=0; j< TotalColumns; j++)
                {
                    DrawPuyo(Playfield[i, j]);

                    if (j == TotalColumns - 1)
                    {
                        TextManager.Write("_.", ConsoleColor.Black);

                        switch (i)
                        {
                            case 0:
                                TextManager.Write("Next:");
                                break;
                            
                            case 1:
                                DrawPuyo(CurrentPair.PuyoPartner);
                                break;
                            
                            case 2:
                                DrawPuyo(CurrentPair.PuyoCenter);
                                break;
                        }
                    }
                }
                TextManager.WriteLine();
            }
        }

        private void DrawPuyo(PuyoColor color)
        {
            switch (color)
            {
                case PuyoColor.None:
                    TextManager.WriteCharacter('_', ConsoleColor.DarkGray);
                    TextManager.WriteCharacter('.', ConsoleColor.Black);
                    return;
                
                default:
                    TextManager.WriteCharacter('O', IndexToActiveColor[color]);
                    TextManager.WriteCharacter('.', ConsoleColor.Black);
                    return;
                
            }
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
        private void DrawPuyoBoard()
        {
            
        }
    }
}
