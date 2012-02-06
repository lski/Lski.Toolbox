using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Lski.Data.Exceptions;
using Lski.Data.Common.Exceptions;

namespace Lski.Data.Common {
	
	public static class DbProviders {

		/// <summary>
		/// A method for getting the advanced DbProvider, the combination of connection string and provider name give a greater amount of precision, than simply providername 
		/// </summary>
		/// <param name="connectionStringName"></param>
		/// <returns></returns>
		public static DbProvider GetFactoryPrecise(String connectionStringName) {

			var cs = ConfigurationManager.ConnectionStrings[connectionStringName];

			if (cs == null)
				throw new ArgumentException(String.Format("The connection string {0} could not be found", connectionStringName));

			return GetFactoryPrecise(cs.ConnectionString, cs.ProviderName);
		}

		/// <summary>
		/// A method for getting the advanced DbProvider, the combination of connection string and provider name give a greater amount of precision, than simply providername 
		/// </summary>
		/// <param name="connectionStringName"></param>
		/// <returns></returns>
		public static DbProvider GetFactoryPrecise(String connectionString, String providerName) {

			if (providerName == "DbFactory.SqlServer2005")
				return new SqlServer2005Factory();

			if (providerName == "DbFactory.SqlServer" || providerName == "System.Data.SqlClient")
				return new SqlServerFactory();

			if (providerName == "DbFactory.MySql" || providerName == "MySql.Data.MySQLClient")
				return new MySqlFactory();

			if (providerName == "DbFactory.FoxPro" || (providerName == "System.Data.OleDb" && connectionString.StartsWith("Provider=vfpoledb", StringComparison.OrdinalIgnoreCase)))
				return new FoxProFactory();

			if (providerName == "DbFactory.Access" || (providerName == "System.Data.OleDb" && connectionString.StartsWith("Provider=Microsoft.Jet.OLEDB.4.0", StringComparison.OrdinalIgnoreCase)))
				return new AccessFactory();

			if (providerName == "DbFactory.OleDb" || providerName == "System.Data.OleDb")
				return new OleFactory();

			throw new ProviderNotSupportedException(providerName);
		}

		/// <summary>
		/// Returns a DbProvider for the appropriate factory depending on the providername given. Not recommended unless using the DbFactory.XXX provider names.
		/// </summary>
		/// <param name="connectionStringName"></param>
		/// <returns></returns>
		public static DbProvider GetFactory(String providerName) {

			switch (providerName) {
				case "DbFactory.SqlServer2005":
					return new SqlServer2005Factory();
				case "DbFactory.SqlServer":
				case "System.Data.SqlClient":
					return new SqlServerFactory();
				case "DbFactory.MySql":
				case "MySql.Data.MySQLClient":
					return new MySqlFactory();
				case "DbFactory.FoxPro":
					return new FoxProFactory();
				case "DbFactory.Access":
					return new AccessFactory();
				case "DbFactory.OleDb":
				case "System.Data.OleDb":
					return new OleFactory();
				default:
					throw new ProviderNotSupportedException(providerName);
			}
		}
	}
}
