using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Server;

public static partial class UserDefinedFunctions
{
	public static readonly RegexOptions Options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.IgnoreCase;

	/// <summary>
	/// Performs a regular expression comparison on the input passed, using the regular expression passed. Returns 0 on no match and 1 on success. The settings of the regular
	/// expression being used include: Singleline, IgnoreCase and IgnorePatternWhitespace
	/// </summary>
	/// <param name="input"></param>
	/// <param name="pattern"></param>
	/// <returns></returns>
	[SqlFunctionAttribute]
	public static SqlBoolean RegexMatch(SqlChars input, SqlString pattern)
	{
		Regex regex = new Regex(pattern.Value, Options);
		return regex.IsMatch(new string(input.Value));
	}

	/// <summary>
	/// Slightly advanced length function, where it also handles null by returning 0, rather than null (avoiding additional checks) and also trims the input string, prior to
	/// giving the length back
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	[Microsoft.SqlServer.Server.SqlFunction()]
	public static SqlInt64 Length(SqlChars input) {
		return new SqlInt64(input.IsNull ? 0 : input.Value.ToString().Trim().Length);
	}

	/// <summary>
	/// Same as ToTitleCasePreserveCaps, except by default converts all words, e.g. is equiv to ToTitleCasePreserveCaps(input, false)
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
    [SqlFunction()]
    public static SqlString ToTitleCase(SqlChars input) {
        return ToTitleCasePreserveCaps(input, false);
    }

	/// <summary>
	/// Converts the passed string into a title case verson, preserveAllCaps being true means that if a word is all in capitals then it will remain all in captials. However
	/// if preserveAllCaps is false then all words are converted.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="preserveAllCaps"></param>
	/// <returns></returns>
    [SqlFunction()]
    public static SqlString ToTitleCasePreserveCaps(SqlChars input, SqlBoolean preserveAllCaps) {

        try {
			return new SqlString(new CultureInfo("en-gb", false).TextInfo.ToTitleCase(preserveAllCaps.IsFalse ? new string(input.Value).ToLower() : new string(input.Value)));
	    }
	    catch (Exception) {
	        return null;
	    }
        
    }

	/// <summary>
	/// Accepts two strings, if there is something is string one AND soemthing in string two it adds a separator between the two.
	/// </summary>
	/// <param name="separator"></param>
	/// <param name="strOne"></param>
	/// <param name="strTwo"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlString ConcatWS(SqlString separator, SqlString strOne, SqlString strTwo) {

