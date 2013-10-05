using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Lski.Toolbox.Objects {

	/// <summary>
	/// Simply an emtpy class designed to mark that a property should be ignored from certain processes. Normally because they are not required, but if read
	/// by default can cause a larse amount of overhead... e.g. a database call
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Property)]
	public class IgnorePropertyAttribute : System.Attribute {}

	/// <summary>
	/// Two extension methods based on the ToDictionary and FromDictionary in SubSonic by Rob Conery, however Ive added my functionality of caching the properties in the object types
	/// to avoid constant recalculation AND added the ability to exclude properties from the conversion (To prevent unnecessary overhead in expensive properties)
	/// </summary>
	public static class Generics {

		/// <summary>
		/// Returns true or false depending on whether the passed type if a Nullable type
		/// </summary>
		/// <param name="theType"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool IsNullableType(Type theType) {
			return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
		}

		/// <summary>
		/// Takes in an object of any type and checks to see if it is null, first against null, then against dbnull, finally whether it is nullable and has no value.
		/// </summary>
		/// <param name="o">The object to check</param>
		/// <returns></returns>
		public static bool IsNullComplete(Object o) {

			if (o == null || object.ReferenceEquals(o, DBNull.Value)) {
				return true;
			}


			if (IsNullableType(o.GetType())) {
				if (!(Boolean)o.GetType().GetProperty("HasValue").GetValue(o, null)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Creates a new object of type T and returns the new object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T CreateObject<T>() where T : new() {
			return new T();
		}

		/// <summary>
		/// Returns the system type the passed object was declared as. E.g. Object o = new CustomObject(); would return a type of 'Object'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static System.Type GetDeclaringType<T>(T o) {
			return typeof(T);
		}

		/// <summary>
		/// Creates a new list of the passed type, containing the objects passed. Useful for creating annoymous objects and then creating a list of that type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static List<T> MakeList<T>(params T[] items) {
			return new List<T>(items);
		}

		/// <summary>
		/// Calls a SHARED method of the class with the name that is passed, the class must be located within
		/// the same assembly (.dll or .exe) as the thing that calls this method
		/// Returns the item in the same type as the one stated for the method
		/// </summary>
		/// <param name="className">The name of the class to find call the method on</param>
		/// <param name="methodName">The name of the method to call</param>
		/// <param name="argList">A list of the arguments that the method is expecting</param>
		/// <returns>The value from the method requested</returns>
		/// <remarks></remarks>
		public static T CallSharedFunc<T>(string className, string methodName, object[] argList = null) {

			Assembly a = Assembly.GetEntryAssembly();
			// The assembly used by the callING method
			Type[] types = a.GetTypes();
			foreach (Type t in types) {

				// If the class is found create an object from it and use it to run the method
				if (t.IsClass && string.Compare(t.Name, className, true) == 0) {
					//Dim o = a.CreateInstance(t.FullName)
					return (T)t.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, null, argList);
				}
			}

			return default(T);
		}

		private const Int32 HashConst = 23;
		/// <summary>
		/// A generic create hash code method for bespoke objects, saves rewritting for each type
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		public static Int32 CreateHashCode(object[] input) {

			// Start with a prime number as its seed
			Int32 hash = 17;


			for (int i = 0; i <= input.Length - 1; i++) {
				// If not null then multiply the hash of the object being looked at and mulitply it with the const prime number
				if (input[i] != null) {
					hash = hash * HashConst + input[i].GetHashCode();
				}

			}

			return hash;

		}
	}
}
