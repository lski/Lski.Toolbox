using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lski.Windows.Forms.Controls.WebBrowser {

	/// <summary>
	/// Define a new EventArgs class to contain the newly exposed data
	/// </summary>
	public class WebBrowserExtendedNewWindowEventArgs : CancelEventArgs {

		private string _newUrl;
		public string NewUrl {
			get { return _newUrl; }
		}

		public WebBrowserExtendedNewWindowEventArgs(string newUrl)
			: base() {
			this._newUrl = newUrl;
		}
	}
}
