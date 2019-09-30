using CSAssignments.Helpers;
using System;

namespace CSAssignments.GameLogic
{
    /// <summary>
    /// Base Game Class.
    /// </summary>
    public abstract class Game
    {
        /// <summary>
        /// Determines if the game should keep looping.
        /// </summary>
        private bool ContinueGame { get; set; }

        /// <summary>
        /// This will start the game
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
        /// Main game loop.
        /// </summary>
        internal abstract bool Update();

        /// <summary>
        /// Ends game.
        /// </summary>
        internal void EndGame() => ContinueGame = false;

        /// <summary>
        /// Write instructions whenever the player starts this game.
        /// </summary>
        internal abstract void WriteInstructions();

        /// <summary>
        /// Will display a message once the game is over.
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

            // Read user input
            TextManager.WriteLine("Enter anything to continue.");
            TextManager.WriteLine();
            TextManager.ReadLine();
            TextManager.Clear();
        }
    }
}
