using Lski.Txt;
using Lski.Txt.Conversion;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lski.IO.Csv {

	public class CsvExport {

		public CsvExportSettings Settings { get; set; }

		public CsvExport(CsvExportSettings settings) {
			Settings = settings;
		}

		public ICsvExportWriter Export<T>(string filepath) where T : new() {
			return this.InternalFileExport<T>(filepath);
		}

		public void Export<T>(string filepath, params T[] values) {

			using (var exporter = this.InternalFileExport<T>(filepath)) {
				
				foreach (var val in values) {
					exporter.Add(val);
				}
			}
		}

		public void Export<T>(string filepath, IEnumerable<T> values) {

			using (var exporter = this.InternalFileExport<T>(filepath)) {

				foreach (var val in values) {
					exporter.Add(val);
				}
			}
		}
		
		public ICsvExportWriter Export<T>(StreamWriter writer) where T : new() {
			return this.InternalFileExport<T>(writer);
		}

		public void Export<T>(StreamWriter writer, params T[] values) {

			using (var exporter = this.InternalFileExport<T>(writer)) {

				foreach (var val in values) {
					exporter.Add(val);
				}
			}
		}

		public void Export<T>(StreamWriter writer, IEnumerable<T> values) {

			using (var exporter = this.InternalFileExport<T>(writer)) {

				foreach (var val in values) {
					exporter.Add(val);
				}
			}
		}

		protected ICsvExportWriter InternalFileExport<T>(string filepath) {

			bool appendHeaderCalculated = Settings.Header;
			var writer = GetFileWriter(filepath, Settings.AppendToFile, ref appendHeaderCalculated);
			Settings.Header = appendHeaderCalculated;

			return new CsvExportWriter(Settings, CreateInternalLinks<T>(Settings.Links, Settings), writer, true);
		}

		protected ICsvExportWriter InternalFileExport<T>(StreamWriter writer) {
			return new CsvExportWriter(Settings, CreateInternalLinks<T>(Settings.Links, Settings), writer);
		}


		protected StreamWriter GetFileWriter(string filepath, bool appendToFile, ref bool appendHeader) {

			FileStream csvStream = null;

			try {
				// 1. If the file does not exist then create it
				// 2. If the user wants this data appended to the end of the file then
				if (appendToFile) {

					// If this set is to be appended to a file (and the file exists) then remove the header
					if (File.Exists(filepath)) {
						appendHeader = false;
					}

					csvStream = new FileStream(filepath, FileMode.Append, FileAccess.Write);
				}
				else {
					csvStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
				}

				return new StreamWriter(csvStream);

			}
			catch (Exception ex) {
				throw new Exception("There was an error opening the csv file '" + filepath + "'", ex);
			}


		}

		public static ICollection<InternalCsvLink> CreateInternalLinks<T>(ICollection<CsvExportLink> links, CsvExportSettings settings) {

			var lst = new List<InternalCsvLink>();
			var textTransformation = new ToCsv(settings.TextDelimiter, settings.TextDelimiter, settings.EnforceTextDelimiter);

			if (links != null) {

				var qry = (from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
						   join l in links on p.Name.ToLowerInvariant() equals l.Property.ToLowerInvariant()
						   select new { Property = p, Link = l });

				foreach (var pl in qry) {

					var transformations = new Transformations();

					// If this is NOT a priimative type then add the tranformation for escaping 
					if (!IsPrimative(pl.Property.PropertyType))
						transformations.Add(textTransformation);

					lst.Add(new InternalCsvLink() {
						Property = pl.Property,
						Position = pl.Link.Position,
						Conversion = pl.Link.Conversion ?? ConvertTo.GetConverter(pl.Property.PropertyType),
						Tranformations = transformations
					});
				}
			}
			// Or generate them completely from the class itself
			else {

				var qry = (from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance) select p).Select((prop, index) => new { Property = prop, Index = index }).ToList();
				foreach (var item in qry) {
					lst.Add(new InternalCsvLink() {
						Property = item.Property,
						Position = item.Index,
						Conversion = ConvertTo.GetConverter(item.Property.PropertyType),
						Tranformations = new Transformations(textTransformation)
					});
				}

			}

			return lst;
		}

		protected static bool IsPrimative(Type type) {

			if (type.IsPrimitive)
				return true;
			else if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? type.GetGenericArguments()[0].IsPrimitive : type.IsPrimitive))
				return true;
			else
				return false;
		}

		public class CsvExportWriter : IDisposable, Lski.IO.Csv.ICsvExportWriter {

			private StreamWriter _internalStream;
			private bool _autoClose = false;
			private bool _firstAdd = true;
			private CsvExportSettings _settings;
			private ICollection<InternalCsvLink> _links;

			public CsvExportWriter(CsvExportSettings settings, ICollection<InternalCsvLink> links, StreamWriter writer, bool autoClose = false) {

				_settings = settings;
				_links = links;
				_internalStream = writer;
			}

			public void Dispose() {

				// Only closes the internal stream if created from the parent item
				if (_autoClose && _internalStream != null) {
					_internalStream.Dispose();
				}
			}

			public void Add<T>(T obj) {

				if (_firstAdd) {
					
					if(_settings.Header) {
						_internalStream.WriteLine(ToCsvHeader(_settings, _links));
					}

					_firstAdd = false;
				}

				_internalStream.WriteLine(ToCsvLine(obj, _settings, _links));
				_internalStream.Flush();
			}

			/// <summary>
			/// Provides a simple method for creating the header line from the datatable
			/// </summary>
			/// <param name="columns">The columns collection desired, usually table.Columns</param>
			/// <returns></returns>
			/// <remarks></remarks>
			protected virtual string ToCsvHeader(CsvExportSettings settings, ICollection<InternalCsvLink> links) {

				var converter = new ToCsv(settings.Delimiter, settings.TextDelimiter, settings.EnforceTextDelimiter);

				return String.Join(settings.Delimiter, links.Select(x => converter.Process(x.Property.Name)));
			}

			/// <summary>
			/// Provides a simple method for creating a line from the the column collection, row passed and delimiter
			/// </summary>
			/// <param name="row">The row containing values</param>
			/// <param name="columns">The columns collection desired, usually table.Columns</param>
			/// <returns></returns>
			/// <remarks></remarks>
			protected virtual string ToCsvLine(Object obj, CsvExportSettings settings, ICollection<InternalCsvLink> links) {

				return String.Join(settings.Delimiter, links.Select(x => {

					var item = x.Property.GetValue(obj, null);

					if (item == null)
						return settings.NULL;
					else
						return x.Conversion.ToString(item);
				
				}));

			}

		}


	}
}
