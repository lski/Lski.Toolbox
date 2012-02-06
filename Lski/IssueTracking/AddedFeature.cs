using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IssueTracking {

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public class AddedFeature : System.Attribute {

		public String Description { get; set; }
		public DateTime? DateAdded { get; set; }
		public Int32 FeatureID { get; set; }
		public String Programmer { get; set; }

		public AddedFeature(String desc, String dateAdded) {

			this.Programmer = String.Empty;
			this.Description = desc;

			DateTime tmpDate;
			if (DateTime.TryParse(dateAdded, out tmpDate))
				this.DateAdded = tmpDate;
		}
	}
}
