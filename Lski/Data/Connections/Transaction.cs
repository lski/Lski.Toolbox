using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Lski.Data.Connections {

	/// <summary>
	/// A wrapper for a DbTransaction object that implements the IDisposible interface so the transaction can be used with a using statement and commit prior to the end of the using
	/// </summary>
	public class Transaction : IDisposable {

		private Boolean _completed = false;
		private DbConnection _connection;
		private DbTransaction _transaction;

		public Transaction(DbConnection conn) {
			_connection = conn;
			_transaction = _connection.BeginTransaction();
		}

		public DbTransaction BaseTransaction {
			get {
				return _transaction;
			}
		}

		public void Commit() {
			_completed = true;
			_transaction.Commit();
		}

		public void Rollback() {
			_transaction.Rollback();
			_transaction.Dispose();
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

		/// <summary>
		/// Receives a transaction object and attempts to roll it back, if transaction is null, simply returns. If marked as suppress error it hides any error
		/// thrown
		/// </summary>
		/// <param name="tran"></param>
		/// <param name="suppressError"></param>
		/// <remarks></remarks>
		[Obsolete("Use the transaction class to wrap the IDbTransaction object rather than using it directly")]
		public static void AttemptRollback(DbTransaction tran, bool suppressError = true) {

			if (suppressError) {

				try {

					if (tran != null)
						tran.Rollback();

				}
				catch (Exception) {
				}
			
			}
			else {

				if (tran != null)
					tran.Rollback();
			}
		}
	}
}
