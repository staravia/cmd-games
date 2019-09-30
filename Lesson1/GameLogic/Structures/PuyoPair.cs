namespace CSAssignments.GameLogic.Structures
{
    public struct PuyoPair
    {
        /// <summary>
        /// 
        /// </summary>
        public PuyoColor PuyoCenter { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public PuyoColor PuyoPartner { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public PuyoPair(PuyoColor first, PuyoColor second)
        {
            PuyoCenter = first;
            PuyoPartner = second;
        }
    }
}