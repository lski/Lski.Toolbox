using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Lski.Txt;
using System.Web.UI;

namespace Lski.Web {
	
	public class WebMethods {

		/// <summary>
		/// Takes a url, either virtual or physical, and returns a strin which should hopefully give a physical physical path. Does not prevent errors, if the url is malformed
		/// </summary>
		/// <param name="urlToParse">The relative or phyical</param>
		/// <returns></returns>
		public static String ResolveUrl(String urlToParse) {
			
			// If it starts with any of the following characters then
			if (urlToParse.StartsWith("\\") || urlToParse.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase) || urlToParse.SubStringAdv(1,1) == ":")
				return urlToParse;
			else if (HttpContext.Current != null)
				return HttpContext.Current.Server.MapPath(urlToParse);
			
			return urlToParse;
		}
	}
}
