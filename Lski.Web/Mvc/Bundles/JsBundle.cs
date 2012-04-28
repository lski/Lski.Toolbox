using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;

namespace Lski.Web.Mvc.Bundles {

	public class JsBundle : Bundle {

		private RequestContext context;

		public JsBundle(RequestContext context) {
			this.context = context;
			this._originalFiles = new List<String>();
		}

		public JsBundle(RequestContext context, params string[] files) {
			this.context = context;
			this._originalFiles = files.ToList();
		}

		protected override String CreateTag(string file) {

			TagBuilder tb = new TagBuilder("script");
			tb.Attributes.Add("type", "text/javascript");
			tb.Attributes.Add("src", file);

			return tb.ToString();
		}

		protected override string ParseFilename(string file) {
			return new UrlHelper(context).Content(file);
		}
	}
}
