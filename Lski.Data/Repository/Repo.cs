using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Configuration;

namespace Lski.Data.Repository {
	
	/// <summary>
	/// Creates a class that can maintain a shared connection (OpenConnection) and shared transaction object (BeginTransaction) where when nested still only uses the single
	/// connection and transaction. Best used by subclassing and providing the connection information with the overloaded constructor.
	/// </summary>
	public class Repo : IDisposable {

		public Repo(String connectionStringName) {

			var cs = ConfigurationManager.ConnectionStrings[connectionStringName];

			if (cs == null)
				throw new ArgumentException("The connection string '" + connectionStringName + "' does not exist");

			this.Init(cs.ConnectionString, cs.ProviderName);
		}

		public Repo(String connectionString, String providerName) {
			this.Init(connectionString, providerName);
		}

		internal Repo(String connectionString, DbProviderFactory factory) {
			this.Init(connectionString, factory);
		}

		protected virtual void Init(String connectionString, String providerName) {
			this.Init(connectionString, this.GetFactory(connectionString, providerName));
		}

		protected virtual void Init(String connectionString, DbProviderFactory factory) {

			this.ConnectionString = connectionString;
			this.Provider = factory;
		}

		private String _connectionString;
		private DbProviderFactory _provider;
		private SharedConnection _connection;
		private Int16 _connectionLevel = 0;

		/// <summary>
		/// The connection string used to connect to the appropriate database
		/// </summary>
		public String ConnectionString {
			get { return _connectionString; }
			set { _connectionString = value; }
		}

		/// <summary>
		/// The provider used to connect to the database
		/// </summary>
		public DbProviderFactory Provider {
			get { return _provider; }
			set { _provider = value; }
		}

		/// <summary>
		/// Simply returns the factory object for this repository
		/// </summary>
		/// <param name="providerName"></param>
		/// <returns></returns>
		protected virtual DbProviderFactory GetFactory(string connectionString, string providerName) {
            return DbProviderFactories.GetFactory(providerName);
        }

		public SharedConnection OpenConnection() {

			_connectionLevel++;
			return _connection ?? (_connection = new SharedConnection(this));
		}

		internal void CloseConnection() {

			// If there are no more levels left then simply close the connection
			if((_connectionLevel--) == 0)
				_connection.Connection.Close();
		}

		public void Dispose() {

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// If when the object is disposed the transaction has NOT been completed then the transaction will be rolled back, a'la TransactionScope
		/// </summary>
		public void Dispose(Boolean disposing) {

			// If disposing equals true, dispose all managed and unmanaged resources.
			if (disposing) {

				if (_connection != null && _connection.Connection != null && _connection.Connection.State == System.Data.ConnectionState.Open)
					_connection.Connection.Close();
			}	
		}
	}
}
