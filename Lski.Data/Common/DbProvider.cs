using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Configuration;

using Lski.Data.Common.Exceptions;
using Lski.Txt;

namespace Lski.Data.Common {

	/// <summary>
	/// An absract superclass used as a factory for creating database specific factory objects.
	/// </summary>
	/// <remarks>An absract superclass used as a factory for creating database specific factory objects.
	/// </remarks>
	[DataContract()]
	public abstract class DbProvider : System.Data.Common.DbProviderFactory, ICloneable {

		/// <summary>
		/// Hidden constructor so that it cannot be created directly
		/// </summary>
		/// <param name="connectionString"></param>
		protected DbProvider() {}

		#region "Properties"

		/// <summary>
		/// Provides a string representation of the value for null for this type of database
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string SqlNull {
			get { return "NULL"; }
		}

		/// <summary>
		/// Provides a string representation of the value for null for this type of database
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		[Obsolete("This method should no longer be used as most modern DB systems use a standard NULL as its null date")]
		public virtual string SqlNullDate {
			get { return "NULL"; }
		}

		/// <summary>
		/// Returns the string that represents a single quote in an SQL statement (As a single quote is used to represent
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string SqlSingleQuote {
			get { return @"\'"; }
		}

		protected virtual string QuoteIdentifierLeft {
			get { return "["; }
		}

		protected virtual string QuoteIdentifierRight {
			get { return "]"; }
		}

		/// <summary>
		/// States whether or not this provider supports multiple result sets, meaning they can use SqlGetIdentity
		/// </summary>
		/// <returns></returns>
		public virtual Boolean SupportsMars {
			get { return false; }
		}

		protected virtual String ParameterChar {
			get { return "@"; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Returns the passed value with the correct formatting to inserting it into a database.
		/// Escapes any single quotes with the database specific escape char
		/// </summary>
		/// <param name="val">The value to convert</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlString(string val) { 
			return "'" + val.Replace("'", this.SqlSingleQuote) + "'"; 
		}

		/// <summary>
		/// Returns the passed date with the correct formatting for inserting it into a database
		/// </summary>
		/// <param name="dat">The date as a string that you want to insert into the database</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlDateTime(System.DateTime dat) { 
			return this.ToSqlDateTime(dat.ToString("yyyy/MM/dd HH:mm:ss")); 
		}

		/// <summary>
		/// Returns the passed date with the correct formatting for inserting it into a database
		/// </summary>
		/// <param name="dat">The date as a string that you want to insert into the database</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlDateTime(string dat) {	
			return "'" + dat + "'"; 
		}

