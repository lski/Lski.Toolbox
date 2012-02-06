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

///// <summary>
//        /// Produces a basic implementation of an sql statement that deletes the duplicate records in a table based on the fields passed.
//        /// </summary>
//        /// <param name="pk">The primary key column</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string DeleteDuplicatesSql(string pk, string table, List<string> fieldsToMatch) {
//            return DeleteDuplicatesSql(pk, table, fieldsToMatch.ToArray());
//        }

//        /// <summary>
//        /// Produces a basic implementation of an sql statement that deletes the duplicate records in a table based on the fields passed.
//        /// </summary>
//        /// <param name="pk">The primary key column</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string DeleteDuplicatesSql(string pk, string table, string[] fieldsToMatch) {

//            String fields = string.Join(",", fieldsToMatch);
//            StringBuilder linkList = new StringBuilder();

//            // Create the link between the good and bad tables on the fields the user wants to match
//            // finishes with 'and' always because there is another statement after the one being created.
//            foreach (var f in fieldsToMatch) {
//                linkList.Append("good_rows.").Append(f).Append(" = bad_rows.").Append(f).Append(" and ");
//            }

//            return "delete bad_rows from " + table + " as bad_rows inner join (select " + fields + ", MIN(" + pk + ") as min_" + pk + " from " + table + " group by " + fields + " having count(*) > 1) as good_rows on " + linkList + " good_rows.min_" + pk + " <> bad_rows." + pk;
//        }

//        /// <summary>
//        /// Creates an Insert Command object, using the tableControl passed to create the correct type of Command and Parameter objects
//        /// </summary>
//        /// <param name="tableName">The name of the table to fill</param>
//        /// <param name="fieldsAndParameters">The list of field names and parameter names (without database specific styles) to be included in the insert command. 
//        /// Note the Key equals field and Value equals parameter.
//        /// </param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual DbCommand CreateInsertCommand(string tableName, Dictionary<string, string> fieldsAndParameters) {

//            return CreateInsertCommand(tableName, fieldsAndParameters, DbType.Int32);
//        }

//        /// <summary>
//        /// Creates an Insert Command object, using the tableControl passed to create the correct type of Command and Parameter objects
//        /// </summary>
//        /// <param name="tableName">The name of the table to fill</param>
//        /// <param name="fieldsAndParameters">The list of field names and parameter names (without database specific styles) to be included in the insert command. 
//        /// Note the Key equals field and Value equals parameter.
//        /// </param>
//        /// <param name="indentityDataType">States the datatype of the indentity value to return when the command is run. 
//        /// If null, the command does not try and retrieve the identity value of the insert on execute, but the normal amount of rows effected result.</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual DbCommand CreateInsertCommand(string tableName, Dictionary<string, string> fieldsAndParameters, DbType? indentityDataType) {

//            List<string> unprocessedFieldList = new List<string>();
//            List<string> processedFieldList = new List<string>();
//            List<string> parametersList = new List<string>();

//            // Now create a version of each field in a format desired for parameters and field names
//            foreach (var field in fieldsAndParameters) {

//                unprocessedFieldList.Add(field.Key);
//                processedFieldList.Add(this.SqlFieldName(field.Key));
//                parametersList.Add(this.SqlParameter(field.Value));
//            }

//            String qry = String.Format("insert into {0} ({1}) values ({2}); ", this.SqlTableName(tableName), string.Join(",", processedFieldList.ToArray()), string.Join(",", parametersList.ToArray()));

//            // If an identity type added then add the additional code for getting the identity created for this record
//            if (indentityDataType.HasValue) 
//                qry = this.SqlGetIdentity(qry, indentityDataType.Value);

//            DbCommand comm = CreateCommand(qry);
//            DbParameter para = null;

//            // Dynamically add the parameters
//            for (Int32 i = 0; i <= (processedFieldList.Count - 1); i++) {

//                para = CreateParameter(parametersList[i]);
//                para.SourceColumn = unprocessedFieldList[i];
//                para.SourceVersion = DataRowVersion.Current;
//                comm.Parameters.Add(para);
//            }

//            return comm;
//        }

//        /// <summary>
//        /// Creates an Insert Command object, using the tableControl passed to create the correct type of Command and Parameter objects
//        /// </summary>
//        /// <param name="tableName">The name of the table to fill</param>
//        /// <param name="fieldList">The list of field names (without enclosing brackets) to be included in the insert command</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual DbCommand CreateInsertCommand(string tableName, List<string> fieldList) {

//            return CreateInsertCommand(tableName, fieldList, DbType.Int32);
//        }