		if (strOne.IsNull || strOne.Value.Length == 0)
			return strTwo;
		else if (strTwo.IsNull || strTwo.Value.Length == 0)
			return strOne;
		else
			return new SqlString(strOne.Value + separator.Value + strTwo.Value);
	}

	/// <summary>
	/// Checks whether the value passed is either null, or after right trimming its length is zero
	/// </summary>
	/// <param name="value"></param>
	/// <param name="altValue"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlBoolean IsNullOrEmpty(SqlChars value) {

		if (value.IsNull || value.ToSqlString().Value.Trim().Length == 0) {
			return new SqlBoolean(true);
		}

		return new SqlBoolean(false);
	}

	/// <summary>
	/// Works like isnull, in that if the value passed is null then use the alternate value. Except this method also trims the value and then checks if its length is zero.
	/// If the length after trim is zero it will also run the alternate value
	/// </summary>
	/// <param name="value"></param>
	/// <param name="altValue"></param>
	/// <returns></returns>
	[SqlFunctionAttribute]
	public static SqlChars IfNullOrEmpty(SqlChars value, SqlChars altValue) {

		if (value.IsNull || value.ToSqlString().Value.Trim().Length == 0) {
			return altValue;
		}

		return value;
	}

	/// <summary>
	/// As SqlServer does not have a builtin function for converting a DateTime to a string using the pattern passed. If the date passed is null then null is returned, if pattern
	/// is null then a simple DateTime.toString() call is used. The pattern matches the style used by the DateTime class in .Net
	/// </summary>
	/// <param name="dat"></param>
	/// <param name="pattern"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlString DateTimeToString(SqlDateTime dat, SqlString pattern) {

		if (dat.IsNull)
			return null;
		else if (pattern.IsNull)
			return dat.ToString();
		else
			return new SqlString(dat.Value.ToString(pattern.Value));
	}

	/// <summary>
	/// Receives a datetime object and returns a new DateTime object, where the time section has been reduced to midnight. Not really needed in Sql2008+ as there is the
	/// date type, but useful in Sql2005
	/// </summary>
	/// <param name="dat"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime DateOnly(SqlDateTime dat) {
		if (dat.IsNull)
			return SqlDateTime.Null;
		else
			return new SqlDateTime(dat.Value.Year, dat.Value.Month, dat.Value.Day);
	}

	/// <summary>
	/// Receives a datetime object and returns a new DateTime object, where the date section has been reduced to a base date (1900/01/01 rather than 1753/01/01 because the user 
	/// might be working with smalldatetime instead of datetime).
	/// </summary>
	/// <param name="dat"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime TimeOnly(SqlDateTime dat) {
		if (dat.IsNull)
			return SqlDateTime.Null;
		else
			return new SqlDateTime(dat.Value.Hour, dat.Value.Minute, dat.Value.Second);
	}

	/// <summary>
	/// Compares just the date (not the time) part of the first date against the second. Works in the same way as .net DateTime.CompareTo in that if dateOne is equal to
	/// dateTwo then 0 is returned, if dateOne is greater than dateTwo then 1 is returned, if dateOne is less than dateTwo then -1 is returned. Note: Null is handled by considering
	/// it as equal to the lowest possible value of a date, so that a result is still returned.
	/// </summary>
	/// <param name="dateOne"></param>
	/// <param name="dateTwo"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlInt32 CompareDateOnly(SqlDateTime dateOne, SqlDateTime dateTwo) {

		if (dateOne.IsNull && dateTwo.IsNull)
			return new SqlInt32(0);
		else if (dateOne.IsNull)
			return new SqlInt32(-1);
		else if(dateTwo.IsNull)
			return new SqlInt32(1);

		return new SqlInt32(new DateTime(dateOne.Value.Year, dateOne.Value.Month, dateOne.Value.Day).CompareTo(new DateTime(dateTwo.Value.Year, dateTwo.Value.Month, dateTwo.Value.Day)));
	}

	/// <summary>
	/// Compares just the time (not the date) part of the first time against the second. Works in the same way as .net DateTime.CompareTo in that if timeOne is equal to
	/// timeTwo then 0 is returned, if timeOne is greater than timeTwo then 1 is returned, if timeOne is less than timeTwo then -1 is returned. Note: Null is handled by considering
	/// it as equal to the lowest possible value of a date, so that a result is still returned.
	/// </summary>
	[SqlFunction()]
	public static SqlInt32 CompareTimeOnly(SqlDateTime timeOne, SqlDateTime timeTwo) {

		if (timeOne.IsNull && timeTwo.IsNull)
			return new SqlInt32(0);
		else if (timeOne.IsNull)
			return new SqlInt32(-1);
		else if (timeTwo.IsNull)
			return new SqlInt32(1);

		return new SqlInt32(new DateTime(1, 1, 1, timeOne.Value.Hour, timeOne.Value.Minute, timeOne.Value.Second).CompareTo(new DateTime(1, 1, 1, timeOne.Value.Hour, timeOne.Value.Minute, timeOne.Value.Second)));
	}

	/// <summary>
	/// Creates a new date, using the values passed, if any are null the value for that field is the lowest value IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="date"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime Date(SqlInt32 year, SqlInt32 month, SqlInt32 date) {
		return new SqlDateTime((year.IsNull ? 1900 : year.Value), (month.IsNull ? 1 : month.Value), (date.IsNull ? 1 : date.Value));
	}

	/// <summary>
	/// Creates a new date, using the values passed, if any are null the value for that field is the lowest value IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="date"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime Time(SqlInt32 hour, SqlInt32 minute, SqlInt32 second) {
		return new SqlDateTime(1900, 1, 1, (hour.IsNull ? 0 : hour.Value), (minute.IsNull ? 1 : minute.Value), (second.IsNull ? 1 : second.Value));
	}

	/// <summary>
	/// Creates a new date, using the values passed, containing just the time with the date at its default base values. Can take null values, if null is passed the lowest default 
	/// value for that field is used as they are for the values not in the parameters. IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="date"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime ShortTime(SqlInt32 hour, SqlInt32 minute) {
		return new SqlDateTime(1900, 1, 1, (hour.IsNull ? 0 : hour.Value), (minute.IsNull ? 1 : minute.Value), 0);
	}

	/// <summary>
	/// Creates a new date, using the values passed if any are null the value for that field is its lowest value. IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="day"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="second"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlDateTime DateTime(SqlInt32 year, SqlInt32 month, SqlInt32 day, SqlInt32 hour, SqlInt32 minute, SqlInt32 second) {
		return new SqlDateTime((year.IsNull ? 1900 : year.Value), (month.IsNull ? 1 : month.Value), (day.IsNull ? 1 : day.Value), (hour.IsNull ? 0 : hour.Value), (minute.IsNull ? 0 : minute.Value), (second.IsNull ? 0 : second.Value));
	}

	/// <summary>
	/// Return true or false depending on whether this is a valid time or not. Returns false on NULL
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	[SqlFunction()]
	public static SqlBoolean IsTime(SqlChars time) {

		if (time.IsNull)
			return false;

		if ((new Regex("^(([0-1]{0,1}[0-9])|([2]{0,1}[0-3])):[0-5][0-9]$")).IsMatch(time.ToSqlString().Value))
		 	return new SqlBoolean(true);
		else
			return new SqlBoolean(false);
	}

	/// <summary>
	/// Accepts the condition, it calculates prior to calling the function, and returns the true value if true or the false value if not, do not use where the true or false action is dynamic as both will run
	/// making this function inefficent, use case instead
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="ifTrue"></param>
	/// <param name="ifFalse"></param>
	/// <returns></returns>
	[Obsolete("Due to inefficient nature use CASE instead")]
	[Microsoft.SqlServer.Server.SqlFunction]
	public static SqlChars IIF(SqlBoolean condition, SqlChars ifTrue, SqlChars ifFalse) {
		return (condition.Value ? ifTrue : ifFalse);
	}

	//[SqlFunction()]
	//public static SqlDateTime MaxDate(SqlDateTime dateOne, SqlDateTime dateTwo, SqlDateTime dateThree, SqlDateTime dateFour, SqlDateTime dateFive, SqlDateTime dateSix, SqlDateTime dateSeven) {

	//    var l = new List<SqlDateTime>();
	//    l.Add(dateOne);
	//    l.Add(dateTwo);
	//    l.Add(dateThree);
	//    l.Add(dateFour);
	//    l.Add(dateFive);
	//    l.Add(dateSix);
	//    l.Add(dateSeven);

	//    return l.Max();
	//}

}