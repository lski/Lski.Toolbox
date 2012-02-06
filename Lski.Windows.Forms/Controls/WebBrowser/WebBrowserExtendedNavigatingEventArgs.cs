using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lski.Windows.Forms.Controls.WebBrowser {

	/// <summary>
	/// Define a new EventArgs class to contain the newly exposed data
	/// </summary>
	public class WebBrowserExtendedNavigatingEventArgs : CancelEventArgs {
		private string _Url;
		public string Url {
			get { return _Url; }
		}

		private string _Frame;
		public string Frame {
			get { return _Frame; }
		}

		public WebBrowserExtendedNavigatingEventArgs(string url, string frame)
			: base() {
			_Url = url;
			_Frame = frame;
		}
	}
}
