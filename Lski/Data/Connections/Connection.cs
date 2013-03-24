using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Lski.Data.Connections.Exceptions;
using System.Collections.Concurrent;

namespace Lski.Data.Connections {

	/// <summary>
	/// A basic selection of static methods associated with connected to a database and the DbConnection object and their providers
	/// </summary>
	public class Connection {

		public static DbProviderFactory GetFactory(String connectionStringName) {

			var css = ConfigurationManager.ConnectionStrings[connectionStringName];
			return DbProviderFactories.GetFactory(css.ProviderName);
		}

		public static DbConnection GetConnection(String connectionStringName) {

            var css = ConfigurationManager.ConnectionStrings[connectionStringName];
			var conn = DbProviderFactories.GetFactory(css.ProviderName).CreateConnection();
			conn.ConnectionString = css.ConnectionString;

			return conn;
		}

		/// <summary>
		/// Returns the connection string details based on the connection string name that was supplied
		/// </summary>
		/// <param name="connectionStringName"></param>
		/// <returns></returns>
		protected static ConnectionStringSettings GetConnectionStringDetails(String connectionStringName) {

			var cs = ConfigurationManager.ConnectionStrings[connectionStringName];

			if (cs == null)
				throw new ConnectionStringNotFoundException(connectionStringName);

			return cs;
		}

		/// <summary>
		/// Tries to open the connection object that has been passed. If the connection was already open, or the 
		/// </summary>
		/// <param name="conn">The connection object to open</param>
		/// <returns>Whether the connection was opened in this method (true) or was already opened (false)</returns>
		/// <remarks></remarks>
		public static bool OpenConnection(ref DbConnection conn) {

			var connState = conn.State;

			if (connState == ConnectionState.Broken || connState == ConnectionState.Closed) {

				conn.Open();

				// If the connection was mearly broken and not closed, dont state this is implicitly opened
				if (connState == ConnectionState.Closed)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Simple close connection method, only attempts to close, if currently open
		/// </summary>
		/// <param name="conn"></param>
		/// <remarks></remarks>
		public static void CloseConnection(DbConnection conn) {

			if (conn != null && conn.State == ConnectionState.Open)
				conn.Close();
		}

		/// <summary>
		/// Close the connection object passed, but only if it is open. Any error messages can be suppressed by setting suppressError to True (there are recorded however)
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="suppressError"></param>
		/// <remarks></remarks>
		public static void CloseConnection(DbConnection conn, bool suppressError) {

			if (conn != null && conn.State == ConnectionState.Open) {

				if (suppressError) {

					try {
						conn.Close();
					} 
					catch (Exception) {
					}

				} 
				else {
					conn.Close();
				}

			}

		}
	}
}