//        /// <summary>
//        /// Creates an Insert Command object, using the tableControl passed to create the correct type of Command and Parameter objects
//        /// </summary>
//        /// <param name="tableName">The name of the table to fill</param>
//        /// <param name="fieldList">The list of field names (without enclosing brackets) to be included in the insert command</param>
//        /// <param name="indentityDataType">States the datatype of the indentity value to return when the command is run. 
//        /// If null, the command does not try and retrieve the identity value of the insert on execute, but the .</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual DbCommand CreateInsertCommand(string tableName, List<string> fieldList, DbType? indentityDataType) {

//            List<string> processedFieldList = new List<string>();
//            List<string> parametersList = new List<string>();

//            // Now create a version of each field in a format desired for parameters and field names
//            foreach (var field in fieldList) {

//                processedFieldList.Add(this.SqlFieldName(field));
//                parametersList.Add(this.SqlParameter(field));
//            }

//            String qry = String.Format("insert into {0} ({1}) values ({2}); ", this.SqlTableName(tableName), string.Join(",", processedFieldList.ToArray()), string.Join(",", parametersList.ToArray()));

//            // If an identity type added then add the additional code for getting the identity created for this record
//            if (indentityDataType.HasValue) 
//                qry = this.SqlGetIdentity(qry, indentityDataType.Value);

//            DbCommand comm = CreateCommand(qry);
//            DbParameter para = null;

//            // Dynamically add the parameters

//            for (Int32 i = 0; i <= (processedFieldList.Count - 1); i++) {
				
//                para = CreateParameter(parametersList[i]);
//                para.SourceColumn = fieldList[i];
//                para.SourceVersion = DataRowVersion.Current;
//                comm.Parameters.Add(para);
//            }

//            return comm;

//        }

///// <summary>
//        /// Creates an insert command object, which includes the parameter objects being created based on the reader being passed in.
//        /// </summary>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual DbCommand CreateInsertCommand(DbDataReader reader, String tableName) {

//            DbCommand comm = this.CreateCommand();
//            DbParameter para = null;
//            String[] fieldNames = new String[reader.FieldCount];
//            String[] paramNames = new String[reader.FieldCount];

//            // Dynamically create the parameters
//            for (int i = 0; i <= (reader.FieldCount) - 1; i++) {

//                fieldNames[i] = reader.GetName(i);
//                paramNames[i] = this.SqlParameter(fieldNames[i]);

//                para = this.CreateParameter();
//                para.ParameterName = paramNames[i];
//                para.DbType = TypeConvertor.ToDbType(reader.GetFieldType(i));
//                para.SourceVersion = DataRowVersion.Current;
//                para.SourceColumn = fieldNames[i];

//                // Now ensure it has the correct format for the current database style.
//                fieldNames[i] = this.SqlFieldName(fieldNames[i]);

//                comm.Parameters.Add(para);

//            }

//            comm.CommandText = String.Concat("insert into ", this.SqlTableName(tableName), " (") + String.Join(",", fieldNames) + ") VALUES (" + String.Join(",", paramNames) + ");";

//            return comm;

//        }

///// <summary>
//        /// Creates an sql insert statement using the criteria passed
//        /// </summary>
//        /// <param name="tableName">The name of the table to insert into</param>
//        /// <param name="values">The values to be inserted into the table</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string CreateInsertSQL(string tableName, string[] values) {

//            return CreateInsertSQL(tableName, values, null, true);

//        }

//        /// <summary>
//        /// Creates an sql insert statement using the criteria passed
//        /// </summary>
//        /// <param name="tableName">The name of the table to insert into</param>
//        /// <param name="values">The values to be inserted into the table</param>
//        /// <param name="parseNames">If set to true, the Create statement will NOT try to parse each field and table name through the parser, but run 'as is' 
//        /// to reduce processing time. NOTE: Does not effect values either way</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string CreateInsertSQL(string tableName, string[] values, bool parseNames) {

//            return CreateInsertSQL(tableName, values, null, parseNames);

//        }

//        /// <summary>
//        /// Creates an sql insert statement using the criteria passed
//        /// </summary>
//        /// <param name="tableName">The name of the table to insert into</param>
//        /// <param name="values">The values to be inserted into the table</param>
//        /// <param name="fields">The field names of the table columns</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string CreateInsertSQL(string tableName, string[] values, string[] fields) {

//            return CreateInsertSQL(tableName, values, fields, true);
//        }

