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
        internal virtual void StartGame()
        {
            ContinueGame = true;
            WriteInstructions();
            while (Update() || ContinueGame) ;
        }

        /// <summary>
        /// 
        /// </summary>
        internal abstract bool Update();

        /// <summary>
        /// 
        /// </summary>
        internal virtual void EndGame() => ContinueGame = false;

        /// <summary>
        /// 
        /// </summary>
        internal abstract void WriteInstructions();
    }
}
