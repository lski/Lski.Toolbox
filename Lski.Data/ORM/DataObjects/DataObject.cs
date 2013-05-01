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