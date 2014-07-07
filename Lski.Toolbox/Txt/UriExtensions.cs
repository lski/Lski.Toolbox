namespace System {

	public static class UriExtensions {

		/// <summary>
		/// Creates a new uri string from an existing Uri object, where a parameter has been added. Non-destructive.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string AddParameter(this Uri uri, string name, object value) {

			if (uri == null) {
				throw new ArgumentNullException("uri");
			}
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			var url = uri.GetLeftPart(UriPartial.Query) + (uri.Query.Length > 0 ? "&" : "?") + String.Format("{0}={1}", name, value);

			return url;
		}

		/// <summary>
		/// Creates a new uri string from an existing Uri, where a parameter has been added. Non-destructive.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string AddParameter(string uri, string name, object value) {
			return AddParameter(new Uri(uri), name, value);
		}
	}
}