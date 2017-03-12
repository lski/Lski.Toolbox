using System;
using System.Collections;
using System.Collections.Generic;

namespace Lski.Toolbox.Collections {

	/// <summary>
	/// Wrapper class for handling enumerables where each item&lt;T&gt; that are of type <see cref="IDisposable"/>.
	/// It can be used as a belt &amp; braces approach to ensure all items are disposed.
	/// NB: iterations are not cached in advance.
	/// </summary>
	/// <example><code>
	/// using (var streams = filepaths.Select(x => File.OpenRead(x)).AsDisposable()) {
	///		foreach (var item in streams) {
	///				// Do somthing with the streams
	///			}
	///		}
	///	}
	/// </code></example>
	/// <remarks>
	///Note: Internal enumerables are not cached/run before the <see cref="DisposableEnumerator{T}"/> is enumerated itself
	/// </remarks>
	public class DisposableEnumerator<T> : IEnumerable<T>, IDisposable where T : IDisposable {

		private readonly IEnumerable<T> _collection;

		/// <summary>
		/// Creates the <see cref="DisposableEnumerator{T}"/> with the <see cref="IEnumerable{T}"/> of <see cref="IDisposable"/> objects
		/// </summary>
		/// <param name="enumerable">The enumerable to loop through</param>
		public DisposableEnumerator(IEnumerable<T> enumerable) {

			if (enumerable == null) {
				throw new ArgumentNullException(nameof(enumerable));
			}

			_collection = enumerable;
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