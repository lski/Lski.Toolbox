using System.Net;

namespace Lski.Toolbox.Net
{
    /// <summary>
    /// Extenstion methods to the HttpStatusCode enum
    /// </summary>
    public static class HttpStatusCodeExtensions
    {
        /// <summary>
        /// States whether the status code is between 200 and 299 and can be considered a 'success'
        /// </summary>
        public static bool IsSuccess(this HttpStatusCode code)
        {
            var num = (int)code;

            return num >= 200 && num < 300;
        }
    }
}