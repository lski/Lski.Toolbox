using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Reflection;

using Lski.Data.Common;

namespace Lski.Data.ORM.Schema {


	/// <summary>
	/// Represents a database
	/// </summary>
	public class FieldSchema : IFieldSchema {

		#region "Members"

		public ITableSchema ParentTable { get; set; }

		/// <summary>
		/// The property name that this field links to in the business record class
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public String PropertyName { 
			get; set; 
		}

		private String _fieldName;
		/// <summary>
		/// The original field name, before its parsed to go into an SQL Statement, removes the cached versions, as they ar3e based on the fieldName property
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public String FieldName {
			get { return _fieldName; }
			set {
				_fieldName = value;
				_CachedSqlFieldName = null;
				_CachedParameterName = null;
				_CachedFullQualifiedName = null;
			}
		}

		/// <summary>
		/// States whether this field can hold nulls or not
		/// </summary>
		public Boolean IsNullable { get; set; }

		public DbType FieldType { get; set; }
		
		public Boolean IsReadOnly { get; set; }
		
		public Boolean IsPrimary { get; set; }
		
		public Boolean IsAutoIncrement { get; set; }

		protected String _CachedParameterName;
		/// <summary>
		/// The field name converted into a format useable for the parameters (NB Caches result)
		/// </summary>
		public String ParameterName {
			get {
				// If not already set then use the parent table to create one
				if (_CachedParameterName == null) _CachedParameterName = this.ParentTable.Provider.ToParameterName(this.FieldName);
				return _CachedParameterName;
			}
		}

		private String _CachedSqlFieldName;
		/// <summary>
		/// The field name converted into a format useable for sql statements (NB Caches result)
		/// </summary>
		public String SqlFieldName {
			get {
				return _CachedSqlFieldName ?? (_CachedSqlFieldName = this.ParentTable.Provider.ToSqlFieldName(this.FieldName));
			}
		}

		private String _CachedFullQualifiedName;
		/// <summary>
		/// States the full field name, including the table name, followed by the field name, all printed specificall for this factory type (NB Caches result)
		/// </summary>
		public String FullQualifiedName {
			get {
				return _CachedFullQualifiedName = (_CachedFullQualifiedName = this.ParentTable.SqlTableName + "." + this.SqlFieldName);
			}
		}


		#endregion

		#region "Constructors"

		public FieldSchema(ITableSchema table, String propertyName, String fieldName, DbType type, Boolean isReadOnly = false, Boolean isNullable = false, Boolean isPrimary = false, Boolean isAutoIncrement = false) {
			this.Init(table, propertyName, fieldName, type, isReadOnly, isNullable, isPrimary, isAutoIncrement);
		}

		protected void Init(ITableSchema table, String propertyName, String fieldName, DbType type, Boolean isReadOnly = false, Boolean isNullable = false, Boolean isPrimary = false, Boolean isAutoIncrement = false) {
			this.ParentTable = table;
			this.PropertyName = propertyName;
			this.FieldName = fieldName;
			this.FieldType = type;
			this.IsReadOnly = isReadOnly;
			this.IsNullable = isNullable;
			this.IsPrimary = isPrimary;
			this.IsAutoIncrement = isAutoIncrement;
		}

		#endregion

	}

}