		/// <summary>
		/// Returns the passed date with the correct formatting for inserting it into a database, by default uses format: yyyy/MM/dd
		/// </summary>
		/// <param name="dat">The date as a string that you want to insert into the database</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlDate(DateTime dat) { 
			return "'" + dat.ToString("yyyy/MM/dd") + "'";
		}

		/// <summary>
		/// Converts the value passed into a format for inserting into a database
		/// </summary>
		/// <param name="dat">The date passed in a string format</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlDate(string dat) {	
			return "'" + dat + "'"; 
		}

		/// <summary>
		/// Converts the field name and converts it into a format applicable for the current DBMS, e.g. [My Field]
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlFieldName(string fieldName) {

			bool hasStart = false;
			bool hasEnd = false;

			String[] parts = fieldName.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			// If no parts, throw exception
			if (parts.Length == 0) 
				throw new ArgumentException("The fieldname can not be blank");

			for (int i = 0; i <= parts.Length - 1; i++) {

				hasStart = parts[i].StartsWith(this.QuoteIdentifierLeft);
				hasEnd = parts[i].EndsWith(this.QuoteIdentifierRight);

				// If already has the idenifiers round then simply return the string
				// If only one side done add the other side
				// Else add both
				if (hasStart && hasEnd) 
					parts[i] = parts[i];
				else if (hasStart)
					parts[i] = parts[i] + this.QuoteIdentifierRight;
				else if (hasEnd)
					parts[i] = this.QuoteIdentifierLeft + parts[i];
				else
					parts[i] = this.QuoteIdentifierLeft + parts[i] + this.QuoteIdentifierRight;

			}

			return String.Join(".", parts);

		}

		/// <summary>
		/// Returns sql for getting the last insert id created
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToGetIdentitySql(String insertQry, DbType type) {
			return String.Concat((insertQry.TrimEnd().EndsWith(";") ? insertQry : String.Concat(insertQry, ";")), "SELECT CAST(@@IDENTITY as " + GetDbSpecificType(type) + ");");
		}

		/// <summary>
		/// Converts the passed table name into a format applicable for the current DBMS e.g. dbo.[My Table] or just [My Table]
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToSqlTableName(string tableName) {

			Boolean hasStart = tableName.StartsWith(this.QuoteIdentifierLeft);
			Boolean hasEnd = tableName.EndsWith(this.QuoteIdentifierRight);

			// If already has the idenifiers round then simply return the string
			// If only one side done add the other side
			if (hasStart && hasEnd)
				return tableName;
			else if (hasStart)
				return tableName + this.QuoteIdentifierRight;
			else if (hasEnd)
				return this.QuoteIdentifierLeft + tableName;

			// Else add both
			return this.QuoteIdentifierLeft + tableName + this.QuoteIdentifierRight;

		}

		/// <summary>
		/// Converts the passed string into a useable parameter name for this factory, that is actually used in the ParameterObject itself. 
		/// If there are any ' ' spaces they are converted to underscores E.g. SqlServer = @paramName, MySql = ?paramName, Oracle = :paramName
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToParameterName(string name) {

			if (string.IsNullOrEmpty(name)) 
				throw new ArgumentException("The parameter name can not be blank");

			String[] parts = name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			// If the fieldname only contained dots, throw exception
			if (parts.Length == 0) 
                throw new ArgumentException("The parameter name can not be blank");

			// If the first character is in fact the parameter char then simply return the parameter ensuring removal of spaces
			if (parts[parts.Length - 1].StartsWith(this.ParameterChar)) 
				return parts[parts.Length - 1].Replace(' ', '_');

			// When this is added to a particular factory class this bit needs to be overridden
			return this.ParameterChar + (parts[parts.Length - 1].Replace(' ', '_'));
		}

		/// <summary>
		/// Used to return a parameter placeholder for an sql query, normally the same as ToParameter
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string ToParameterPlaceholder(string name) {
			return ToParameterName(name);
		}

		/// <summary>
		/// Creates a parameter specific to this factory, and returns it with the name specified
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DbParameter CreateParameter(string parameterName) {

			DbParameter para = this.CreateParameter();
			para.ParameterName = parameterName;
			return para;

		}

		/// <summary>
		/// Creates a parameter specific to this factory, and returns it with the name specified
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DbParameter CreateParameter(string parameterName, DbType parameterType) {

			DbParameter para = this.CreateParameter();
			para.ParameterName = parameterName;
			para.DbType = parameterType;
			return para;

		}

		/// <summary>
		/// Creates a parameter specific to this factory, and returns it with the name specified
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DbParameter CreateParameter(string parameterName, DbType parameterType, String sourceColumn) {

			DbParameter para = this.CreateParameter();
			para.ParameterName = parameterName;
			para.DbType = parameterType;
			para.SourceColumn = sourceColumn;
			return para;
		}

		/// <summary>
		/// Creates a complete parameter including the value to add NOTE: If the value is null, its converted to DBNull.Value
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="parameterType"></param>
		/// <param name="value"></param>
		/// <param name="sourceColumn"></param>
		/// <returns></returns>
		public DbParameter CreateParameter(string parameterName, DbType parameterType, Object value, String sourceColumn) {

			DbParameter para = this.CreateParameter();
			para.ParameterName = parameterName;
			para.DbType = parameterType;
			para.SourceColumn = sourceColumn;
			para.Value = (value ?? DBNull.Value);
			return para;
		}

		public DbConnection CreateConnection(String connString) {
			var conn = this.CreateConnection();
			conn.ConnectionString = connString;
			return conn;
		}

		public DbCommand CreateCommand(string query) {

			DbCommand comm = this.CreateCommand();
			comm.CommandText = query;
			return comm;

		}

		public DbCommand CreateCommand(string query, DbConnection conn) {

			DbCommand comm = this.CreateCommand();
			comm.Connection = conn;
			comm.CommandText = query;
			return comm;

		}

		public override DbCommand CreateCommand() {
			return this.CreateCommand(this.CreateConnection()); 
		}

		/// <summary>
		/// Generally not needed but can clean up values placed into a command via parameters in sub classes if necessary
		/// </summary>
		/// <returns></returns>
		public virtual DbCommand CleanCommand(DbCommand comm) { 
			return comm; 
		}

		/// <summary>
		/// Returns the passed select query with a generic wrapper around that limits the amount of returned results
		/// Works like adding the limit clause in MySQL, by top and tailing the inner select
		/// </summary>
		/// <param name="qry">The select qry to convert</param>
		/// <param name="startRowIndex">The starting position</param>
		/// <param name="maximumSize">The size of the resultset</param>
		/// <param name="orderBy">The field to use to limit the results</param>
		/// <returns>SQL select query as string</returns>
		/// <remarks>Returns the passed select query with a wrapper around that limits the amount of returned results
		/// Works like adding the limit clause in MySQL
		/// Should ONLY be used when the 'orderOn' field is indexed otherwise large tables will be VERY slow
		/// </remarks>
		public virtual string LimitSelect(string qry, Int32 startRowIndex, Int32 maximumSize, string orderBy) {

			if (startRowIndex < 0)
				startRowIndex = 0;

			if (maximumSize < 0)
				maximumSize = Int32.MaxValue;

			return String.Format(@"select * from ( 
	select top {1} * from (
		select top {2} * from (
			{4}
		) as a order by {3} asc
	) as b order by {3} desc) as c 
order by {3} asc", startRowIndex, maximumSize, (startRowIndex + maximumSize), orderBy, qry);
		}

		/// <summary>
		/// Adds the top clause to a select statement
		/// </summary>
		/// <param name="qry">The select query to restrict</param>
		/// <param name="maximumSize">The maximum amount to view</param>
		/// <param name="orderBy">The order that is used to produce the top amount of anything</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual string TopSelect(string qry, Int32 maximumSize, string orderBy) {
			return String.Format("select top {0} from ({1}) as tmp_outer_top_qry", (maximumSize < 0 ? Int32.MaxValue : maximumSize), qry);
		}

		#endregion

		#region "Abstract Members"

		/// <summary>
		/// Simply returns a string type name for the database, to compare against
		/// </summary>
		public abstract string DatabaseType { get; }

		/// <summary>
		/// Returns the database type as a string representing the actual dbType of that database e.g. SqlDbType
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <remarks>
		/// Returns the database type as a string representing the actual dbType of that database e.g. SqlDbType. Slightly updated using some of the options
		/// that where showed in SubSonic as they had a couple of my missing types in MsSql and MySql
		/// </remarks>
		public abstract string GetDbSpecificType(DbType t);

		/// <summary>
		/// Clones the factory, so another copy can be used
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract object Clone();

		public abstract DbCommand CreateCommand(DbConnection conn);

		#endregion

		#region "Protected Members"

		/// <summary>
		/// Accepts a string containing a what should be a field name in a table, it first checks to see if the field has the quote identifiers round it, if it does
		/// it strips them then converts any spaces to underscores. It then places the passed char to the front of the resulting string.
		/// </summary>
		/// <param name="fieldName">The fieldname to convert to a parameter name.</param>
		/// <exception cref="ArgumentException">If the fieldname is empty.</exception>
		/// <returns></returns>
		/// <remarks>
		/// Accepts a string containing a what should be a field name in a table, it first checks to see if the field has the quote identifiers round it, if it does
		/// it strips them then converts any spaces to underscores. It then places the passed char to the front of the resulting string. This is not useful to all
		/// factories e.g. OleFactory where the parameter is purely '?' in the correct order, but 
		/// </remarks>
		protected String ParseParameterName(String fieldName, char parameterChar) {

			// If the string is empty throw exception
			if (string.IsNullOrEmpty(fieldName))
				throw new ArgumentException("The parameter name can not be blank");

			String[] parts = fieldName.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			// If the fieldname only contained dots, throw exception
			if (parts.Length == 0)
				throw new ArgumentException("The parameter name can not be blank");

			// When this is added to a particular factory class this bit needs to be overridden
			return parameterChar + parts[parts.Length - 1].Replace(' ', '_');
		}

		#endregion
	}
}



