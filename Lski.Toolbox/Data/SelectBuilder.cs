using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace Lski.Toolbox.Data {

	/// <summary>
	/// A class used to stored different sections of a Select SQL query so that it can be manipulated at different places in any order and reused. 
	/// Internally uses the Petapoco 4 to combine the various parts and their parameters.
	/// </summary>
	public class SelectBuilder {

		private ICollection<string> _SelectList;
		public ICollection<string> SelectList {
			get { return _SelectList ?? (_SelectList = new List<String>()); }
			set { _SelectList = value ?? new List<String>(); }
		}

		private ICollection<string> _FromTables;
		public ICollection<string> FromTables {
			get { return _FromTables ?? (_FromTables = new List<String>()); }
			set { _FromTables = value ?? new List<String>(); } 
		}

		private ICollection<Joined> _Joins;
		public ICollection<Joined> Joins {
			get { return _Joins ?? (_Joins = new List<Joined>()); }
			set { _Joins = value ?? new List<Joined>(); }
		}

		private ICollection<WhereClause> _WhereClauses;
		public ICollection<WhereClause> WhereClauses {
			get { return _WhereClauses ?? (_WhereClauses = new List<WhereClause>()); }
			set { _WhereClauses = value ?? new List<WhereClause>(); } 
		}

		private ICollection<String> _GroupByClauses;
		public ICollection<String> GroupByClauses {
			get { return _GroupByClauses ?? (_GroupByClauses = new List<String>()); }
			set { _GroupByClauses = value ?? new List<String>(); }
		}

		private ICollection<String> _OrderByClauses;
		public ICollection<String> OrderByClauses {
			get { return _OrderByClauses ?? (_OrderByClauses = new List<String>()); }
			set { _OrderByClauses = value ?? new List<String>(); } 
		}

		public class SelectSql {

			public String SQL { get; set; }
			public Object[] Args { get; set; }
		}

		public SelectSql ToSQLCount {
			get {

				if (this.FromTables.Count() == 0)
					throw new Exception("The from clause requires at least one table");

				var qry = new SqlGenerator().Select("count(*)").From(this.FromTables.ToArray());

				foreach (Joined item in this.Joins) {

					if (item.GetType() == typeof(InnerJoined))
						qry.InnerJoin(item.Table).On(item.On, item._args);
					else
						qry.LeftJoin(item.Table).On(item.On, item._args);
				}

				foreach (WhereClause item in this.WhereClauses) {
					qry.Where(item._qry, item._args);
				}

				if (this.GroupByClauses.Count > 0)
					qry.GroupBy(this.GroupByClauses.ToArray());

				if (this.OrderByClauses.Count > 0)
					qry.OrderBy(this.OrderByClauses.ToArray());

				return new SelectSql() { SQL = qry.SQL, Args = qry.Arguments };
			}
		}

		/// <summary>
		/// Returns all the parts of this SQL statement combined into an SQL
		/// </summary>
		public SelectSql ToSQL {
			get {

				if(this.SelectList.Count() == 0)
					throw new Exception("The select statement requires at least one item to select");
				if(this.FromTables.Count() == 0)
					throw new Exception("The from clause requires at least one table");

				var qry = new SqlGenerator()
							.Select(this.SelectList.ToArray())
								.From(this.FromTables.ToArray());

				foreach (Joined item in this.Joins) {

					if (item.GetType() == typeof(InnerJoined))
						qry.InnerJoin(item.Table).On(item.On, item._args);
					else
						qry.LeftJoin(item.Table).On(item.On, item._args);
				}

				foreach (WhereClause item in this.WhereClauses) {
					qry.Where(item._qry, item._args);
				}

				if(this.GroupByClauses.Count > 0)
					qry.GroupBy(this.GroupByClauses.ToArray());

				if (this.OrderByClauses.Count > 0)
					qry.OrderBy(this.OrderByClauses.ToArray());

				return new SelectSql() { SQL = qry.SQL, Args = qry.Arguments };
			}
		}

		public SelectBuilder Select(String select) {
			this.SelectList.Add(select);
			return this;
		}

		public SelectBuilder Selects(params String[] select) {

			foreach (var item in select) {
				this.SelectList.Add(item);
			}
			return this;
		}

		public SelectBuilder From(String table) {
			this.FromTables.Add(table);
			return this;
		}

		public SelectBuilder From(params String[] tables) {
			
			foreach (var i in tables) {
				this.FromTables.Add(i);
			}
			return this;
		}

		public SelectBuilder InnerJoin(String table, String on, params Object[] args) {
			this.Joins.Add(new InnerJoined(table, on, args));
			return this;
		}

		public SelectBuilder LeftJoin(String table, String on, params Object[] args) {
			this.Joins.Add(new LeftJoined(table, on, args));
			return this;
		}

		public SelectBuilder Where(String qry, params Object[] args) {
			this.WhereClauses.Add(new WhereClause(qry, args));
			return this;
		}

		public SelectBuilder OrderBy(String orderBy) {
			this.OrderByClauses.Add(orderBy);
			return this;
		}

		public SelectBuilder OrderBy(params String[] orderBys) {

			foreach (var item in orderBys) {
				this.OrderByClauses.Add(item);
			}
			return this;
		}

		public SelectBuilder GroupBy(String groupBy) {
			this.GroupByClauses.Add(groupBy);
			return this;
		}

		public SelectBuilder GroupBy(params String[] groupBys) {

			foreach (var item in groupBys) {
				this.GroupByClauses.Add(item);
			}
			return this;
		}

		public abstract class Criteria {

			internal String _qry;
			internal Object[] _args;

			protected Criteria() {
				this._qry = String.Empty;
				this._args = new Object[] { };
			}

			public Criteria(String qry, params Object[] args) {
				this.Init(qry, args);
			}

			public void Init(String qry, params Object[] args) {
				this._qry = qry;
				this._args = args;
			}
		}

		public class WhereClause : Criteria {
 
			public WhereClause(String qry, params Object[] args) : base(qry, args) { }
		}

		public abstract class Joined : Criteria {

			// Temp fields until the PetaPoco.Sql object is no longer being used
			internal String Table { get; private set; }
			internal String On { get; private set; }

			// ready for when this qry becomes split from the PetaPoco one
			protected abstract String JoinKeyword { get; }

			public Joined(String table, String on, params Object[] args) {
				
				this.Init((JoinKeyword + " " + table + " ON " + on), args);
				this.Table = table;
				this.On = on;
			}
		}

		public class InnerJoined : Joined {

			protected override string JoinKeyword { get { return ""; } }

			public InnerJoined(String table, String on, params Object[] args) : base(table, on, args) { }
		}

		public class LeftJoined : Joined {

			protected override string JoinKeyword { get { return ""; } }

			public LeftJoined(String table, String on, params Object[] args) : base(table, on, args) { }
		}

		#region HelperMethods

		private static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// A function used to split an sql qry into useable parts. This is adapted from the query used within 
		/// </summary>
		/// <param name="sql">The qry to split</param>
		/// <param name="sqlCount"></param>
		/// <param name="sqlSelectRemoved"></param>
		/// <param name="sqlOrderBy"></param>
		/// <returns></returns>
		public static void SplitSql(string sql, out String body, out String orderBy) {

			// Extract the columns from "SELECT <whatever> FROM"
			var m = rxColumns.Match(sql);
			if (!m.Success)
				throw new ArgumentException("The SQL statement being parsed doesnt contain a select list");

			Group g = m.Groups[1];
			body = sql.Substring(g.Index + g.Length);

			// Look for an "ORDER BY <whatever>" clause, and extract it
			m = rxOrderBy.Match(body);
			if (m.Success) {

				orderBy = m.Groups[0].ToString();
				body = body.Substring(0, body.Length - orderBy.Length);
			} else
				orderBy = null;

			return;
		}

		/// <summary>
		/// A function used to split an sql qry and returns the body (select and orderby removed)
		/// </summary>
		/// <param name="sql">The qry to split</param>
		/// <returns></returns>
		public static String ExtractBody(string sql) {

			String body;

			// Extract the columns from "SELECT <whatever> FROM"
			var m = rxColumns.Match(sql);
			if (!m.Success)
				throw new ArgumentException("The SQL statement being parsed doesnt contain a select list");

			Group g = m.Groups[1];
			body = sql.Substring(g.Index + g.Length);

			// Look for an "ORDER BY <whatever>" clause, and remove it
			m = rxOrderBy.Match(body);
			if (m.Success)
				body = body.Substring(0, body.Length - m.Groups[0].ToString().Length);

			return body;
		}

		/// <summary>
		/// A function used to split an sql qry and returns the order by
		/// </summary>
		/// <param name="sql">The qry to split</param>
		/// <returns></returns>
		public static String ExtractOrderBy(string sql) {

			String orderBy = null;

			// Look for an "ORDER BY <whatever>" clause, and extract it
			var m = rxOrderBy.Match(sql);
			if (m.Success)
				orderBy = m.Groups[0].ToString();

			return orderBy ;
		}

		public static String StartsWith(String value, String wildcard = "%") {
			return (value == null ? null : String.Concat(value, wildcard));
		}

		public static String EndsWith(String value, String wildcard = "%") {
			return (value == null ? null : String.Concat(wildcard, value));
		}

		/// <summary>
		/// Returns a string 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="wildcard"></param>
		/// <returns></returns>
		public static String Contains(String value, String wildcard = "%") {
			return (value == null ? null : String.Concat(wildcard, value, wildcard));
		}

		/// <summary>
		/// Converts a startRowIndex to a page number, so that an ObjectDataSource can work with the value.
		/// </summary>
		/// <param name="startRowIndex"></param>
		/// <param name="maximumRows"></param>
		/// <returns></returns>
		public static Int32 ToPageNo(Int32 startRowIndex, Int32 maximumRows) {
			return (startRowIndex != 0 ? startRowIndex / maximumRows : 0) + 1;
		}
		
		#endregion

		#region PetaPoco

		// Simple helper class for building SQL statments
		private class SqlGenerator
		{
			public SqlGenerator()
			{
			}

			public SqlGenerator(string sql, params object[] args)
			{
				_sql = sql;
				_args = args;
			}

			public static SqlGenerator Builder
			{
				get { return new SqlGenerator(); }
			}

			string _sql;
			object[] _args;
			SqlGenerator _rhs;
			string _sqlFinal;
			object[] _argsFinal;

			private void Build()
			{
				// already built?
				if (_sqlFinal != null)
					return;

				// Build it
				var sb = new StringBuilder();
				var args = new List<object>();
				Build(sb, args, null);
				_sqlFinal = sb.ToString();
				_argsFinal = args.ToArray();
			}

			public string SQL
			{
				get
				{
					Build();
					return _sqlFinal;
				}
			}

			public object[] Arguments
			{
				get
				{
					Build();
					return _argsFinal;
				}
			}

			public SqlGenerator Append(SqlGenerator sql)
			{
				if (_rhs != null)
					_rhs.Append(sql);
				else
					_rhs = sql;

				return this;
			}

			public SqlGenerator Append(string sql, params object[] args)
			{
				return Append(new SqlGenerator(sql, args));
			}

			static bool Is(SqlGenerator sql, string sqltype)
			{
				return sql != null && sql._sql != null && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
			}

			private void Build(StringBuilder sb, List<object> args, SqlGenerator lhs)
			{
				if (!String.IsNullOrEmpty(_sql))
				{
					// Add SQL to the string
					if (sb.Length > 0)
					{
						sb.Append("\n");
					}

					var sql = ProcessParams(_sql, _args, args);

					if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
						sql = "AND " + sql.Substring(6);
					if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
						sql = ", " + sql.Substring(9);

					sb.Append(sql);
				}

				// Now do rhs
				if (_rhs != null)
					_rhs.Build(sb, args, this);
			}

			public SqlGenerator Where(string sql, params object[] args)
			{
				return Append(new SqlGenerator("WHERE (" + sql + ")", args));
			}

			public SqlGenerator OrderBy(params object[] columns)
			{
				return Append(new SqlGenerator("ORDER BY " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
			}

			public SqlGenerator Select(params object[] columns)
			{
				return Append(new SqlGenerator("SELECT " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
			}

			public SqlGenerator From(params object[] tables)
			{
				return Append(new SqlGenerator("FROM " + String.Join(", ", (from x in tables select x.ToString()).ToArray())));
			}

			public SqlGenerator GroupBy(params object[] columns)
			{
				return Append(new SqlGenerator("GROUP BY " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
			}

			private SqlJoinClause Join(string JoinType, string table)
			{
				return new SqlJoinClause(Append(new SqlGenerator(JoinType + table)));
			}

			public SqlJoinClause InnerJoin(string table) { return Join("INNER JOIN ", table); }
			public SqlJoinClause LeftJoin(string table) { return Join("LEFT JOIN ", table); }

			public class SqlJoinClause
			{
				private readonly SqlGenerator _sql;

				public SqlJoinClause(SqlGenerator sql)
				{
					_sql = sql;
				}

				public SqlGenerator On(string onClause, params object[] args)
				{
					return _sql.Append("ON " + onClause, args);
				}
			}

			// Helper to handle named parameters from object properties
			static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
			public static string ProcessParams(string _sql, object[] args_src, List<object> args_dest) {
				return rxParams.Replace(_sql, m => {
					string param = m.Value.Substring(1);

					object arg_val;

					int paramIndex;
					if (int.TryParse(param, out paramIndex)) {
						// Numbered parameter
						if (paramIndex < 0 || paramIndex >= args_src.Length)
							throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length, _sql));
						arg_val = args_src[paramIndex];
					} else {
						// Look for a property on one of the arguments with this name
						bool found = false;
						arg_val = null;
						foreach (var o in args_src) {
							var pi = o.GetType().GetProperty(param);
							if (pi != null) {
								arg_val = pi.GetValue(o, null);
								found = true;
								break;
							}
						}

						if (!found)
							throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, _sql));
					}

					// Expand collections to parameter lists
					if ((arg_val as System.Collections.IEnumerable) != null &&
						(arg_val as string) == null &&
						(arg_val as byte[]) == null) {
						var sb = new StringBuilder();
						foreach (var i in arg_val as System.Collections.IEnumerable) {
							sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
							args_dest.Add(i);
						}
						return sb.ToString();
					} else {
						args_dest.Add(arg_val);
						return "@" + (args_dest.Count - 1).ToString();
					}
				}
				);
			}
		}

		#endregion

	}
}
