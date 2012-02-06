using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Lski.Data.Common {

	public class MySqlFactory : DbProvider {

		#region "Constructor"

		/// <summary>
		/// Static so called prior to the first instance is created automatically
		/// </summary>
		static MySqlFactory() {
			// To avoid shipping with MySQL.data.dll as this is now obsolete, store a DbProviderFactory ref and use that
			_internalFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
		}

		public MySqlFactory() 
			: base() { }
		
		#endregion

		#region "Properties"

		private static readonly DbProviderFactory _internalFactory;

		public const String DatabaseName = "MySQL";

		public override string DatabaseType {
			get { return DatabaseName; }
		}

		protected override string QuoteIdentifierLeft {
			get { return "`"; }
		}

		protected override string QuoteIdentifierRight {
			get { return "`"; }
		}

		protected override string ParameterChar {
			get { return "?"; }
		}

		#endregion

		public override bool CanCreateDataSourceEnumerator {
			get { return _internalFactory.CanCreateDataSourceEnumerator; }
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator() {
			return _internalFactory.CreateDataSourceEnumerator();
		}

		public override DbConnection CreateConnection() {
			return _internalFactory.CreateConnection();
		}

		public override DbDataAdapter CreateDataAdapter() {
			return _internalFactory.CreateDataAdapter();
		}

		public override object Clone() {
			return new MySqlFactory();
		}

		public override DbCommandBuilder CreateCommandBuilder() {
			return _internalFactory.CreateCommandBuilder();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder() {
			return _internalFactory.CreateConnectionStringBuilder();
		}

		/// <summary>
		/// States this provider does support automatic identity retrieval on insert
		/// </summary>
		public override bool SupportsMars {
			get { return true; }
		}

		public override string ToGetIdentitySql(string insertQry, System.Data.DbType type) {
			return String.Concat((insertQry.TrimEnd().EndsWith(";") ? insertQry : String.Concat(insertQry, ";")), "select cast(LAST_INSERT_ID() as " + GetDbSpecificType(type) + " ) as insert_id;");
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
					return "tinyint";
				case DbType.SByte:
				case DbType.Binary:
				case DbType.Byte:
					return "longblob";
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
					return "binary";
				case DbType.UInt32:
				case DbType.UInt16:
				case DbType.UInt64:
					return "unsigned int";
				case DbType.Int16:
				case DbType.Int32:
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

		public override string LimitSelect(string selectQry, int start, int size, string orderBy) {
			return selectQry + " ORDER BY " + orderBy + " LIMIT " + (start < 0 ? 0 : start) + ", " + (size < 0 ? Int32.MaxValue : size);
		}

		public override string TopSelect(string selectQry, int maxAmount, string orderBy) {
			return this.LimitSelect(selectQry, 0, maxAmount, orderBy);
		}

		public override DbCommand CreateCommand(DbConnection conn) {
			DbCommand comm = _internalFactory.CreateCommand();
			comm.Connection = conn;
			return comm;
		}

		public override System.Data.Common.DbParameter CreateParameter() {
			return _internalFactory.CreateParameter();
		}
	}
}
