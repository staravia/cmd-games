namespace CSAssignments.GameLogic.Structures
{
    /// <summary>
    /// Represents a coordinate in the MineSweeper game.
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Row
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Column
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Create a new coordinate
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public Coordinate (int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