//        /// <summary>
//        /// Creates an sql insert statement using the criteria passed
//        /// </summary>
//        /// <param name="tableName">The name of the table to insert into</param>
//        /// <param name="values">The values to be inserted into the table</param>
//        /// <param name="fields">The field names of the table columns</param>
//        /// <param name="parseNames">If set to true, the Create statement will NOT try to parse each field and table name through the parser, but run 'as is' 
//        /// to reduce processing time. NOTE: Does not effect values either way</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public virtual string CreateInsertSQL(string tableName, string[] values, string[] fields, bool parseNames) {

//            // If field names were not passed then return without them
//            String columns;

//            // Sort the fields (if any) second
//            if (fields == null) 
//                columns = "";
//            else if (!parseNames)
//                columns = String.Concat("(", String.Join(",", fields), ")");
//            else {

//                StringBuilder sb = new StringBuilder("(");

//                foreach (var field in fields) {
//                    sb.Append(this.SqlFieldName(field)).Append(", ");
//                }

//                columns = sb.Remove(sb.Length - 3, 2).Append(")").ToString();
//            }

//            return String.Format("insert into {0} {1} values ({2})", (parseNames? SqlTableName(tableName) : tableName), columns, string.Join(",", values));
//        }

//        /// <summary>
//        /// Returns as a list of strings for each record returned from the passed select qry as an insert statement for the table specified.
//        /// </summary>
//        /// <param name="connString"></param>
//        /// <param name="selectQry"></param>
//        /// <param name="tableName"></param>
//        /// <returns></returns>
//        /// <remarks>
//        /// Returns as a list of strings for each record returned from the passed select qry as an insert statement for the table specified.
//        /// 
//        /// Performs the connection to the db table direct using a DataReader rather than a DataTable for speed.</remarks>
//        public List<string> ExportDataAsInserts(string connString, string selectQry) {
//            return ExportDataAsInserts(CreateConnection(connString), selectQry);
//        }

//        /// <summary>
//        /// Returns as a list of strings for each record returned from the passed select qry as an insert statement for the table specified. If connections not open, its opens it
//        /// and closes it.
//        /// </summary>
//        /// <param name="connString"></param>
//        /// <param name="selectQry"></param>
//        /// <param name="tableName"></param>
//        /// <returns></returns>
//        /// <remarks>
//        /// Returns as a list of strings for each record returned from the passed select qry as an insert statement for the table specified.
//        /// 
//        /// Performs the connection to the db table direct using a DataReader rather than a DataTable for speed.</remarks>
//        public List<string> ExportDataAsInserts(DbConnection conn, string selectQry) {

//            Boolean implicityOpened = false;
//            string tmp = null;
//            List<String> result = new List<string>();
//            DbCommand comm = null;
			
//            List<String> fields = new List<String>();

//            try {

//                implicityOpened = Connections.OpenConnection(ref conn);

//                comm = CreateCommand(selectQry, conn);

//                using (DbDataReader reader = comm.ExecuteReader()) {

//                    // Create the field list for the insert statement first, to save doing it each time
//                    DataTable tab = reader.GetSchemaTable();

//                    String tableName = Convert.ToString(tab.Rows[0]["BaseTableName"]);

//                    foreach (DataRow row in tab.Rows) {
//                        fields.Add(Convert.ToString(row["ColumnName"]));
//                    }

//                    StringBuilder sb = new StringBuilder("insert into ").Append(tableName).Append(" (");

//                    foreach (var field in fields) {
//                        sb.Append(SqlFieldName(field)).Append(", ");
//                    }

//                    String insertStart = String.Concat(sb.ToString(0, sb.Length - 2), ") values (");

//                    while (reader.Read()) {

//                        string[] values = new String[fields.Count]; // Reinitialize the array

//                        // Loop through the fields in the reader object

//                        for (Int32 i = 0, n = values.Length; i < n; i++) {

//                            Int32 ordinal = reader.GetOrdinal(fields[i]);

//                            // Make only one call to get the field type
//                            Type fieldType = reader.GetFieldType(ordinal);

//                            // Check the type and return sensible string tmpue
//                            if (fieldType.Equals(typeof(string)))
//                                tmp = SqlString(StringExt.ConvertToString(reader[ordinal]));
//                            else if (fieldType.Equals(typeof(System.DateTime))) {
//                                // Store a reference to date to avoid recalling it
//                                DateTime dat = reader.GetDateTime(i);

//                                // If a null date then return the text for a null date for this database, else return this database version
//                                tmp = (dat.Equals(DBNull.Value) ? SqlNullDate() : SqlDate(dat));

//                            } else
//                                tmp = StringExt.ConvertToString(reader[ordinal]);

