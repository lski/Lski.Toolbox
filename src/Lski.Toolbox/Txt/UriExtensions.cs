using System;
using System.Net;

namespace Lski.Toolbox.Txt
{
    /// <summary>
    /// Extension methods to help working with Uris, currently just adds query string params
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Creates a new Uri with the name/value pair both encoded and added to the end of the Uri. Therefore is non-destructive.
        /// </summary>
        /// <param name="uri">The original Uri to add the value too</param>
        /// <param name="name">The name of the parameter to add</param>
        /// <param name="value">The value of the parameter to add, if null neither the name/value pair are NOT added.</param>
        /// <returns>The new Uri object</returns>
        /// <remarks>
        /// - Uses String.Format to convert the value to a string before encoding the name and value for use in a Url.
        /// - If value is null then it does not added the name to the string and returns the uri unchanged
        /// </remarks>
        public static Uri AddParameter(this Uri uri, string name, object value)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                return uri;
            }

            var qry = uri.Query;
            qry = qry.Length > 0 ? qry.Substring(1) + "&" : qry;
            qry += WebUtility.UrlEncode(name) + "=" + WebUtility.UrlEncode($"{value}");

            var newUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath)
            {
                Query = qry,
                Fragment = uri.Fragment
            };

            return newUri.Uri;
        }

        /// <summary>
        /// Creates a new Uri with the name/value pair both encoded and added to the end of the Uri. This method is non-destructive.
        /// </summary>
        /// <param name="uri">The original Uri to add the value too</param>
        /// <param name="name">The name of the parameter to add</param>
        /// <param name="value">The value of the parameter to add, if null neither the name/value pair are NOT added.</param>
        /// <returns>A string of the full Uri created</returns>
        /// <remarks>
        /// - Uses String.Format to convert the value to a string before encoding the name and value for use in a Url.
        /// - If value is null then it does not added the name to the string and returns the uri unchanged
        /// </remarks>
        public static string AddParameter(string uri, string name, object value)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return AddParameter(new Uri(uri), name, value).ToString();
        }

		/// <summary>
        /// Creates a new Uri with the only the scheme changed. Best used with the static scheme properties in Uri e.g. Uri.UriSchemeHttps. This method is non-destructive.
        /// </summary>
		/// <remarks>
		/// Adapted from code on Stackoverflow <see>http://stackoverflow.com/questions/17968426/changing-the-scheme-of-system-uri</see>
		/// </remarks>
        public static Uri ChangeScheme(this Uri uri, string scheme = "https")
        {
			var newUri = new UriBuilder(uri);

			var hadDefaultPort = uri.IsDefaultPort;
			newUri.Scheme = scheme;
			newUri.Port = hadDefaultPort ? -1 : uri.Port;

			return newUri.Uri;
        }
    }
}