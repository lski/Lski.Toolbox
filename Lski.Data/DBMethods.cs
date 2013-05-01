using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Configuration;

using Lski.Txt;

namespace Lski.Data {

	/// <summary>
	/// Provides a selection of basic functions for working with databases
	/// </summary>
	/// <remarks></remarks>
	public class DBMethods {

		/// <summary>
		/// Returns a date time obj with the transalated value for a FoxProEmptyDate (which is equiv to 1899/12/30)
		/// </summary>
		/// <returns></returns>
		public static DateTime FoxProEmptyDate() {
			return new DateTime(1899, 12, 30);
		}

		/// <summary>
		/// Uses the passed command object and reader object to fill the commands parameter values with values coming from the datareader, ready to be used. Fairly database independant
		/// as long as datatypes match. Ignores values not found in the parameter list that are present in the reader.
		/// </summary>
		/// <param name="comm"></param>
		/// <param name="reader"></param>
		/// <param name="errorOnCast">If true, then when a casting error occurs it throws a casting error</param>
		/// <exception cref="Exception">Thrown if data types are not compatible</exception>
		/// <remarks></remarks>
		public static void FillParameterValues(DbCommand comm, DbDataReader reader, Boolean errorOnCast = false) {

			// Clean the parameter values in case not matched below
			foreach (DbParameter p in comm.Parameters) {
				p.Value = DBNull.Value;
			}

			// Do a double loop, because the only common ref is SourceColumn, which because the DbParameterCollection type does not work with linq means double loop
			for (int i = 0, n = reader.FieldCount; i < n; i++) {

				foreach(DbParameter p in comm.Parameters) {

					if(p.SourceColumn.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase)) {

						if (errorOnCast) {

							try {
								//If a field of that name exists then get the value before continuing
								p.Value = reader.GetValue(i);
							} catch (Exception ex) {
								throw new InvalidCastException(String.Format("There was an error casting the value '{0}' to a {1}", reader.GetValue(i), reader.GetFieldType(i)), ex);
							}

						} else {

							//Attempt to set the value
							try {
								p.Value = reader.GetValue(i);
							} catch {}
						}

						break;
					}
				}
			}
		}

		/// <summary>
		/// Uses the passed command object and dictionary to fill the commands parameter values with values coming from the datareader, ready to be used. Fairly database independant
		/// as long as datatypes match. Ignores values not found in the parameter list that are present in the dictionary.
		/// </summary>
		/// <param name="comm"></param>
		/// <param name="reader"></param>
		/// <param name="errorOnCast">If true, then when a casting error occurs it throws a casting error</param>
		/// <exception cref="Exception">Thrown if data types are not compatible</exception>
		/// <remarks></remarks>
		public static void FillParameterValues(DbCommand comm, Dictionary<String, Object> obj, Boolean errorOnCast = false) {

			foreach (DbParameter para in comm.Parameters) {

				// Reset the value, in case its not in the list
				para.Value = DBNull.Value;


				// Try find if there is a value to fill with 
				var o = obj.SingleOrDefault(x => x.Key == para.SourceColumn);

				if (o.Equals(default(KeyValuePair<String, Object>)))
					continue;

				if (errorOnCast) {

					try {
						//If a field of that name exists then get the value before continuing
						para.Value = o.Value;
					} catch (Exception ex) {
						throw new InvalidCastException(String.Format("There was an error casting the value '{0}' to a {1}", o.Value, (o.Value == null ? "NULL" : o.Value.GetType().ToString())), ex);
					}

				} else {

					try {
						//If a field of that name exists then get the value before continuing
						para.Value = (o.Value ?? DBNull.Value);
					} catch {
						para.Value = DBNull.Value;
					}

				}
			}
		}
	}
}