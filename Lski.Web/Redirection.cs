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

		public TransferType Type { get; set; }
		public String Location { get; set; }
		public HttpContextBase Context { get; set; }

		public Redirection() 
			: this(new HttpContextWrapper(HttpContext.Current), "", TransferType.Redirect) {}

		public Redirection(HttpContextBase context, String location, TransferType transferType) {

			this.Context = context;
			this.Type = transferType;
			this.Location = location;
		}

		public void Go()
		{
			if (Type == TransferType.Transfer)
				this.Context.Server.Transfer(this.Location);
			else
				this.Context.Response.Redirect(this.Location, false);
			
		}
		
	}


}

