using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Util
{
    /// <summary>
    /// Helper class that contains various sequence generator functions.
    /// </summary>
    public static class SequenceGenerator
    {
        /// <summary>
        /// Given start and end, it generates a series of integers that the specified function "f" yields at each integer.
        /// In other words, the generated sequence "a" satisfies the following equations
        /// a[0] = f(from)
        /// a[a.length - 1] = f(to - 1)
        /// </summary>
        /// <param name="f"></param>
        /// <param name="to">The end point </param>
        /// <param name="from"></param>
        public static IEnumerable<int> GenerateIntegerSequence(Func<int, int> f, int to, int from = 0)
        {
            for(int i = from; i < to; ++i)
                yield return f(i);
        }
    }
}
