using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Lski.Web.Mvc.Bundles;

namespace Lski.Web.Mvc.Extensions {
	
	public static class MvcHelpers {

		public static Bundle Js(this HtmlHelper helper, params string[] files) {
			return new JsBundle(helper.ViewContext.RequestContext, files);
		}

		public static Bundle Css(this HtmlHelper helper, params string[] files) {
			return new CssBundle(helper.ViewContext.RequestContext, files);
		}
	}
}
