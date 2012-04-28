using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;

namespace Lski.Web.Mvc.Bundles {

	public class CssBundle : Bundle {

		private RequestContext context;

		public CssBundle(RequestContext context) {
			this.context = context;
			this._originalFiles = new List<String>();
		}

		public CssBundle(RequestContext context, params string[] files) {
			this.context = context;
			this._originalFiles = files.ToList();
		}

		protected override String CreateTag(string file) {

			TagBuilder tb = new TagBuilder("link");
			tb.Attributes.Add("type", "text/css");
			tb.Attributes.Add("href", file);
			tb.Attributes.Add("rel", "Stylesheet");

			return tb.ToString(TagRenderMode.SelfClosing);
		}

		protected override string ParseFilename(string file) {
			return new UrlHelper(context).Content(file);
		}
	}
}
