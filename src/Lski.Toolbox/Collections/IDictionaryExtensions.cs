using System;
using System.Collections.Generic;

namespace Lski.Toolbox.Collections
{
    /// <summary>
    /// Adds functions to IDictionary type
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Returns the value found using the key passed, otherwise returns the default() of the value type in the IDictionary object (e.g. null) without throwing an exception.
        /// </summary>
        /// <returns>Value found or the default for the value type in the dictionary</returns>
        /// <remarks>
        /// Its important to note that the default for the value type of the IDictionary does not mean the key does not exist 
        /// because that the key could exist and the value returned could in fact just be the default value for that type.
        /// 
        /// Therefore this method is designed as a convenience to those that dont care about the presense of a key in the IDictionary, just the value returned, or
        /// they use ContainsKey(key) prior to this call.
        /// </remarks>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out TValue value) ? value : default(TValue);
        }
    }
}