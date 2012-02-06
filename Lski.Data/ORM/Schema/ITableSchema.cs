using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Lski.Data.Common;

namespace Lski.Data.ORM.Schema {

	public interface ITableSchema {

		/// <summary>
		/// The provider for handling connection to the database
		/// </summary>
		DbProvider Provider { get; set; }
		/// <summary>
		/// The name of the table
		/// </summary>
		String TableName { get; set; }
		/// <summary>
		/// The name of the table, when written in Sql for this provider type
		/// </summary>
		String SqlTableName { get; }
		/// <summary>
		/// The field marked as Primary Key
		/// </summary>
		IFieldSchema PrimaryKey { get; }
		/// <summary>
		/// The list of all fields asscoiated with this table
		/// </summary>
		Dictionary<String, IFieldSchema> Fields { get; }
		/// <summary>
		/// A mapping list where the key (Property Names) and values (Field Names)
		/// </summary>
		Dictionary<String, String> PropertyFieldMappings { get; }
		/// <summary>
		/// Creates a DbCommand object filled with all the necessary information for passed object based on this schema for inserting it into the database
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		DbCommand CreateInsertQry(Object obj);
		/// <summary>
		/// Creates a DbCommand object filled with all the necessary information for passed object based on this schema for updating it in the database
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		DbCommand CreateUpdateQry(Object obj);
	}
}
