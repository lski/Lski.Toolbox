using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IssueTracking {

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public class ToFix : System.Attribute {

		public Int32 TicketID { get; set; }
		public String Description { get; set; }
		public DateTime? IssueRaisedOn { get; set; }

		public ToFix(String description, String issueRaisedOn) {

			this.Description = description;
			DateTime tmpDate;
			if (DateTime.TryParse(issueRaisedOn, out tmpDate))
				this.IssueRaisedOn = tmpDate;
		}

	}
}
