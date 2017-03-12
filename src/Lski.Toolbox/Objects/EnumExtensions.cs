using System;

namespace Lski.Toolbox.Objects
{
    /// <summary>
    /// Extension method for Enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Used with enumerations containing the flags attribute, to see if a variable of that type contains a particular value... (saves having to remember
        /// the format for checking!)
        /// </summary>
        public static bool Has<T>(this Enum e, T value)
        {
            try
            {
                return (System.Convert.ToInt64(e) & System.Convert.ToInt64(value)) == System.Convert.ToInt64(value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Used with enumerations containing the flags attribute, to see if the passed value is the only value in the enumeration variable.
        /// </summary>
        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (Int64)(object)type == (Int64)(object)value;
            }
            catch
            {
                return false;
            }
        }
    }
}