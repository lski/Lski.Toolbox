using Lski.Txt.ConvertTo;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lski.IO.Csv {
	
	internal class InternalCsvImportMapLink {

		/// <summary>
		/// Line position within the Csv line
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// The property to map the value too.
		/// </summary>
		public PropertyInfo Property { get; set; }

		/// <summary>
		/// An override for the conversion process, default used if null
		/// </summary>
		public ConvertTo Conversion { get; set; }

		/// <summary>
		/// A list of tranformations to run on the data prior to conversion
		/// </summary>
		public Transformations Tranformations { get; set; }


		public static ICollection<InternalCsvImportMapLink> CreateInternalLinks<T>(ICollection<CsvImportLink> links) {

			var lst = new List<InternalCsvImportMapLink>();

			if (links != null) {

				var qry = (from p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
						   join l in links on p.Name.ToLowerInvariant() equals l.Property.ToLowerInvariant()
						   select new { Property = p, Link = l });


				foreach (var pl in qry) {

					lst.Add(new InternalCsvImportMapLink() { 
						Property = pl.Property, 
						Position = pl.Link.Position,
						Conversion = pl.Link.Conversion ?? ConvertTo.GetConverter(pl.Property.PropertyType), 
						Tranformations = pl.Link.Tranformations 
					});
				}
			}

			return lst;
		}
	}
}
