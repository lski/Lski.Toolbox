using System.Collections.Generic;
using System.Text;

namespace Lski.Toolbox.Txt
{
    /// <summary>
    /// Extensions methods for StringBuilder
    /// </summary>
    public static class StringBuilderExt
    {
        /// <summary>
        /// Allows a StringBuilder to looped in the same way as a string. Returning each character in turn.
        /// </summary>
        public static IEnumerable<char> AsEnumerable(this StringBuilder sb)
        {
            for (int i = 0, n = sb.Length; i < n; i++)
            {
                yield return sb[i];
            }

            yield break;
        }
    }
}