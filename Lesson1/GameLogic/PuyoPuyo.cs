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
        private PuyoPair NextPair { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private Coordinate CurrentPosition { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private Stack<PuyoPair> CurrentPocket { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private int CurrentScore { get; set; }

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
            { ConsoleKey.LeftArrow, PuyoInput.MoveLeft },
            { ConsoleKey.RightArrow, PuyoInput.MoveRight },
            { ConsoleKey.DownArrow, PuyoInput.SoftDrop },
            { ConsoleKey.UpArrow, PuyoInput.RotateClockwise },
            { ConsoleKey.A, PuyoInput.RotateAntiClockwise },
            { ConsoleKey.S, PuyoInput.RotateClockwise },
            { ConsoleKey.Spacebar, PuyoInput.HardDrop }
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
            CurrentPosition = new Coordinate(0,0);
            
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
            NextPair = CurrentPocket.Pop();
            CurrentPair = NextPair;
            CurrentPosition.Row = 0;
            CurrentPosition.Column = 3;
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
            var partnerpos = GetCurrentPartnerPosition();
            
            switch (input)
            {
                case PuyoInput.MoveLeft:
                    if (partnerpos.Column > 0 && Playfield[Math.Max(0,partnerpos.Row), partnerpos.Column - 1] == PuyoColor.None)
                        if (CurrentPosition.Column > 0 && Playfield[CurrentPosition.Row, CurrentPosition.Column - 1] == PuyoColor.None)
                            CurrentPosition.Column--;
                    return;
                
                case PuyoInput.MoveRight:
                    if (partnerpos.Column < TotalColumns - 1 && Playfield[Math.Max(0,partnerpos.Row), partnerpos.Column + 1] == PuyoColor.None)
                        if (CurrentPosition.Column < TotalColumns - 1 && Playfield[CurrentPosition.Row, CurrentPosition.Column + 1] == PuyoColor.None)
                            CurrentPosition.Column++;
                    return;
                
                case PuyoInput.HardDrop:
                    CurrentPosition.Row = 0;
                    return;
                
                case PuyoInput.SoftDrop:
                    if (!IsTouchingGround())
                        CurrentPosition.Row++;
                    return;
                
                case PuyoInput.RotateClockwise:
                    RotatePuyo(true);
                    return;
                
                case PuyoInput.RotateAntiClockwise:
                    RotatePuyo(false);
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clockwise"></param>
        private void RotatePuyo(bool clockwise)
        {
            var rot = clockwise ? (int)CurrentOrientation + 1 : (int)CurrentOrientation - 1;
            
            if (rot == 4) rot = 0;
            else if (rot == -1) rot = 3;

            CurrentOrientation = (PuyoOrientation) rot;
            
            var partnerpos = GetCurrentPartnerPosition();
            if (partnerpos.Column > TotalColumns - 1)
                CurrentPosition.Column = TotalColumns - 2;

            if (partnerpos.Column < 0)
                CurrentPosition.Column = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleGameTick()
        {
            if (CurrentPocket.Count == 0)
            {
                GeneratePuyoPocket();
            }

            if (IsTouchingGround())
            {
                var partnerpos = GetCurrentPartnerPosition();
                Playfield[CurrentPosition.Row, CurrentPosition.Column] = CurrentPair.PuyoCenter;
                Playfield[partnerpos.Row, partnerpos.Column] = CurrentPair.PuyoPartner;
                SpawnNextPuyo();
            }
            else
            {
                CurrentPosition.Row ++;
            }
            
            DrawPlayfield();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsTouchingGround()
        {
            var partnerpos = GetCurrentPartnerPosition();

            if (CurrentPosition.Row + 1 >= TotalRows || partnerpos.Row + 1 >= TotalRows)
                return true;

            if (Playfield[CurrentPosition.Row + 1, CurrentPosition.Column] != PuyoColor.None)
                return true;

            if (Playfield[partnerpos.Row + 1, partnerpos.Column] != PuyoColor.None)
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Coordinate GetCurrentPartnerPosition()
        {
            switch (CurrentOrientation)
            {
                case PuyoOrientation.Up:
                    return new Coordinate(CurrentPosition.Row - 1, CurrentPosition.Column);
                
                case PuyoOrientation.Right:
                    return new Coordinate(CurrentPosition.Row, CurrentPosition.Column + 1);
                
                case PuyoOrientation.Down:
                    return new Coordinate(CurrentPosition.Row + 1, CurrentPosition.Column);
                
                case PuyoOrientation.Left:
                    return new Coordinate(CurrentPosition.Row, CurrentPosition.Column - 1);
                
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawPlayfield()
        {
            TextManager.Clear();
            var partnerpos = GetCurrentPartnerPosition();
            
            
            for (var i = 0; i < TotalRows; i++)
            {
                for (var j=0; j< TotalColumns; j++)
                {
                    if (CurrentPosition.Row == i && CurrentPosition.Column == j)
                    {
                        DrawPuyo(CurrentPair.PuyoCenter, true);  
                    }                    
                    else if (partnerpos.Row == i && partnerpos.Column == j)
                        DrawPuyo(CurrentPair.PuyoPartner, true);
                            
                    else
                        DrawPuyo(Playfield[i, j]);

                    if (j == TotalColumns - 1)
                    {
                        TextManager.Write("_.", ConsoleColor.Black);

                        switch (i)
                        {
                            case 0:
                                TextManager.Write("Score:");
                                break;
                            
                            case 1:
                                TextManager.Write(CurrentScore.ToString(), ConsoleColor.Red);
                                break;
                            
                            
                            case 3:
                                TextManager.Write("Next:");
                                break;
                            
                            case 4:
                                DrawPuyo(CurrentPair.PuyoCenter);
                                break;
                            
                            case 5:
                                DrawPuyo(CurrentPair.PuyoCenter);
                                break;
                        }
                    }
                }
                TextManager.WriteLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        private void DrawPuyo(PuyoColor color, bool current = false)
        {
            switch (color)
            {
                case PuyoColor.None:
                    TextManager.WriteCharacter('_', ConsoleColor.DarkGray);
                    TextManager.WriteCharacter('.', ConsoleColor.Black);
                    return;
                
                default:
                    TextManager.WriteCharacter(current ? 'X' : 'O', IndexToActiveColor[color]);
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
            WaitForInput();
        }
    }
}
