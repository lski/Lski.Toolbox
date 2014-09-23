using System.Net;

namespace System {

	/// <summary>
	/// Extension methods to help working with Uris, currently just adds query string params
	/// </summary>
	public static class UriExtensions {


		/// <summary>
		/// Creates a new Uri with the added name/value pair added to the end of the Uri.
		///
		/// This method creates a new Uri and is non-destructive and uses String.Format to covert the value to a string before encoding the name and value for use in a Url
		/// </summary>
		/// <returns></returns>
		public static Uri AddParameter(this Uri uri, string name, object value) {

			if (uri == null) {
				throw new ArgumentNullException("uri");
			}

			if (name == null) {
				throw new ArgumentNullException("name");
			}

			var param = WebUtility.UrlEncode(name) + "=" + WebUtility.UrlEncode(String.Format("{0}", value));
			var qry = uri.Query + (uri.Query.Length == 0 ? "?" : "&") + param;
			var lnk = uri.GetLeftPart(UriPartial.Path) + qry + uri.Fragment;
			
			return new Uri(lnk);
		}

		/// <summary>
		/// Creates a new Uri with the added name/value pair added to the end of the Uri.
		///
		/// This method creates a new Uri and is non-destructive and uses String.Format to covert the value to a string before encoding the name and value for use in a Url
		/// </summary>
		/// <returns></returns>
		public static string AddParameter(string uri, string name, object value) {

			return AddParameter(new Uri(uri), name, value).ToString();
		}
	}
}