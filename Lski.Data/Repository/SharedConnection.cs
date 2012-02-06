using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Lski.Data.Repository {

	public class SharedConnection : IDisposable {

		internal SharedConnection(Repo repo) {
			_repo = repo;
			_connection = repo.Provider.CreateConnection();
			_connection.ConnectionString = repo.ConnectionString;
			_connection.Open();
		}

		private Repo _repo;
		/// <summary>
		/// The transaction object, if any, held by this connection
		/// </summary>
		private SharedTransaction _transaction;
		/// <summary>
		/// The amount of times the _transaction has tried to be opened
		/// </summary>
		private Int16 _transactionLevel = 0;
		/// <summary>
		/// Just a flag to say 
		/// </summary>
		private Boolean _transactionRolledBack = false;

		/// <summary>
		/// The underlying connection object this shared object is holding
		/// </summary>
		public DbConnection _connection;

		public DbConnection Connection { get { return _connection; } }
		public Boolean HasTransaction { get; set; }

		public SharedTransaction BeginTransaction() {

			// If this is after a rollback then cleanup the transaction management flags
			if (_transactionRolledBack)
				_transactionRolledBack = false;

			_transactionLevel++;
			return _transaction ?? (_transaction = new SharedTransaction(this));
		}

		internal void RollbackTransaction() {

			_transactionRolledBack = true;
			_transactionLevel = 0;
			_transaction.Transaction.Rollback();
		}

		internal void CompleteTransaction() {
		
			if(_transactionRolledBack)
				throw new InvalidOperationException("The transaction can not be completed as it has already been rolled back");

			// If no transaction then its been destoryed
			if (_transaction == null)
				throw new NullReferenceException("The transaction object was null so could not be completed");

			// If there is no more layers to remove & commit the transaction
			if ((--_transactionLevel) == 0) {
				_transaction.Transaction.Commit();
				_transaction = null;
			}
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
			if (disposing)
				_repo.CloseConnection();
		}
	}
}
