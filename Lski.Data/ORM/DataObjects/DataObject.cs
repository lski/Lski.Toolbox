using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Text;
using System.Runtime.Serialization;

using Lski.Objects.Extensions;
using Lski.Data.ORM.Schema;
using Lski.Data.Common;
using Lski.Data.Connections;

namespace Lski.Data.ORM.DataObjects {

	/// <summary>
	/// Interface for holding together each of the business objects
	/// </summary>
	/// <remarks></remarks>
	[DataContract()]
	public abstract class DataObject : DataObjectBasic {

		public DataObject() { }

		#region "Properties"

		private static Dictionary<Type, TableSchema> _CachedSchemas = new Dictionary<Type, TableSchema>(0);

		/// <summary>
		/// Gets the table schema for this class, for effeciency uses a static cache.
		/// </summary>
		/// <returns></returns>
		public TableSchema GetSchema() {

			var type = this.GetType();

			if (!_CachedSchemas.ContainsKey(type)) {

				var ts = CreateSchema();
				_CachedSchemas.Add(type, ts);
				return ts;
			}
			
			return _CachedSchemas[type];
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Must override to be able to create the most applicable schema for this class. If using the DataObjects.TT files then simply point
		/// this to the appropriate Schema default and return that.
		/// </summary>
		/// <returns></returns>
		protected abstract TableSchema CreateSchema();

		/// <summary>
		/// Depending on the state of this record, it attempts to update the object in the underlying data source. If the 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Depending on the state of this record, it attempts to update the object in the underlying data source. 
		/// 
		/// If the state of the object is 'Unchanged' then the record will not update the underlying source.
		/// </remarks>
		public virtual Int32 Save() {

			// This is a new object to be inserted
			if (this.RecordState == DataObjectState.Added)
				return this.Insert();

				// This record already exists and needs to be updated in the database
			else if (this.RecordState == DataObjectState.Modified)
				return this.Update();

			// Else do nothing as the record is unchanged... (Or already deleted)
			return 0;
		}

		/// <summary>
		/// Creates and returns a valid insert command for this object, can not be cached as it can be different for each insert (because of default values)
		/// </summary>
		/// <returns></returns>
		public virtual Int32 Insert() {

			Int32 result = 0;
			Boolean explicitlyOpened = false;
			DbConnection conn = null;

			try {

				var schema = this.GetSchema();

				var comm = schema.CreateInsertQry(this);

				conn = schema.Provider.CreateConnection();
				comm.Connection = conn;

				explicitlyOpened = Connection.OpenConnection(ref conn);

				// As the insert id command will have been added already check to see if we can return 
				if (schema.PrimaryKey.IsAutoIncrement && schema.Provider.SupportsMars) {

					var o = comm.ExecuteScalar();

					if (o != null) {


						var prop = this.GetType().GetProperty(schema.PrimaryKey.PropertyName);

						if (prop != null)
							prop.SetValue(this, o, null);
					}

					result = 1;

				} else {

					result = comm.ExecuteNonQuery();
				}

			} finally {

				if (explicitlyOpened)
					Connection.CloseConnection(conn, true);
			}

			return result;
		}



		public virtual Int32 Update() {

			Int32 result = 0;
			Boolean explicitlyOpened = false;
			DbConnection conn = null;

			try {

				var schema = this.GetSchema();

				var comm = schema.CreateUpdateQry(this);

				conn = schema.Provider.CreateConnection();
				comm.Connection = conn;
				explicitlyOpened = Connection.OpenConnection(ref conn);

				result = comm.ExecuteNonQuery();

			}
			finally {

				if (explicitlyOpened)
					Connection.CloseConnection(conn, true);
			}

			return result;
		}

		public virtual Int32 Delete() {

			Int32 result = 0;
			Boolean explicitlyOpened = false;
			DbConnection conn = null;

			try {

				var schema = this.GetSchema();

				conn = schema.Provider.CreateConnection();
				DbCommand deleteCommand = schema.CreateDeleteQuery(this);

				explicitlyOpened = Connection.OpenConnection(ref conn);
				deleteCommand.Connection = conn;
				result = deleteCommand.ExecuteNonQuery();

			} finally {

				if (explicitlyOpened)
					Connection.CloseConnection(conn, true);
			}

			return result;
		}

		#endregion
	}

}

//using Microsoft.VisualBasic;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Data.Common;
//using System.Text;
//using System.Runtime.Serialization;

//using Toolbox;
//using Toolbox.Logging;
//using Toolbox.ADO.DbFactoryProvider;
//using Toolbox.DataHolders;

//namespace Toolbox.ADO.DataObjects {

//    /// <summary>
//    /// Interface for holding together each of the business objects
//    /// </summary>
//    /// <remarks></remarks>
//    [DataContract()]
//    public abstract class DataObjectFull : DataObjectBasic {

//        protected static bool _errorOnCast = false;
//        /// <summary>
//        /// States whether this partcular record should throw an error on cast or simply set it to null instead
//        /// </summary>
//        /// <value></value>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public static bool ErrorOnCast { 
//            get { return _errorOnCast; } 
//        }

//    }

//    /// <summary>
//    /// Works as a base class for all data access objects for particular business logic objects
//    /// </summary>
//    public abstract class BusinessLogicLayer {

//        #region "Constructors"

//        public BusinessLogicLayer() { this.Provider = DefaultDbProvider; }
//        public BusinessLogicLayer(DbProvider provider) { this.Provider = provider; }

//        #endregion

//        #region "Properties"

//        public static DbProvider DefaultDbProvider { get; set; }

//        private DbProvider _provider;
//        /// <summary>
//        /// Simply supplies the correct factory type for this object
//        /// </summary>
//        public DbProvider Provider {
//            get {
//                    _provider = _provider ?? DefaultDbProvider;

//                    // Now do a secondary test to ensure that the factory is set
//                    if (_provider == null) throw new NullReferenceException("The provider in the " + this.GetType().Name + " object can not be null");

//                    return _provider;
//            }

//            set {
//                if (value == null) throw new ArgumentNullException("The provider in the " + this.GetType().Name + " object can not be null");
//                _provider = value;
//            }
//        }



//        #endregion

//        #region "Methods"

//        /// <summary>
//        /// Depending on the state of this record, it attempts to update the object in the underlying data source. If the 
//        /// </summary>
//        /// <returns></returns>
//        /// <remarks>
//        /// Depending on the state of this record, it attempts to update the object in the underlying data source. 
//        /// 
//        /// If the state of the object is 'Unchanged' then the record will not update the underlying source.
//        /// </remarks>
//        public virtual Int32 UpdateDataSource(DataObjectFull obj) {

//            // This is a new object to be inserted
//            if (obj.RecordState == DataObjectState.Added) 
//                return this.Insert(obj);

//                // This record already exists and needs to be updated in the database
//            else if (obj.RecordState == DataObjectState.Modified)
//                return this.Update(obj);

//            // Else do nothing as the record is unchanged... (Or already deleted)
//            return 0;
//        }

//        public Int64? SelectCount() {
//            return RunSelectCount(CreateSelectCount());
//        }

//        protected abstract String CreateSelectCount();

//        /// <summary>
//        /// Actually performs the getting of the max number of records, using the Select methods to create the sql.
//        /// </summary>
//        /// <remarks>
//        /// Actually performs the getting of the max number of records, using the Select methods to create the sql.
//        /// 
//        /// Protected so that it can only be run within this object, to prevent stupid sql being planted
//        /// </remarks>
//        protected virtual Int64 RunSelectCount(String selectStatement) {

//            Boolean implicitlyOpended = false;
//            DbConnection conn = this.Provider.Connection;

//            try {

//                // Create the command object to use it
//                DbCommand comm = conn.CreateCommand();
//                comm.CommandText = selectStatement;
//                implicitlyOpended = Connections.OpenConnection(ref conn);
//                Object o = comm.ExecuteScalar();

//                return ((Int64)o);

//            } catch (Exception ex) {

//                Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//                throw ex;

//            } finally {
//                if (implicitlyOpended) Connections.CloseConnection(conn);
//            }
//        }

//        public abstract Int32 Insert(DataObjectFull obj);
//        public abstract Int32 Update(DataObjectFull obj);
//        public abstract Int32 Delete(DataObjectFull obj);

//        protected virtual Int32 Insert(DbCommand insertCommand) {
//            Object o = null; // Dummy object as nothing will be filled
//            return this.Insert(insertCommand, false, ref o);
//        }

//        protected virtual Int32 Insert(DbCommand insertCommand, Boolean autoIncrementValue, ref Object autoIncrememtValue) {

//            Int32 result = 0;
//            Boolean implicitlyOpened = false;
//            DbConnection conn = this.Provider.Connection;

//            try {

//                implicitlyOpened = Connections.OpenConnection(ref conn);
//                insertCommand.Connection = conn;

//                if (!autoIncrementValue) {
//                    result = insertCommand.ExecuteNonQuery();
//                    autoIncrememtValue = null;
//                } else {
//                    // Now attempt to insert the new record, but also to get the autoincrement value
//                    autoIncrememtValue = (insertCommand.ExecuteScalar());
//                    result = 1;
//                }

//            } catch(Exception ex) {

//                Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//                throw ex;

//            } finally {

//                if(implicitlyOpened) 
//                    Connections.CloseConnection(conn, true);
//            }

//            return result;
//        }

//        protected virtual Int32 Update(DbCommand updatedCommand) {

//            Int32 result = 0;
//            Boolean implicitlyOpened = false;
//            DbConnection conn = this.Provider.Connection;

//            try {

//                implicitlyOpened = Connections.OpenConnection(ref conn);
//                updatedCommand.Connection = conn;
//                result = updatedCommand.ExecuteNonQuery();

//            } catch(Exception ex) {

//                Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//                throw ex;

//            } finally {
//                if(implicitlyOpened) 
//                    Connections.CloseConnection(conn, true);
//            }

//            return result;
//        }

//        protected virtual Int32 Delete(DbCommand deleteCommmand) {

//            Int32 result = 0;
//            Boolean implicitlyOpened = false;
//            DbConnection conn = this.Provider.Connection;

//            try {

//                implicitlyOpened = Connections.OpenConnection(ref conn);
//                deleteCommmand.Connection = conn;
//                result = deleteCommmand.ExecuteNonQuery();

//            } catch(Exception ex) {

//                Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//                throw ex;

//            } finally {

//                if (implicitlyOpened && conn != null) 
//                    Connections.CloseConnection(conn, true); 
//            }

//            return result;
//        }

//        protected virtual Int32 EmptyTable(String tableName) {

//            Int32 result = 0;
//            Boolean implicitlyOpened = false;
//            DbConnection conn = this.Provider.Connection;

//            try {

//                DbCommand comm = conn.CreateCommand();
//                comm.CommandText = String.Format("delete from {0};", tableName);

//                implicitlyOpened = Connections.OpenConnection(ref conn);
//                result = comm.ExecuteNonQuery();

//            } catch(Exception ex) {

//                Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//                throw ex;

//            } finally {

//                if (implicitlyOpened) 
//                    Connections.CloseConnection(conn, true); 
//            }

//            return result;
//        }

//        #endregion
//    }

//}

//public DbCommand GetUpdateCommand() {

//    var type = this.GetType();

//    if (!_CachedUpdatedCommands.ContainsKey(type)) {

//        if (this.Schema.PrimaryKey == null)
//            throw new Exception(String.Format("The update command can not be created for the {0} because the schema does not have a primary key set", type));

//        var comm = this.Schema.Provider.CreateCommand();

//        // Create the object as a dictionary to get the propertynames (after any that need to be excluded are excluded)
//        var objAsDictionary = this.ToDictionary();

//        // Create the field name and parameter pair to update to place in the command text
//        var fieldParameterString = new StringBuilder();

//        foreach (var field in this.Schema.Fields.Values) {

//            // If the schema field doesnt match one in the object, then dont add it, or its the primary key field (as thats used to update this object
//            if (field.Equals(Schema.PrimaryKey) || !objAsDictionary.Any(x => x.Key.Equals(field.PropertyName, StringComparison.OrdinalIgnoreCase)))
//                continue;

//            comm.Parameters.Add(this.Schema.Provider.CreateParameter(field.ParameterName, field.FieldType, field.FieldName));

//            fieldParameterString.Append(field.SqlFieldName).Append(" = ").Append(field.ParameterName).Append(", ");
//        }

//        var pkParameterString = String.Concat(this.Schema.PrimaryKey.FieldName, " = ", this.Schema.PrimaryKey.ParameterName);

//        comm.Parameters.Add(this.Schema.Provider.CreateParameter(this.Schema.PrimaryKey.ParameterName, this.Schema.PrimaryKey.FieldType, this.Schema.PrimaryKey.FieldName));

//        comm.CommandText = String.Format("update {0} set {1} where {2};", Schema.SqlTableName, fieldParameterString.ToString(0, fieldParameterString.Length - 2), pkParameterString);

//        _CachedUpdatedCommands.Add(type, comm);

//        return comm;
//    }

//    return _CachedUpdatedCommands[type];
//}

//// Create the fieldlist
//var fields = this.ToDictionary();
//var mappedDic = new Dictionary<String, Object>(fields.Count);

//// Mapping the property names to FieldNames
//foreach (var item in fields) {

//    var o = this.Schema.Fields.SingleOrDefault(x => x.Key == item.Key);

//    if (o.Equals(default(KeyValuePair<String, IFieldSchema>)))
//        continue;

//    mappedDic.Add(o.Value.FieldName, item.Value);
//}

//// Now the keya are the field names they can be filled using the mapped Dictionary
//DBMethods.FillParameterValues(comm, mappedDic);

//comm = this.Schema.Provider.CleanCommand(comm);

//// Get the primary key
//var primaryKey = this.Schema.PrimaryKey;
//var IsAutoIncrement = (primaryKey != null ? primaryKey.IsAutoIncrement : false);

//var comm = this.Schema.Provider.CreateCommand();

//// Create the object as a dictionary to get the propertynames (after any that need to be excluded are excluded)
//var objAsDictionary = this.ToDictionary();

//// Create the field name and parameter pair to update to place in the command text
//var fieldnameString = new StringBuilder();
//var parametersString = new StringBuilder();

//foreach (var f in this.Schema.Fields.Values) {

//    // If a primary key and autoincrement, then dont try adding it
//    if (f.IsAutoIncrement)
//        continue;

//    var o = objAsDictionary.SingleOrDefault(x => x.Key.Equals(f.PropertyName, StringComparison.OrdinalIgnoreCase));

//    // If an empty object is returned OR the object value is null, but the field is not nullable then dont add the parameter (so db uses default)
//    if (o.Equals(default(KeyValuePair<String, IFieldSchema>)) || (o.Value == null && !f.IsNullable))
//        continue;

//    fieldnameString.Append(f.SqlFieldName).Append(", ");
//    parametersString.Append(f.ParameterName).Append(", ");

//    comm.Parameters.Add(this.Schema.Provider.CreateParameter(f.ParameterName, f.FieldType, o.Value, f.FieldName));
//}

//var commandText = String.Format("insert into {0} ({1}) values ({2});", Schema.SqlTableName, fieldnameString.ToString(0, fieldnameString.Length - 2), parametersString.ToString(0, parametersString.Length - 2));

//if (this.Schema.Provider.SupportsGetIdentity && this.Schema.PrimaryKey != null && this.Schema.PrimaryKey.IsAutoIncrement)
//    commandText = this.Schema.Provider.SqlGetIdentity(commandText, this.Schema.PrimaryKey.FieldType);

//comm.CommandText = commandText;

//// Ensure the values are vlid for this provider
//comm = this.Schema.Provider.CleanCommand(comm);

//protected virtual String CreateSelectCount() {
//    return "select count(*) from " + Schema.SqlTableName;
//}

///// <summary>
///// Actually performs the getting of the max number of records, using the Select methods to create the sql.
///// </summary>
///// <remarks>
///// Actually performs the getting of the max number of records, using the Select methods to create the sql.
///// 
///// Protected so that it can only be run within this object, to prevent stupid sql being planted
///// </remarks>
//protected virtual Int32 RunSelectCount(String whereStatement) {

//    Boolean implicitlyOpended = false;
//    DbConnection conn = this.Schema.Provider.Connection;

//    try {

//        // Create the command object to use it
//        DbCommand comm = conn.CreateCommand();
//        comm.CommandText = selectStatement;
//        implicitlyOpended = Connections.OpenConnection(ref conn);
//        Object o = comm.ExecuteScalar();

//        return ((Int32)o);

//    } catch (Exception ex) {

//        Logger.Default.WriteEntry(ex, Logger.EventType.Error);
//        throw ex;

//    } finally {
//        if (implicitlyOpended) Connections.CloseConnection(conn);
//    }
//}