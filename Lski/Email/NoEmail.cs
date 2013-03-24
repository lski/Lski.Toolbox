using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Email {
	
	public class NoEmail : Email {

		public NoEmail() : base() {}

		/// <summary>
		/// Provides a stub for email, meaning when called in a collection this settings/broker object doesnt not attempt to actually send anything.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="format"></param>
		/// <param name="cc"></param>
		/// <param name="bcc"></param>
		public override void Send(string from, IEnumerable<string> to, string subject, string body, bool isHtml = false, IEnumerable<string> cc = null, IEnumerable<string> bcc = null) {
			return;
		}
	}
}
