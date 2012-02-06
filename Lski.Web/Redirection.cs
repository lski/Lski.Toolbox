using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Lski.Web
{

	/// <summary>
	/// To get round the issue of a try/catch/finally being disrupted by transfer and redirection, fill the details of the redirection in this class
	/// and run at the end of finally if not null.
	/// </summary>
	public class Redirection
	{
		public enum TransferType
		{
			Redirect,
			Transfer
		}

		private TransferType _transferType = TransferType.Redirect;
		public TransferType Type {
			get { return _transferType; }
			set { _transferType = value; }
		}

		private String _location;
		public String Location {
			get { return _location; }
			set { _location = value; }
		}

		public Redirection(TransferType transferType, String location)
		{
			this._transferType = transferType;
			this._location = location;
		}


		public void Go()
		{
			if (_transferType == TransferType.Transfer) {
				System.Web.HttpContext.Current.Server.Transfer(this.Location);
			} else {
				System.Web.HttpContext.Current.Response.Redirect(this.Location, false);
			}
		}
	}


}

