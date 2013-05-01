using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using IfacesEnumsStructsClasses;

namespace Lski.Windows.Forms.Controls.WebBrowser {

	/// <summary>
	/// Extend the WebBrowser control
	/// </summary>
	public class ExtendedWebBrowser : System.Windows.Forms.WebBrowser
	{

		public delegate ExtendedWebBrowser GetNewFormDelegate();

		private System.Windows.Forms.AxHost.ConnectionPointCookie cookie;

		private WebBrowserExtendedEvents events;

		public GetNewFormDelegate GetNewForm;

		#region "Constructors"

		public ExtendedWebBrowser()
		{
		}

		public ExtendedWebBrowser(GetNewFormDelegate func)
		{
			this.GetNewForm = func;
		}

		#endregion


		//This method will be called to give you a chance to create your own event sink
		protected override void CreateSink()
		{
			//MAKE SURE TO CALL THE BASE or the normal events won't fire
			base.CreateSink();
			events = new WebBrowserExtendedEvents(this);
			cookie = new System.Windows.Forms.AxHost.ConnectionPointCookie(this.ActiveXInstance, events, typeof(IfacesEnumsStructsClasses.DWebBrowserEvents2));
		}

		protected override void DetachSink()
		{
			if (cookie != null) {
				cookie.Disconnect();
				cookie = null;
			}
			base.DetachSink();
		}

		//This new event will fire when the page is navigating
		public event EventHandler<WebBrowserExtendedNavigatingEventArgs> ExtendedNavigating;

		protected void OnExtendedNavigating(string url, string frame, bool cancel)
		{
			WebBrowserExtendedNavigatingEventArgs args = new WebBrowserExtendedNavigatingEventArgs(url, frame);
			if (ExtendedNavigating != null) {
				ExtendedNavigating(this, args);
			}
			//Pass the cancellation chosen back out to the events
			cancel = args.Cancel;
		}

		//This new event will fire when the program tries to open a new window
		public event EventHandler<CancelEventArgs> ExtendedNewWindow;

		protected void OnExtendedNewWindow()
		{
			CancelEventArgs args = new CancelEventArgs();
			if (ExtendedNewWindow != null) {
				ExtendedNewWindow(this, args);
			}
		}

		//This class will capture events from the WebBrowser
		private class WebBrowserExtendedEvents : System.Runtime.InteropServices.StandardOleMarshalObject, IfacesEnumsStructsClasses.DWebBrowserEvents2
		{

			private ExtendedWebBrowser _Browser;
			public WebBrowserExtendedEvents(ExtendedWebBrowser browser)
			{
				_Browser = browser;
			}

			//Implement whichever events you wish
			public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
			{
				_Browser.OnExtendedNavigating((string)URL, (string)targetFrameName, cancel);
			}


			public void NewWindow2(ref object ppDisp, ref bool Cancel)
			{

				if (_Browser.GetNewForm == null) {
					Cancel = true;
					_Browser.OnExtendedNewWindow();
					return;
				}

				var newBrowser = _Browser.GetNewForm();

				//TabPage tmp = new TabPage();
				//LoginSystem.WebBrowserTab wbTmp = new LoginSystem.WebBrowserTab();
				//wbTmp.Dock = DockStyle.Fill;
				//tmp.Controls.Add(wbTmp);
				//tabBrowsers.TabPages.Add(tmp);
				//Form f = new Form();
				//f.WindowState = FormWindowState.Maximized;
				//f.Controls.Add(wbTmp.WebPane);
				var bro = (SHDocVw.WebBrowser)newBrowser.ActiveXInstance;
				bro.RegisterAsBrowser = true;
				ppDisp = bro.Application;
			}

			public void NavigateComplete2(			[MarshalAs(UnmanagedType.IDispatch)]
object pDisp, 			[In()]

ref object URL)
			{
			}
		}
	}

	


}

