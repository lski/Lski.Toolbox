using System;

namespace Lski.Toolbox.Dates {

    public static class DateTimeExt {

        public const String IsTime = @"^(20|21|22|23|[01]\d|\d)[:](([0-5]\d){1,2})$";
        public const String EndsWithTime = "^" + IsTime;

        private static readonly DateTime _epoch;

        public static DateTime Epoch {
            get { return _epoch; }
        }

        static DateTimeExt() {
            _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a date into the number of seconds from Epoch. This is done using te UTC version of the time.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime date) {

            return (long)((date.ToUniversalTime() - Epoch).TotalSeconds);
        }

        /// <summary>
        /// Converts a number into a dateTime where the number is the number of seconds since the Epoch.
        ///
        /// Returns a DateTime UTC object, to get a local version call FromUnixTime().ToLocalTime() on the result.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this int date) {

            return Epoch.AddSeconds(date);
        }

        /// <summary>
        /// Converts a number into a dateTime where the number is the number of seconds since the Epoch.
        ///
        /// Returns a DateTime UTC object, to get a local version call FromUnixTime().ToLocalTime() on the result.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this long date) {

            return Epoch.AddSeconds(date);
        }

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
        /// <returns></returns>
        public static DateTime CombineWithTime(this DateTime date, DateTime time, Boolean includeSeconds = false) {

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, (includeSeconds ? time.Second : 0));
        }
    }
}