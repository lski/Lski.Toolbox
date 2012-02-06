using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Lski.Objects;

namespace Lski.Data.Extensions {

	/// <summary>
	/// Partly based on the ext in SubSonic, but includes code I had created in a similar extension, where the property 
	/// </summary>
	public static class IDataReaderExt {

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
		private static HashSet<PropertyInfo> GetProperties(Type t, Boolean requiresWriteAccess) {

			HashSet<PropertyInfo> cachedProps;

			if (CachedProperties.ContainsKey(t))
				cachedProps = CachedProperties[t];
			else {

				cachedProps = new HashSet<PropertyInfo>();

				var qry = (from x in t.GetProperties(BindingFlags.Instance | BindingFlags.Public) select x);

				foreach (var x in qry) {

					// If not being selective or matching the writable criteria if desired add the property
					if (!requiresWriteAccess || (requiresWriteAccess && x.CanWrite)) {
						cachedProps.Add(x);
					}
				}

				CachedProperties.Add(t, cachedProps);
			}

			return cachedProps;
		}

		/// <summary>
		/// Coerces an IDataReader to try and load an object using name/property matching
		/// </summary>
		public static void Load<T>(this IDataReader rdr, T item) {

			HashSet<PropertyInfo> props = GetProperties(typeof(T), true);

			// Quickly create a linq useable version of reader names
			Dictionary<Int32, String> readerNames = new Dictionary<Int32, string>(rdr.FieldCount);
			for (int i = 0, n = rdr.FieldCount; i < n; i++) {
				readerNames.Add(i, rdr.GetName(i).ToLowerInvariant());
			}

			// Use linq to 'link' the properties of this object to the reader coming in, use joins to exclude non matching items
			var qry = (from rN in readerNames
					   join p in props on rN.Value equals p.Name.ToLowerInvariant()
					   select new { Property = p, ReaderPosition = rN.Key });

			// Run through the list trying to assign values
			foreach (var p in qry) {

				var val = rdr.GetValue(p.ReaderPosition);
				var valType = rdr.GetFieldType(p.ReaderPosition);

				// Only attempt to update id 
				if (!DBNull.Value.Equals(val)) {

					if (valType == typeof(Boolean)) {

						var value = val.ToString();
						p.Property.SetValue(item, (value == "1" || value == "True"), null);

					} else if (p.Property.PropertyType == typeof(Guid)) {

						p.Property.SetValue(item, rdr.GetGuid(p.ReaderPosition), null);

					} else {

						//try to assign it
						if (p.Property.PropertyType.IsAssignableFrom(valType)) {
							p.Property.SetValue(item, val, null);
						} else {

							try {
								p.Property.SetValue(item, val.ChangeTypeTo(p.Property.PropertyType), null);
							} catch {
								// Stub, in case of the very unlikely event theres an error setting the value
							}
						}
						
					}
				}
			}
		}

		/// <summary>
		/// Coerces an IDataReader to try and load an object using name/property matching
		/// </summary>
		/// <param name="item">The object to fill</param>
		/// <param name="propertyNameMappings">A dictionary where the property names of the object (keys) match up with the field names of the reader (values)</param>
		public static void Load<T>(this IDataReader rdr, T item, Dictionary<String,String> propertyNameMappings) {

			HashSet<PropertyInfo> props = GetProperties(typeof(T), true);

			// Quickly create a linq useable version of reader names
			Dictionary<Int32, String> readerNames = new Dictionary<Int32,string>(rdr.FieldCount);
			for (int i = 0, n = rdr.FieldCount; i < n; i++) {
				readerNames.Add(i, rdr.GetName(i).ToLowerInvariant());
			}

			// Use linq to 'link' the properties of this object to the reader coming in, use joins to exclude non matching items
			var qry = (from rN in readerNames
					   join pnm in propertyNameMappings on rN.Value equals pnm.Value.ToLowerInvariant()
					   join p in props on pnm.Key.ToLowerInvariant() equals p.Name.ToLowerInvariant()
					   select new {Property = p, ReaderPosition = rN.Key});

			// Run through the list trying to assign values
			foreach (var p in qry) {

				var val = rdr.GetValue(p.ReaderPosition);
				var valType = rdr.GetFieldType(p.ReaderPosition);

				// Only attempt to update id 
				if (!DBNull.Value.Equals(val)) {

					if (valType == typeof(Boolean)) {
						
						var value = val.ToString();
						p.Property.SetValue(item, (value == "1" || value == "True"), null);
					
					} else if (p.Property.PropertyType == typeof(Guid)) {

						p.Property.SetValue(item, rdr.GetGuid(p.ReaderPosition), null);
					
					} else {

						try {
							p.Property.SetValue(item, val, null);
						} catch { 
							// Stub, in case of the very unlikely event theres an error setting the value
						}
					}
				}
			}
		}

		/// <summary>
		/// Simply creates a new object of the type T and fills it with the data from the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		/// <returns></returns>
		public static T CreateObject<T>(this IDataReader r) where T : new() {

			var obj = new T();
			r.Load<T>(obj);
			return obj;
		}

		/// <summary>
		/// Simply creates a new object of the type T and fills it with the data from the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		/// <param name="propertyNameMappings">A dictionary where the field names of the reader (keys) match up with the property names of the object (values)</param>
		/// <returns></returns>
		public static T CreateObject<T>(this IDataReader r, Dictionary<String, String> propertyNameMappings) where T : new() {

			var obj = new T();
			r.Load<T>(obj, propertyNameMappings);
			return obj;
		}
	}

}