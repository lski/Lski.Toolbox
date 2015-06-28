using System;
using System.Collections;
using System.Collections.Generic;

namespace Lski.Toolbox.Collections {

	/// <summary>
	/// Wrapper class for handling enumerables where each item is of type Disposable. It enables the enumerable to be used with a Dispose, where each disposable is disposed
	/// and therefore can be used with the using statement.
	///
	/// E.g.
	/// <code>
	/// using (var streams = new DisposableEnumerator<FileStream>(filepaths.Select(x => File.OpenRead(x)))) {
	///		// Do something with stream */
	/// }
	/// </code>
	/// 
	/// NB: Does not run the IEnumerable passed in until run itself via an enumerable or ToList() etc.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DisposableEnumerator<T> : IEnumerable<T>, IDisposable where T : IDisposable {

		private IEnumerable<T> _collection;

		public DisposableEnumerator(IEnumerable<T> collection) {

			if (collection == null) {
				throw new ArgumentNullException(nameof(collection));
			}

			_collection = collection;
		}

		public void Dispose() {

			foreach (var item in _collection) {
				if (item != null) {
					item.Dispose();
				}
			}
		}

		public IEnumerator<T> GetEnumerator() {

			return _collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {

			return this.GetEnumerator();
		}
	}
}