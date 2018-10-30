using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSAssignments.GameLogic.Structures
{
    /// <summary>
    /// An area in a MineField used for gameplay.
    /// </summary>
    public enum MineArea
    {
        Unknown,
        Revealed,
        Safe,
        Mine
    }
}
