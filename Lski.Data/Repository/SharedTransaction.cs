using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Lski.Data.Repository {

	public class SharedTransaction : IDisposable {

		private Boolean _completed = false;
		private SharedConnection _connection;
		private DbTransaction _transaction;

		/// <summary>
		/// The underlying transaction object for this shared transaction/connection
		/// </summary>
		public DbTransaction Transaction { get { return _transaction; } }

		internal SharedTransaction(SharedConnection conn) {
			_connection = conn;
			_transaction = conn.Connection.BeginTransaction();
		}

		public void Complete() {
			_completed = true;
			_connection.CompleteTransaction();
		}

		public void Rollback() {
			_connection.RollbackTransaction();
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

				// When reaching here check the if this level has been completed, if not, then rollback
				if (!_completed)
					Rollback();
			}
		}
	}
}
