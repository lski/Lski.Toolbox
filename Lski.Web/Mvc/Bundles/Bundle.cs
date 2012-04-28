using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lski.Web.Mvc.Bundles {

	public abstract class Bundle {

		protected List<string> _originalFiles;

		public Bundle Add(String file) {
			
			_originalFiles.Add(file);
			return this;
		}

		public Bundle Add(params String[] files) {
			
			_originalFiles.AddRange(files);
			return this;
		}

		public IEnumerable<string> OriginalFiles {
			get {
				return _originalFiles;
			}
		}

		public MvcHtmlString Render(String minifiedFile) {

#if DEBUG
			return new MvcHtmlString(String.Join(Environment.NewLine, _originalFiles.Select(x => CreateTag(ParseFilename(x)))));
#else
			return new MvcHtmlString(String.IsNullOrEmpty(minifiedFile) ? "" : CreateTag(ParseFilename(minifiedFile)));
#endif
		}

		protected abstract String CreateTag(string file);
		protected abstract string ParseFilename(string file);
	}
}
