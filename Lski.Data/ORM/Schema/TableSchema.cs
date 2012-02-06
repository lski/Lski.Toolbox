using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Diagnostics;

using Lski.Data;
using Lski.Data.Extensions;
using Lski.Objects;
using Lski.Data.Common;
using Lski.Data.Connections;

namespace Lski.Data.ORM.Schema {

	public class TableSchema : ITableSchema {

		#region Props

		public DbProvider Provider { get; set; }

		public String TableName { get; set; }

		private String _CachedSqlTableName;
		public String SqlTableName {
			get {
				return _CachedSqlTableName ?? (_CachedSqlTableName = Provider.ToSqlTableName(this.TableName));
			}
		}

		private IFieldSchema _CachedPrimaryKey;
		public IFieldSchema PrimaryKey {
			get {
				if (_CachedPrimaryKey == null)
					_CachedPrimaryKey = (from x in this.Fields where x.Value.IsPrimary select x.Value).FirstOrDefault();

				return _CachedPrimaryKey;
			}
		}

		public Dictionary<String, IFieldSchema> Fields { get; private set; }

		private Dictionary<String, String> _CachedPropertyFieldMappings;
		public Dictionary<String, String> PropertyFieldMappings {
			get {

				if (_CachedPropertyFieldMappings == null) {

					_CachedPropertyFieldMappings = new Dictionary<string, string>();

					foreach (var f in Fields) {
						_CachedPropertyFieldMappings.Add(f.Value.PropertyName, f.Value.FieldName);
					}
				}

				return _CachedPropertyFieldMappings;
			}
		}

		#endregion

		/// <summary>
		/// Used for serialization ONLY
		/// </summary>
		protected TableSchema() { }

		public TableSchema(DbProvider provider, string tableName) {
			this.Provider = provider;
			this.TableName = tableName;
			this.Fields = new Dictionary<String, IFieldSchema>();
		}

		#region Select

		public List<T> Select<T>() where T : new() {
			return Select<T>(null);
		}

		public List<T> Select<T>(String where) where T : new() {
			return RunSelect<T>(CreateSelectQuery(where));
		}

		public List<T> Select<T>(Int32 StartRow) where T : new() {
			return Select<T>(StartRow, -1, null);
		}

		public List<T> Select<T>(Int32 StartRow, Int32 MaxRows) where T : new() {
			return Select<T>(StartRow, MaxRows, null);
		}

		public List<T> Select<T>(String where, Int32 StartRow, Int32 MaxRows) where T : new() {
			return Select<T>(where, StartRow, MaxRows, null);
		}

		public List<T> Select<T>(Int32 StartRow, Int32 MaxRows, String sortExpression) where T : new() {
			return Select<T>(null, StartRow, MaxRows, sortExpression);
		}

		public List<T> Select<T>(String where, Int32 StartRow, Int32 MaxRows, String sortExpression) where T : new() {
			return RunSelect<T>(CreateSelectQuery(where, StartRow, MaxRows, sortExpression));
		}


		/// <summary> 
		/// Actually performs the getting of this records, using the Select methods to create the sql.
		/// </summary>
		/// <remarks>
		/// Actually performs the getting of this records, using the Select methods to create the sql.
		/// 
		/// Protected so that it can only be run within this object, to prevent stupid sql being planted. The only real criteria is that the resulting 
		/// result set contains at least the same footprint as this object (can contain more but wont be captured by this method)
		/// </remarks>
		protected List<T> RunSelect<T>(DbCommand selectCommand) where T : new() {

			var l = new List<T>();

			Boolean explicitlyOpened = false;
			DbConnection conn = this.Provider.CreateConnection();

			try {

				// Create the command object to use it
				explicitlyOpened = Connection.OpenConnection(ref conn);

#if DEBUG
				Debug.WriteLine(selectCommand.CommandText);
#endif

				using (DbDataReader r = selectCommand.ExecuteReader()) {

					while (r.Read()) {
						l.Add(r.CreateObject<T>(PropertyFieldMappings));
					}

				}

			}
			finally {

				if (explicitlyOpened)
					Connection.CloseConnection(conn);
			}

			return l;
		}

