using Lski.Toolbox.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Lski.Toolbox.Txt
{
    /// <summary>
    /// Extension methods to help working with Uri&apos;s and strings containing Uris
    /// </summary>
    public static class UriHelper
    {
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

        /// <summary>
        /// Creates a new Uri with the name/value pair both encoded and added to the end of the Uri. Therefore is non-destructive.
        /// </summary>
        /// <param name="uri">The original Uri to add the value too</param>
        /// <param name="name">The name of the parameter to add</param>
        /// <param name="value">The value of the parameter to add, if null neither the name/value pair are NOT added.</param>
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

            // Need to strip the '?' if one exists as it will double up in the UrlBuilder
            qry = (qry.Length > 0 ? qry.Substring(1) + "&" : qry) + WebUtility.UrlEncode(name) + "=" + WebUtility.UrlEncode($"{value}");

            var newUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath)
            {
                Query = qry,
                Fragment = uri.Fragment
            };

            return newUri.Uri;
        }

        /// <summary>
        /// Creates a new string with the name/value pair both encoded and added to the end of the Uri. This method is pure and non-destructive.
        /// </summary>
        /// <param name="uri">The original Uri to add the value too</param>
        /// <param name="name">The name of the parameter to add</param>
        /// <param name="value">The value of the parameter to add, if null neither the name/value pair are NOT added.</param>
        /// <param name="checkExistingParam">Used for performance and chaining. By default checks the string to see it has &apos;?&apos; and therefore change the seperator to &apos;&amp;&apos;. If false simply uses an &apos;&amp;&apos; so if chaining multiple AddParams set this to false</param>
        /// <param name="handleHash">Used for performance. By default will not attempt to handle hashes, if true will strip the hash and append it to the new uri</param>
        /// <remarks>
        /// - Uses String.Format to convert the value to a string before encoding the name and value for use in a Url.
        /// - If value is null then it does not added the name to the string and returns the uri unchanged
        /// </remarks>
        public static string AddQueryParameter(this string uri, string name, object value, bool checkExistingParam = true, bool handleHash = false)
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

            var separator = checkExistingParam ? QueryStringSeparator(uri) : '&';

            string returnUri;
            string hash;

            if (!handleHash)
            {
                hash = "";
                returnUri = uri;
            }
            else
            {
                var hashIndex = uri.IndexOf('#');

                if (hashIndex > -1)
                {
                    hash = uri.Substring(hashIndex);
                    returnUri = uri.Substring(0, hashIndex);
                }
                else
                {
                    hash = "";
                    returnUri = uri;
                }
            }

            return returnUri + separator + WebUtility.UrlEncode(name) + "=" + WebUtility.UrlEncode($"{value}") + hash;
        }

        /// <summary>
        /// Appends a query string name/value encoded to the end of a StringBuilder object. StringBuilder methods are impure so by design this is destructive.
        /// </summary>
        /// <param name="uri">The original Uri to add the value too</param>
        /// <param name="name">The name of the parameter to add</param>
        /// <param name="value">The value of the parameter to add, if null neither the name/value pair are NOT added.</param>
        /// <param name="checkExistingParam">Used for performance and chaining. By default checks the string to see it has &apos;?&apos; and therefore change the seperator to &apos;&amp;&apos;. If false simply uses an &apos;&amp;&apos; so if chaining multiple AddParams set this to false</param>
        /// <param name="handleHash">Used for performance. By default will not attempt to handle hashes, if true will strip the hash and append it to the new uri</param>
        /// <remarks>
        /// - Uses String.Format to convert the value to a string before encoding the name and value for use in a Url.
        /// - If value is null then it does not added the name to the string and returns the uri unchanged
        /// </remarks>
        public static StringBuilder AddQueryParameter(this StringBuilder uri, string name, object value, bool checkExistingParam = true, bool handleHash = false)
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

            var separator = checkExistingParam ? QueryStringSeparator(uri.AsEnumerable()) : '&';

            StringBuilder returnUri;
            string hash;

            if (!handleHash)
            {
                hash = "";
                returnUri = uri;
            }
            else
            {
                var hashIndex = uri.AsEnumerable().IndexOf('#');

                if (hashIndex > -1)
                {
                    hash = uri.ToString(hashIndex, uri.Length - hashIndex);
                    returnUri = uri.Remove(hashIndex, uri.Length - hashIndex);
                }
                else
                {
                    hash = "";
                    returnUri = uri;
                }
            }

            return returnUri
                .Append(separator)
                .Append(WebUtility.UrlEncode(name))
                .Append('=')
                .Append(WebUtility.UrlEncode($"{value}"))
                .Append(hash);
        }

        internal static char QueryStringSeparator(IEnumerable<char> chars) => chars.Any(c => c.Equals('?')) ? '&' : '?';
    }
}