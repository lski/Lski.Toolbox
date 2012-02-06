using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Data.OleDb;

namespace Lski.Data.Common {

	public class OleFactory : DbProvider {

		#region "Constructor"

		public OleFactory() 
			: base() { }
		
		#endregion

		#region "Properties"

		public const string DatabaseName = "Ole";

		public override string DatabaseType {
			get { return DatabaseName; }
		}

		protected override String ParameterChar {
			get { return "?"; }
		}

		#endregion

		#region "Methods"

		public override string ToGetIdentitySql(string insertQry, DbType type) {
			return "SELECT @@IDENTITY";
		}

		/// <summary>
		/// Returns a simple '?' to represent the parameter, as parameters in the Ole are not named.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string ToParameterPlaceholder(string name) {
			return this.ParameterChar;
		}

		public override object Clone() { 
			return new OleFactory(); 
		}

		public override DbConnection CreateConnection() { 
			return new OleDbConnection();
		}
		public override DbDataAdapter CreateDataAdapter() { 
			return new OleDbDataAdapter(); 
		}
		public override DbParameter CreateParameter() { 
			return new OleDbParameter(); 
		}
		public override DbCommandBuilder CreateCommandBuilder() { 
			return new OleDbCommandBuilder();
		}
		public override DbCommand CreateCommand(DbConnection conn) { 
			DbCommand comm = new OleDbCommand();
			comm.Connection = conn;
			return comm;
		}

		public override DbCommand CleanCommand(DbCommand comm) {

			foreach (DbParameter p in comm.Parameters) {
				if (p.DbType == DbType.DateTime)
					((OleDbParameter)p).OleDbType = OleDbType.Date;
			}

			return comm;
		}

		public override string GetDbSpecificType(DbType t) {

			switch (t) {
				case DbType.Object:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					return "varchar";
				case DbType.Boolean:
					return "tinyint";
				case DbType.SByte:
				case DbType.Binary:
				case DbType.Byte:
					return "longblob";
				case DbType.Time:
				case DbType.Date:
				case DbType.DateTime:
					return "datetime";
				case DbType.Currency:
				case DbType.Decimal:
					return "decimal";
				case DbType.Double:
					return "float";
				case DbType.Guid:
					return "binary";
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
					return "varchar";
			}
		}

		#endregion

	}

}