// No longer required dbProvider settings, simply use the DBProvider.GetFactory()
//<system.data>
//<!-- The dbprovider settings  -->
//<DbProviderFactories>
//<add name="MSSQL 2005 Data Provider" invariant="Toolbox.ADO.DbFactoryProvider.SQL2005Factory" description="Provides a data provider for MSSQL 2005 and above" type="Toolbox.ADO.DbFactoryProvider.SQL2005Factory, Toolbox, Version=2.0.0.1, Culture=neutral, PublicKeyToken=3c2773b309db8580" />
//<add name="MSSQL Data Provider" invariant="Toolbox.ADO.DbFactoryProvider.SQLFactory" description="Provides a data provider for SQL Server that has the benefits of the provider extensions" type="Toolbox.ADO.DbFactoryProvider.SQLFactory, Toolbox, Version=2.0.0.1, Culture=neutral, PublicKeyToken=3c2773b309db8580" />
//<add name="OleDb Data Provider" invariant="Toolbox.ADO.DbFactoryProvider.OleFactory" description="Provides a data provider for Ole databases that has the benefits of the provider extensions" type="Toolbox.ADO.DbFactoryProvider.OleFactory, Toolbox, Version=2.0.0.1, Culture=neutral, PublicKeyToken=3c2773b309db8580" />
//<add name="MySQL Data Provider" invariant="Toolbox.ADO.DbFactoryProvider.MySqlFactory" description="Provides a data provider for MySQL databases that has the benefits of the provider extension" type="Toolbox.ADO.DbFactoryProvider.MySqlFactory, Toolbox, Version=2.0.0.1, Culture=neutral, PublicKeyToken=3c2773b309db8580" />
//<add name="FoxPro Data Provider" invariant="Toolbox.ADO.DbFactoryProvider.FoxProFactory" description="Provides a specific data provider for FoxPro using an extended Ole driver" type="Toolbox.ADO.DbFactoryProvider.FoxProFactory, Toolbox, Version=2.0.0.1, Culture=neutral, PublicKeyToken=3c2773b309db8580" />
//</DbProviderFactories>
//</system.data>
