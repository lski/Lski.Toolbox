using System;

namespace Lski.IssueTracking {
	
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public class BugFix : System.Attribute {
		
		public String Description { get; set; }
		public DateTime? DateFixed { get; set; }

		public Int32 IssueID { get; set; }
		public String Programmer { get; set; }

		public BugFix(string desc, string dateFixed) {

			this.Programmer = String.Empty;
			this.Description = desc;

			DateTime tmpDate;
			if (DateTime.TryParse(dateFixed, out tmpDate))
				this.DateFixed = tmpDate;
		}
	}
}