//                            values[i] = tmp;
//                        }

//                        result.Add(String.Concat(insertStart, String.Join(", ", values), ");"));
//                    }
//                }

//            } finally {
//                if (implicityOpened) Connections.CloseConnection(conn, true);
//            }

//            return result;

//        }

//        /// <summary>
//        /// Returns as a list of strings for each record returned from the selected table as an insert statement for the table specified. Excluding any autoincrement columns if stated
//        /// </summary>
//        /// <param name="connString">The connection string used to connect the the DB</param>
//        /// <param name="tableName">The name of the table to get values from</param>
//        /// <param name="excludeAutoIncrement">Whether to exlude and autoincrement columns it finds</param>
//        /// <returns></returns>
//        /// <remarks>Returns as a list of strings for each record returned from the selected table as an insert statement for the table specified.
//        /// Excluding the field names contained in the passed list that match the database table.
//        /// 
//        /// Performs the connection to the db table direct using a DataReader rather than a DataTable for speed.
//        /// </remarks>
//        public List<string> ExportDataAsInserts(String connString, string tableName, Boolean excludeAutoIncrement) {
//            return ExportDataAsInserts(CreateConnection(connString), tableName, excludeAutoIncrement);
//        }

//        /// <summary>
//        /// Returns as a list of strings for each record returned from the selected table as an insert statement for the table specified.
//        /// Excluding the field names contained in the passed list that match the database table.
//        /// </summary>
//        /// <param name="connString">The connection string used to connect the the DB</param>
//        /// <param name="tableName">The name of the table to get values from</param>
//        /// <param name="excludeAutoIncrement">Whether to exlude and autoincrement columns it finds</param>
//        /// <returns></returns>
//        /// <remarks>Returns as a list of strings for each record returned from the selected table as an insert statement for the table specified.
//        /// Excluding the field names contained in the passed list that match the database table.
//        /// 
//        /// Performs the connection to the db table direct using a DataReader rather than a DataTable for speed.
//        /// </remarks>
//        public List<string> ExportDataAsInserts(DbConnection conn, string tableName, Boolean excludeAutoIncrement) {

//            Boolean implicityOpened = false;
//            string tmp = null;
//            List<String> result = new List<string>();
//            DbCommand comm = null;

//            List<String> fields = new List<String>();

//            try {

//                implicityOpened = Connections.OpenConnection(ref conn);

//                comm = CreateCommand(String.Concat("select * from ", SqlTableName(tableName)), conn);

//                using (DbDataReader reader = comm.ExecuteReader()) {

//                    // Create the field list for the insert statement first, to save doing it each time
//                    DataTable tab = reader.GetSchemaTable();

//                    // Add each of the fields, but only if the 
//                    foreach (DataRow row in tab.Rows) {
//                        if(excludeAutoIncrement && !(row["isautoincrement"] as Boolean?).GetValueOrDefault(false)) fields.Add(Convert.ToString(row["ColumnName"]));
//                    }

//                    StringBuilder sb = new StringBuilder("insert into ").Append(tableName).Append(" (");

//                    foreach (var field in fields) {
//                        sb.Append(SqlFieldName(field)).Append(", ");
//                    }

//                    String insertStart = String.Concat(sb.ToString(0, sb.Length - 2), ") values (");

//                    while (reader.Read()) {

//                        string[] values = new String[fields.Count]; // Reinitialize the array

//                        // Loop through the fields in the reader object

//                        for (Int32 i = 0, n = values.Length; i < n; i++) {

//                            Int32 ordinal = reader.GetOrdinal(fields[i]);

//                            // Make only one call to get the field type
//                            Type fieldType = reader.GetFieldType(ordinal);

//                            // Check the type and return sensible string tmpue
//                            if (fieldType.Equals(typeof(string)))
//                                tmp = SqlString(StringExt.ConvertToString(reader[ordinal]));
//                            else if (fieldType.Equals(typeof(System.DateTime))) {
//                                // Store a reference to date to avoid recalling it
//                                DateTime dat = reader.GetDateTime(i);

//                                // If a null date then return the text for a null date for this database, else return this database version
//                                tmp = (dat.Equals(DBNull.Value) ? SqlNullDate() : SqlDate(dat));

//                            } else
//                                tmp = StringExt.ConvertToString(reader[ordinal]);

//                            values[i] = tmp;
//                        }

//                        result.Add(String.Concat(insertStart, String.Join(", ", values), ");"));
//                    }
//                }

//            } finally {
//                if (implicityOpened) Connections.CloseConnection(conn, true);
//            }

//            return result;

//        }
