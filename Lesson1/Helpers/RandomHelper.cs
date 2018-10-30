using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSAssignments.Helpers
{
    /// <summary>
    /// This class is used to generate random numbers or manipulate probability.
    /// </summary>
    public static class RandomHelper
    {

        /// <summary>
        /// Random Number Generator
        /// </summary>
        private static Random RNG = new Random();

        /// <summary>
        /// Generate a random integer
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            if (min > max)
            {
                var temp = min;
                max = min;
                min = max;
            }

            return RNG.Next(max - min + 1) + min;
        }

        /// <summary>
        /// Generate a random float
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomFloat(float min, float max)
        {
            if (min > max)
            {
                var temp = min;
                max = min;
                min = max;
            }

            return min + RNG.Next(((int)(max * 1000) - (int)(min * 1000)) + 1) / 1000f;
        }
    }
}
