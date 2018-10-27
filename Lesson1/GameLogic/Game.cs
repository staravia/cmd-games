using Lesson1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson1.GameLogic
{
    public abstract class Game
    {
        private bool ContinueGame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal void StartGame()
        {
            bool notFailed;
            ContinueGame = true;
            WriteInstructions();
            while ((notFailed = Update()) && ContinueGame) ;

            // Display a message when the game ends
            DisplayOver(!notFailed);
        }

        /// <summary>
        /// 
        /// </summary>
        internal abstract bool Update();

        /// <summary>
        /// 
        /// </summary>
        internal void EndGame() => ContinueGame = false;

        /// <summary>
        /// 
        /// </summary>
        internal abstract void WriteInstructions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failed"></param>
        private void DisplayOver(bool failed)
        {
            TextManager.WriteLineBreak();
            TextManager.WriteLine();

            // Display appropriate message when the game ends.
            if (failed)
                TextManager.WriteLine("Game Over.", ConsoleColor.Red);
            else
                TextManager.WriteLine("You win!", ConsoleColor.Green);

            TextManager.WriteLine("Enter anything to continue.");
            TextManager.WriteLine();
            TextManager.ReadLine();
            TextManager.Clear();
        }
    }
}
