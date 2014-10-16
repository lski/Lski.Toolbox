using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
		/// Returns a date, comprising the first day of the week of the date passed, according to the current culture. The process is non-destructive 
		/// as it returns the result as a new date.
		/// </summary>
		/// <param name="dat"></param>
		/// <returns></returns>
		public static DateTime FirstDayOfWeek(this DateTime dat) {

			dat = dat.Date;

			int difference = (int)dat.DayOfWeek - (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

			if (difference < 0) {
				difference = 7 + difference;
			}

			return dat.AddDays(-difference);
		}



		/// <summary>
		/// Converts the .net supported date format current culture format into JQuery Datepicker format.
		/// </summary>
		/// <param name="format">Date format supported by .NET.</param>
		/// <returns>Format string that supported in JQuery Datepicker.</returns>
		/// <remarks>
		/// Based on code from http://www.rajeeshcv.com/post/details/31/jqueryui-datepicker-in-asp-net-mvc
		/// 
		/// Date used in this comment : 5th - Nov - 2009 (Thursday)
		/// 
		///   .NET    JQueryUI        Output      Comment
		///   --------------------------------------------------------------
		///   d       d               5           day of month(No leading zero)
		///   dd      dd              05          day of month(two digit)
		///   ddd     D               Thu         day short name
		///   dddd    DD              Thursday    day long name
		///   M       m               11          month of year(No leading zero)
		///   MM      mm              11          month of year(two digit)
		///   MMM     M               Nov         month name short
		///   MMMM    MM              November    month name long.
		///   yy      y               09          Year(two digit)
		///   yyyy    yy              2009        Year(four digit)
		/// </remarks>
		public static string ToJQueryFormat(string format) {

			string currentFormat = format;

			// Convert the date
			currentFormat = currentFormat.Replace("dddd", "DD");
			currentFormat = currentFormat.Replace("ddd", "D");

			// Convert month
			if (currentFormat.Contains("MMMM")) {
				currentFormat = currentFormat.Replace("MMMM", "MM");
			} else if (currentFormat.Contains("MMM")) {
				currentFormat = currentFormat.Replace("MMM", "M");
			} else if (currentFormat.Contains("MM")) {
				currentFormat = currentFormat.Replace("MM", "mm");
			} else {
				currentFormat = currentFormat.Replace("M", "m");
			}

			// Convert year
			currentFormat = currentFormat.Contains("yyyy") ? currentFormat.Replace("yyyy", "yy") : currentFormat.Replace("yy", "y");

			return currentFormat;
		}

		// TODO: Change to returning list of DateTimes and excepting a TimeSpan

		/// <summary>
		/// Creates a list of times in the format 09:00 where it starts at the minimum time and finishes at the maximum time with the set size of intervals
		/// </summary>
		/// <param name="minTime">The minimum (start) time in the format 09:00</param>
		/// <param name="maxTime">The maximum (end) time in the format 09:00</param>
		/// <param name="intervals">Simple integer representing the amount of minutes between each time returned</param>
		/// <returns></returns>
		public static List<String> CreateTimeList(String minTime, String maxTime, Int32 intervals) {

			if (minTime == null || maxTime == null || intervals <= 0)
				throw new ArgumentException("When creating a useable list of times the minimum time, maximum time and the intervals all need to be set");

			var regex = new Regex(EndsWithTime);

			var minMatch = regex.Match(minTime);
			var maxMatch = regex.Match(maxTime);

			if (!minMatch.Success || minMatch.Groups.Count < 4)
				throw new ArgumentException("The format of the minimum time needs to be in the format 09:00");

			if (!maxMatch.Success || maxMatch.Groups.Count < 4)
				throw new ArgumentException("The format of the minimum time needs to be in the format 09:00");

			var startTime = new DateTime(2000, 1, 1, Int32.Parse(minMatch.Groups[1].Value), Int32.Parse(minMatch.Groups[2].Value), 0);
			var endTime = new DateTime(2000, 1, 1, Int32.Parse(maxMatch.Groups[1].Value), Int32.Parse(maxMatch.Groups[2].Value), 0);

			return CreateTimeList(startTime, endTime, intervals);
		}

		/// <summary>
		/// Creates a list of times in the format 09:00 where it starts at the minimum time and finishes at the maximum time with the set size of intervals.
		/// </summary>
		/// <param name="minTime">The minimum (start) time in the format 09:00</param>
		/// <param name="maxTime">The maximum (end) time in the format 09:00</param>
		/// <param name="intervals">Simple integer representing the amount of minutes between each time returned</param>
		/// <returns></returns>
		public static List<String> CreateTimeList(DateTime minTime, DateTime maxTime, Int32 intervals) {

			var lst = new List<String>();

			while (minTime <= maxTime) {

				lst.Add(minTime.ToString("HH:mm"));
				minTime = minTime.AddMinutes(intervals);
			}

			return lst;
		}

		/// <summary>
		/// Useful for combining separate dates and separate times together in one datetime object, in a constructive manner. The new object
		/// is returned
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
