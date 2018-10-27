using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson1.Helpers
{
    public static class RandomHelper
    {

        /// <summary>
        /// 
        /// </summary>
        private static Random RNG = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            // Throw exception if max is greater than min
            if (min > max)
                throw new Exception("Max value has to be greater than Min value.");

            return RNG.Next(max - min + 1) + min;
        }
    }
}
