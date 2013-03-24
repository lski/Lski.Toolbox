using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Lski.Objects {
	
	// TODO: Do testing on the current version of ConvertTo to decide whether or not to depreciate this

	public static class Convert {

		/// <summary>
		/// Returns an Object with the specified Type and whose value is equivalent to the specified object.
		/// </summary>
		/// <param name="value">An Object that implements the IConvertible interface.</param>
		/// <returns>
		/// An object whose Type is conversionType (or conversionType's underlying type if conversionType is Nullable) and whose 
		/// value is equivalent to value. -or- a null reference, if value is a null reference and conversionType is not a value type.
		/// </returns>
		/// <remarks>
		/// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
		/// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
		/// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
		/// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
		/// This method was written by Peter Johnson at: http://aspalliance.com/author.aspx?uId=1026.
		/// </remarks>
		public static object ChangeTypeTo<T>(this object value) {
			Type conversionType = typeof(T);
			return ChangeTypeTo(value, conversionType);
		}

		public static object ChangeTypeTo(this object value, Type conversionType) {

			// Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're checking properties on conversionType below.
			if (conversionType == null)
				throw new ArgumentNullException("conversionType");

			// If it's not a nullable type, just pass through the parameters to Convert.ChangeType

			if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
				// It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
				// InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
				// determine what the underlying type is
				// If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
				// have a type--so just return null
				// Note: We only do this check if we're converting to a nullable type, since doing it outside
				// would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
				// value is null and conversionType is a value type.
				if (value == null)
					return null;

				// It's a nullable type, and not null, so that means it can be converted to its underlying type,
				// so overwrite the passed-in conversion type with this underlying type
				NullableConverter nullableConverter = new NullableConverter(conversionType);
				conversionType = nullableConverter.UnderlyingType;

			} else if (conversionType == typeof(Guid)) {

				return new Guid(value.ToString());

			} else if (conversionType == typeof(Int64) && value.GetType() == typeof(int)) {
				//there is an issue with SQLite where the PK is ALWAYS int64. If this conversion type is Int64
				//we need to throw here - suggesting that they need to use LONG instead


				throw new InvalidOperationException("Can't convert an Int64 (long) to Int32(int). If you're using SQLite - this is probably due to your PK being an INTEGER, which is 64bit. You'll need to set your key to long.");
			}

			// Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
			// nullable type), pass the call on to Convert.ChangeType
			return System.Convert.ChangeType(value, conversionType);
		}

		/// <summary>
		/// Caches the properties so they have to be re-calculated each time
		/// </summary>
		private static Dictionary<Type, HashSet<PropertyInfo>> CachedProperties = new Dictionary<Type, HashSet<PropertyInfo>>();

		/// <summary>
		/// Returns the property list for this object, either from cache or calculated if this is the first time its called
		/// </summary>
		/// <param name="t"></param>
		/// <param name="requiresWriteAccess">If true will only return properties that can be written to, otherwise all</param>
		/// <returns></returns>
		private static HashSet<PropertyInfo> GetProperties(Type t, Boolean requiresWriteAccess, Boolean excludeProps) {

			HashSet<PropertyInfo> cachedProps;

			if (CachedProperties.ContainsKey(t))
				cachedProps = CachedProperties[t];
			else {

				var attrToExclude = typeof(IgnorePropertyAttribute);

				cachedProps = new HashSet<PropertyInfo>();

				var qry = (from x in t.GetProperties(BindingFlags.Instance | BindingFlags.Public) select x);

				foreach (var x in qry) {

					// If not being selective or matching the writable criteria if desired add the property
					if (!requiresWriteAccess || (requiresWriteAccess && x.CanWrite)) {

						// If its marked as excludeProps then check if its got a IgnorePropertyAttribute
						if (excludeProps && !x.GetCustomAttributes(attrToExclude, false).Any())
							cachedProps.Add(x);
						else
							cachedProps.Add(x);
					}

				}

				CachedProperties.Add(t, cachedProps);
			}

			return cachedProps;
		}

		/// <summary>
		/// Converts this object into a dictionary object, witht the keys being the public properties and the values being the value stored in that property.
		/// If a property has a IgnorePropertyAttribute then it is not collected, unless allProps is true.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="allProps">States whether to get ALL props or just those not with the IgnorePropertyAttribute</param>
		/// <returns></returns>
		/// <remarks>
		/// Converts this object into a dictionary object, witht the keys being the public properties and the values being the value stored in that property.
		/// If a property has a IgnorePropertyAttribute then it is not collected, unless allProps is true. Based on the one in Subsonic, with except with the
		/// ability to exclude certain properties due to overhead of calling them.
		/// </remarks>
		public static Dictionary<string, object> ToDictionary(this object o, Boolean excludeProps = true) {

			Dictionary<string, object> result = new Dictionary<string, object>();
			HashSet<PropertyInfo> props = GetProperties(o.GetType(), false, excludeProps);

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
		/// Attempts to fill the
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="settings"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T FromDictionary<T>(this Dictionary<string, object> settings, T obj) where T : class {

			HashSet<PropertyInfo> props = GetProperties(obj.GetType(), true, false);

			foreach (PropertyInfo pi in props) {

				if (settings.ContainsKey(pi.Name)) {
					pi.SetValue(obj, settings[pi.Name], null);
				}

			}
			return obj;
		}
	}
}
