using System;

namespace Lski.IO.Csv {

	public interface ICsvExportWriter : IDisposable {

		void Add<T>(T obj);

	}
}
