using System;

namespace Lski.IO.Csv {

	/// <summary>
	/// Allows for handling of adding new records to the CSV one at a time, and allows for disposal of the necessary resources
	/// </summary>
	public interface ICsvExportWriter : IDisposable {

		/// <summary>
		/// Add a new line to the csv file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		void Add<T>(T obj);
	}
}
