using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lski.Toolbox.Objects {

	public static class Convert {

		/// <summary>
		/// Converts this object into a dictionary object
		/// </summary>
		public static Dictionary<string, object> ToDictionary(object o) {

			var result = new Dictionary<string, object>();
			var props = GetProperties(o.GetType(), false);

			foreach (PropertyInfo pi in props) {
				try {
					result.Add(pi.Name, pi.GetValue(o, null));
				} catch {
					//Stub ignore error
				}
			}
			return result;
		}

		/// <summary>
		/// Attempts to fill the object using the dictionary keys to match properties in the object.
		/// </summary>
		public static T FromDictionary<T>(Dictionary<string, object> settings, T obj) where T : class {

			var props = GetProperties(obj.GetType(), true);

			foreach (PropertyInfo pi in props) {

				if (settings.ContainsKey(pi.Name)) {

					try {
						pi.SetValue(obj, settings[pi.Name], null);
					} catch {
						//Stub ignore error
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// Caches the properties so they have to be re-calculated each time
		/// </summary>
		private static readonly ConcurrentDictionary<Type, HashSet<PropertyInfo>> _cached = new ConcurrentDictionary<Type, HashSet<PropertyInfo>>();

		/// <summary>
		/// Returns the property list for this object, either from cache or calculated if this is the first time its called
		/// </summary>
		/// <param name="t"></param>
		/// <param name="requiresWriteAccess">If true will only return properties that can be written to, otherwise all</param>
		/// <returns></returns>
		private static IEnumerable<PropertyInfo> GetProperties(Type t, Boolean requiresWriteAccess) {

			HashSet<PropertyInfo> cachedProps;

			if (!_cached.TryGetValue(t, out cachedProps)) {

				cachedProps = new HashSet<PropertyInfo>(from x in t.GetRuntimeProperties() where x.CanRead && x.CanWrite select x);

				_cached.TryAdd(t, cachedProps);
			}

			if (requiresWriteAccess) {
				return cachedProps.Where(x => x.CanWrite);
			}

			return cachedProps;
		}
	}
}