		[Lski.IssueTracking.ToFix("Really need to sort out the issue with the where clause, to include params, to avoid being so insecure", "2011/04/01")]
		public DbCommand CreateSelectQuery(String where, Int32 StartRow, Int32 MaxRows, String sortExpression) {

			String parsedSort = null;

			if (!String.IsNullOrEmpty(sortExpression))
				parsedSort = new SortExpression(sortExpression, this.PropertyFieldMappings).MappedExpression;
			else {

				if (this.PrimaryKey == null)
					throw new Exception("A primary key column needs to be set to enable sorting without a defined sort");

				parsedSort = this.PrimaryKey.SqlFieldName;
			}

			var comm = this.CreateSelectQuery(where);

			comm.CommandText = this.Provider.LimitSelect(comm.CommandText, StartRow, MaxRows, parsedSort);

			return comm;
		}

		/// <summary>
		/// Creates a DbCommand object filled with the correct commandText adding the where and order bys needed.
		/// </summary>
		/// <param name="where">Optional sort expression eg. 'ColumnA ASC, ColumnB Desc'</param>
		/// <returns></returns>
		[Lski.IssueTracking.ToFix("Really need to sort out the issue with the where clause, to include params, to avoid being so insecure", "2011/04/01")]
		public DbCommand CreateSelectQuery(String where) {

			var comm = this.Provider.CreateCommand();

			StringBuilder fieldList = new StringBuilder();

			foreach (var f in this.Fields.Values) {
				fieldList.Append(f.SqlFieldName).Append(", ");
			}

			String parsedWhere;

			if (String.IsNullOrEmpty(where))
				parsedWhere = String.Empty;
			else {

				parsedWhere = where.Trim();

				if (!parsedWhere.StartsWith("where", StringComparison.OrdinalIgnoreCase))
					parsedWhere = "where " + parsedWhere;
			}

			var qry = String.Format("select {0} from {1} ", fieldList.ToString(0, fieldList.Length - 2), this.SqlTableName);

			comm.CommandText = String.Concat(qry, parsedWhere);

			return comm;
		}

		#endregion

		#region Insert

		public DbCommand CreateInsertQry(Object obj) {

			DbCommand comm = null;

			comm = this.Provider.CreateCommand();

			// Get the primary key
			var primaryKey = this.PrimaryKey;
			var IsAutoIncrement = (primaryKey != null ? primaryKey.IsAutoIncrement : false);

			// Create the object as a dictionary to get the propertynames (after any that need to be excluded are excluded)
			var objAsDictionary = obj.ToDictionary();

			// Create the field name and parameter pair to update to place in the command text
			var fieldnameString = new StringBuilder();
			var parametersString = new StringBuilder();

			foreach (var f in this.Fields.Values) {

				// If a primary key and autoincrement, then dont try adding it
				if (f.IsAutoIncrement)
					continue;

				var o = objAsDictionary.SingleOrDefault(x => x.Key.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase));

				// If an empty object is returned OR the object value is null, but the field is not nullable then dont add the parameter (so db uses default)
				if (o.Equals(default(KeyValuePair<String, IFieldSchema>)) || (o.Value == null && !f.IsNullable))
					continue;

				fieldnameString.Append(f.SqlFieldName).Append(", ");
				parametersString.Append(f.ParameterName).Append(", ");

				comm.Parameters.Add(this.Provider.CreateParameter(f.ParameterName, f.FieldType, o.Value, f.FieldName));
			}

			var commandText = String.Format("insert into {0} ({1}) values ({2});", this.SqlTableName, fieldnameString.ToString(0, fieldnameString.Length - 2), parametersString.ToString(0, parametersString.Length - 2));

