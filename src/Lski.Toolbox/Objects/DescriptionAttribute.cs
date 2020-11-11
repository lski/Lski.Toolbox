using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Lski.Toolbox.Objects
{
    /// <summary>
    /// This allows a human readable string to be added to an enumeration value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Holds the string value for a value in an enum.
        /// </summary>
        public String Value { get; set; }

        public DescriptionAttribute(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Provides extension methods to Enum values that have DescriptionAttribute attached.
    /// </summary>
    public static class DescriptionAttributeExt
    {
        /// <summary>
        /// Caching to prevent recalling
        /// </summary>
        private static readonly ConcurrentDictionary<Enum, string> _cached = new ConcurrentDictionary<Enum, string>();

        /// <summary>
        /// Returns a string, if the enumeration value has a &apos;Description&apos; attribute it will return the value from that, otherwise returns a string version of the enumeration value itself.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetDescription(this Enum e)
        {
            string output = null;

            // Check first in our cached results...
            if (!_cached.TryGetValue(e, out output))
            {
                //Look for our 'DescriptionAttribute' in the field's custom attributes
                var type = e.GetType();
                var fi = type.GetTypeInfo().GetDeclaredField(e.ToString());
                var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                output = attrs.Select(a => a.Value).FirstOrDefault() ?? e.ToString();

                // Add to cache
                _cached.TryAdd(e, output);
            }

            return output;
        }
    }
}