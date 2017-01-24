using System;
using System.Globalization;

namespace Lski.Toolbox.Dates {

    public static class DateTimeExt {

        private const string IsoDateTimeFormat = "{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}Z";
        private const String IsTime = @"^(20|21|22|23|[01]\d|\d)[:](([0-5]\d){1,2})$";
        private const String EndsWithTime = "^" + IsTime;

        /// <summary>
        /// A DateTime that represents Epoch
        /// </summary>
        public static DateTime Epoch { get; }

        static DateTimeExt() {
            Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a date into the number of seconds from Epoch. This is done using te UTC version of the time.
        /// </summary>
        /// <param name="date"></param>
        public static long ToUnixTime(this DateTime date) {

            return (long)((date.ToUniversalTime() - Epoch).TotalSeconds);
        }

        /// <summary>
        /// Converts a number into a dateTime where the number is the number of seconds since the Epoch.
        ///
        /// Returns a DateTime UTC object, to get a local version call FromUnixTime().ToLocalTime() on the result.
        /// </summary>
        /// <param name="secondsSinceEpoch"></param>
        public static DateTime FromUnixTime(this int secondsSinceEpoch) => Epoch.AddSeconds(secondsSinceEpoch);

        /// <summary>
        /// Converts a number into a dateTime where the number is the number of seconds since the Epoch.
        ///
        /// Returns a DateTime UTC object, to get a local version call FromUnixTime().ToLocalTime() on the result.
        /// </summary>
        /// <param name="secondsSinceEpoch"></param>
        public static DateTime FromUnixTime(this long secondsSinceEpoch) => Epoch.AddSeconds(secondsSinceEpoch);

        /// <summary>
        /// Convert a DateTime to a basic ISO8601 date string, does not include offset or milliseconds.
        /// </summary>
        /// <param name="date"></param>
        public static string ToIso8601(this DateTime date) => string.Format(IsoDateTimeFormat, date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);

        /// <summary>
        /// Convert a string in ISO8601 date format to a date time
        /// </summary>
        /// <param name="date"></param>
        public static DateTime FromIso8601(this string date) => DateTime.ParseExact(date.Replace("T", " "), "u", CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns new a date, comprising the first day of the week of the date passed, according to the current culture.
        /// </summary>
        public static DateTime FirstDayOfWeek(this DateTime dat, DayOfWeek firstDayOfWeek = DayOfWeek.Monday) {

            dat = dat.Date;

            int difference = (int)dat.DayOfWeek - (int)firstDayOfWeek;

            if (difference < 0) {
                difference = 7 + difference;
            }

            return dat.AddDays(-difference);
        }

        /// <summary>
        /// Useful for combining separate dates and separate times together in one datetime object, in a constructive manner. The new object is returned
        /// </summary>
        /// <param name="date">The DateTime object containing the date section desired</param>
        /// <param name="time">The DateTime object containing the time section desired</param>
        /// <param name="includeSeconds">Optional: States whether the new DateTime should include the seconds from the time object or reset to zero</param>
        [Obsolete("Will be removed in the next major release")]
        public static DateTime CombineWithTime(this DateTime date, DateTime time, Boolean includeSeconds = false) {

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, (includeSeconds ? time.Second : 0));
        }
    }
}