			if (this.Provider.SupportsMars && this.PrimaryKey != null && this.PrimaryKey.IsAutoIncrement)
				commandText = this.Provider.ToGetIdentitySql(commandText, this.PrimaryKey.FieldType);

			comm.CommandText = commandText;

			// Ensure the values are vlid for this provider
			comm = this.Provider.CleanCommand(comm);

			return comm;
		}

		#endregion

		#region Update

		public DbCommand CreateUpdateQry(Object obj) {

			DbCommand comm = null;

			comm = this.Provider.CreateCommand();

			if (this.PrimaryKey == null)
				throw new Exception(String.Format("The update command can not be created for the {0} because the schema does not have a primary key set", obj.GetType().Name));

			// Create the object as a dictionary to get the propertynames (after any that need to be excluded are excluded)
			var objAsDictionary = obj.ToDictionary();

			// Create the field name and parameter pair to update to place in the command text
			var fieldParameterString = new StringBuilder();

			foreach (var f in this.Fields.Values) {

				// If its the primary key field (as thats used to update this object) remove it
				if (f.Equals(this.PrimaryKey))
					continue;

				var o = objAsDictionary.SingleOrDefault(x => x.Key.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase));

				// If the schema field doesnt match one in the object, then dont add it, or 
				if (!objAsDictionary.Any(x => x.Key.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase)))
					continue;

				comm.Parameters.Add(this.Provider.CreateParameter(f.ParameterName, f.FieldType, o.Value, f.FieldName));

				fieldParameterString.Append(f.SqlFieldName).Append(" = ").Append(f.ParameterName).Append(", ");
			}

			var pkParameterString = String.Concat(this.PrimaryKey.FieldName, " = ", this.PrimaryKey.ParameterName);

			comm.Parameters.Add(this.Provider.CreateParameter(this.PrimaryKey.ParameterName, this.PrimaryKey.FieldType, objAsDictionary[this.PrimaryKey.PropertyName], this.PrimaryKey.FieldName));

			comm.CommandText = String.Format("update {0} set {1} where {2};", this.SqlTableName, fieldParameterString.ToString(0, fieldParameterString.Length - 2), pkParameterString);

			// Ensure the values are vlid for this provider
			comm = this.Provider.CleanCommand(comm);

			return comm;

		}

		#endregion

		#region Delete

		public DbCommand CreateDeleteQuery(Object obj) {

			try {

				if (this.PrimaryKey == null)
					throw new Exception("The delete command can not be created for the " + obj.GetType().Name + " because the schema does not have a primary key set");

				var pkProperty = obj.GetType().GetProperty(this.PrimaryKey.PropertyName);

				if (pkProperty == null)
					throw new Exception("The object used to create the delete query does not have a property matching the property name '{0}' of that schema");

				var comm = this.Provider.CreateCommand();

				var pkParameterString = String.Concat(this.PrimaryKey.FieldName, " = ", this.PrimaryKey.ParameterName);

				var p = this.Provider.CreateParameter(this.PrimaryKey.ParameterName, this.PrimaryKey.FieldType, pkProperty.GetValue(obj, null), String.Empty);
				p.SourceColumn = this.PrimaryKey.FieldName;
				comm.Parameters.Add(p);

				comm.CommandText = String.Format("delete from {0} where {1};", SqlTableName, pkParameterString);

				return comm;

			}
			catch (Exception) {

				throw;
			}

		}

		public virtual Int32 EmptyTable() {

			Int32 result = 0;
			Boolean explicitlyOpened = false;
			DbConnection conn = this.Provider.CreateConnection();

			try {

				DbCommand comm = conn.CreateCommand();
				comm.CommandText = String.Format("delete from {0};", this.SqlTableName);

				explicitlyOpened = Connection.OpenConnection(ref conn);
				result = comm.ExecuteNonQuery();

			}
			finally {

				if (explicitlyOpened)
					Connection.CloseConnection(conn, true);
			}

			return result;
		}

		#endregion
	}
}
