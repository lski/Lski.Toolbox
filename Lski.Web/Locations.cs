using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lski.Web {

	public class Locations {

		private static string _CachedRoot;
		public static String Root {
			get {
				return _CachedRoot ?? (_CachedRoot = HttpContext.Current.Request.ApplicationPath);
			}
		}

		private static string _CachedWebUrl;
		public static String WebUrl {
			get {

				if(_CachedWebUrl == null) {

					var c = HttpContext.Current;
					_CachedWebUrl = c.Request.Url.Scheme + "://" + c.Request.Url.Host + (c.Request.Url.IsDefaultPort ? "" : ":" + c.Request.Url.Port) + "/";
				}

				return _CachedWebUrl;
			}
		}
	}
}