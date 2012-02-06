using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.Common;
using System.ComponentModel;

namespace Lski.Data.Common {

	public class SqlServerFactory : DbProvider {

		#region "Constructor"

		public SqlServerFactory() : base() { }
		
		#endregion

		#region "Properties"

		public const String DatabaseName = "SqlServer";

		public override string DatabaseType {
			get { return DatabaseName; }
		}

		#endregion

		#region "Methods"

		public override object Clone() { 
			return new SqlServerFactory(); 
		}
		public override DbCommandBuilder CreateCommandBuilder() { 
			return new SqlCommandBuilder(); 
		}
		public override DbConnection CreateConnection() { 
			return new SqlConnection(); 
		}
		public override DbDataAdapter CreateDataAdapter() { 
			return new SqlDataAdapter(); 
		}
		public override DbParameter CreateParameter() { 
			return new SqlParameter();
		}
		public override DbCommand CreateCommand(DbConnection conn) {
			DbCommand comm = new SqlCommand();
			comm.Connection = conn;
			return comm;
		}

		public override string GetDbSpecificType(System.Data.DbType t) {

			switch (t) {
				case DbType.Object:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return "nvarchar";
				case DbType.Boolean:
					return "bit";
				case DbType.SByte:
				case DbType.Binary:
				case DbType.Byte:
					return "image";
				case DbType.Currency:
					return "money";
				case DbType.Time:
				case DbType.Date:
				case DbType.DateTime:
					return "datetime";
				case DbType.Decimal:
					return "decimal";
				case DbType.Double:
					return "float";
				case DbType.Guid:
					return "uniqueidentifier";
				case DbType.UInt32:
				case DbType.UInt16:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.UInt64:
				case DbType.Int64:
					return "int";
				case DbType.Single:
					return "real";
				case DbType.VarNumeric:
					return "numeric";
				case DbType.Xml:
					return "xml";
				default:
					return "nvarchar";
			}
		}

		public override string ToGetIdentitySql(string insertQry, DbType type) {
			return String.Concat((insertQry.TrimEnd().EndsWith(";") ? insertQry : String.Concat(insertQry, ";")), "SELECT CAST(SCOPE_IDENTITY() as " + GetDbSpecificType(type) + ");");
		}

		/// <summary>
		/// Converts the passed table name into a format applicable for the current DBMS e.g. dbo.[My Table] or just [My Table]
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string ToSqlTableName(string tableName) {

			Boolean hasStart = tableName.StartsWith("dbo." + this.QuoteIdentifierLeft);
			Boolean hasShortStart = tableName.StartsWith(this.QuoteIdentifierLeft);
			Boolean hasEnd = tableName.EndsWith(this.QuoteIdentifierRight);

			// If already has the idenifiers round then simply return the string
			// If only one side done add the other side
			if (hasStart && hasEnd) {
				return tableName;
			} else if (hasShortStart && hasEnd) {
				return "dbo." + tableName;
			} else if (hasStart) {
				return tableName + this.QuoteIdentifierRight;
			} else if (hasShortStart) {
				return "dbo." + tableName + this.QuoteIdentifierRight;
			} else if (hasEnd) {
				return "dbo." + this.QuoteIdentifierLeft + tableName;
			}

			// Else add both
			return "dbo." + this.QuoteIdentifierLeft + tableName + this.QuoteIdentifierRight;

		}

		#endregion
	}

}
