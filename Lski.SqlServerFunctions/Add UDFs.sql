---- For use with SQL2005

--ALTER DATABASE InsuranceInvoicing SET TRUSTWORTHY ON;
--GO

--CREATE ASSEMBLY [System.Core]
--AUTHORIZATION [dbo]
--FROM 'C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll'
--WITH PERMISSION_SET = UNSAFE
--GO

--------------------------

IF OBJECT_ID (N'dbo.[CompareDateOnly]', N'FS') IS NOT NULL
	Drop Function [CompareDateOnly];
GO

IF OBJECT_ID (N'dbo.[CompareTimeOnly]', N'FS') IS NOT NULL
	Drop Function [CompareTimeOnly];
GO

IF OBJECT_ID (N'dbo.[DateOnly]', N'FS') IS NOT NULL
	Drop Function [DateOnly];
GO

IF OBJECT_ID (N'dbo.[Date]', N'FS') IS NOT NULL
	Drop Function [Date];
GO


IF OBJECT_ID (N'dbo.[DateTime]', N'FS') IS NOT NULL
	Drop Function [DateTime];
GO

IF OBJECT_ID (N'dbo.[Time]', N'FS') IS NOT NULL
	Drop Function [Time];
GO

IF OBJECT_ID (N'dbo.[IsTime]', N'FS') IS NOT NULL
	Drop Function [IsTime];
GO

IF OBJECT_ID (N'dbo.[ShortTime]', N'FS') IS NOT NULL
	Drop Function [ShortTime];
GO


IF OBJECT_ID (N'dbo.[TimeOnly]', N'FS') IS NOT NULL
	Drop Function TimeOnly;
GO

IF OBJECT_ID (N'dbo.[DateTimeToString]', N'FS') IS NOT NULL
	Drop Function DateTimeToString;
GO

IF OBJECT_ID (N'dbo.[Length]', N'FS') IS NOT NULL
	Drop Function [Length];
GO

IF OBJECT_ID (N'dbo.[RegexMatch]', N'FS') IS NOT NULL
	Drop Function [RegexMatch];
GO

IF OBJECT_ID (N'dbo.[RegexReplace]', N'FS') IS NOT NULL
	Drop Function [RegexReplace];
GO

IF OBJECT_ID (N'dbo.[ConcatWS]', N'FS') IS NOT NULL
	Drop Function [ConcatWS];
GO

IF OBJECT_ID (N'dbo.[ToTitleCase]', N'FS') IS NOT NULL
	Drop Function [ToTitleCase];
GO

IF OBJECT_ID (N'dbo.[ToTitleCasePreserveCaps]', N'FS') IS NOT NULL
	Drop Function [ToTitleCasePreserveCaps];
GO


IF OBJECT_ID (N'dbo.IsNullOrEmpty', N'FS') IS NOT NULL
	Drop Function IsNullOrEmpty;
GO


IF OBJECT_ID (N'dbo.IIF', N'FS') IS NOT NULL
	Drop Function IIF;
GO

IF OBJECT_ID (N'dbo.IfNullOrEmpty', N'FS') IS NOT NULL
	Drop Function IfNullOrEmpty;	
GO

IF OBJECT_ID (N'dbo.MaxDate', N'FS') IS NOT NULL
	Drop Function MaxDate;	
GO

IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'SqlServerFunctions')
DROP ASSEMBLY SqlServerFunctions;

GO

-- Start Adding the assembly and functions

/****** Object:  SqlAssembly [SqlServerFunctions]    Script Date: 11/17/2010 14:28:29 ******/
CREATE ASSEMBLY [SqlServerFunctions]
AUTHORIZATION [dbo]
FROM '** CHANGE TO LOCATION OF DLL **'
WITH PERMISSION_SET = SAFE
GO


create  function [dbo].[DateTimeToString](@date DateTime, @pattern nvarchar(max))
-- As SqlServer does not have a builtin function for converting a DateTime to a string using the pattern passed. If the date passed is null then null is returned, if pattern
-- is null then a simple DateTime.toString() call is used. The pattern matches the style used by the DateTime class in .Net
returns nvarchar(max)
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[DateTimeToString];
GO

create  function [dbo].[CompareDateOnly](@dateOne DateTime, @dateTwo DateTime)
-- Compares just the date (not the time) part of the first date against the second. Works in the same way as .net DateTime.CompareTo in that if dateOne is equal to
-- dateTwo then 0 is returned, if dateOne is greater than dateTwo then 1 is returned, if dateOne is less than dateTwo then -1 is returned. Note: Null is handled by considering
-- it as equal to the lowest possible value of a date, so that a result is still returned.
returns int
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[CompareDateOnly]
GO

create  function [dbo].[CompareTimeOnly](@timeOne DateTime, @timeTwo DateTime)
-- Compares just the time (not the date) part of the first time against the second. Works in the same way as .net DateTime.CompareTo in that if timeOne is equal to
-- timeTwo then 0 is returned, if timeOne is greater than timeTwo then 1 is returned, if timeOne is less than timeTwo then -1 is returned. Note: Null is handled by considering
-- it as equal to the lowest possible value of a date, so that a result is still returned.
returns int
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[CompareTimeOnly]
GO

create  function [dbo].[DateOnly](@DateTime DateTime)
-- Receives a datetime object and returns a new DateTime object, where the time section has been reduced to midnight (00:00:00). Not really needed in Sql2008+ as there is the
-- date type, but useful in Sql2005
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[DateOnly]
GO

create function [dbo].[Date](@Year int, @Month int, @Day int)
-- Creates a new date, using the values passed if any are null the value for that field is its lowest value, with the time section being set to midnight 
-- IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[Date]
GO


create function [dbo].[DateTime](@Year int, @Month int, @Day int, @Hour int, @Minute int, @Second int)
-- Creates a new date, using the values passed if any are null the value for that field is its lowest value. IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[DateTime]
GO

CREATE function [dbo].[ShortTime](@Hour int, @Minute int)
-- Creates a new date, using the values passed, containing just the time with the date at its default base values. Can take null values, if null is passed the lowest default 
-- value for that field is used as they are for the values not in the parameters. IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[ShortTime]
GO

CREATE function [dbo].[Time](@Hour int, @Minute int, @Second int)
-- Creates a new date, using the values passed, containing just the time with the date at its default base values. Can take null values, if null is passed the lowest default 
-- value for that field is used as they are for the values not in the parameters. IE year = 1900, month = 1, day = 1, hour = 0, minute = 0, second = 0
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[Time]
GO

create function [dbo].[TimeOnly](@DateTime DateTime)
-- Receives a datetime object and returns a new DateTime object, where the date section has been reduced to a base date (1900/01/01 rather than 1753/01/01 because the user 
-- might be working with smalldatetime instead of datetime).
returns datetime
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[TimeOnly]
GO

CREATE FUNCTION [dbo].[Length](@input [nvarchar](max))
-- Slightly advanced length function, where it also handles null by returning 0, rather than null (avoiding additional checks) and also trims the input string, prior to
-- giving the length back
RETURNS [bigint] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[Length]
GO

CREATE FUNCTION [dbo].[RegexReplace](@input [nvarchar](max), @pattern [nvarchar](4000), @replacement [nvarchar](4000))
-- Performs a regular expression comparison on the input passed, using the regular expression passed. Returns 0 on no match and 1 on success. The settings of the regular
-- expression being used include: Singleline, IgnoreCase and IgnorePatternWhitespace
RETURNS nvarchar(max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[RegexReplace]
GO

CREATE FUNCTION [dbo].[RegexMatch](@input [nvarchar](max), @pattern [nvarchar](4000))
-- Performs a regular expression comparison on the input passed, using the regular expression passed. Returns 0 on no match and 1 on success. The settings of the regular
-- expression being used include: Singleline, IgnoreCase and IgnorePatternWhitespace
RETURNS [bit] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[RegexMatch]
GO

CREATE FUNCTION [dbo].[IsTime](@input [nvarchar](max))
RETURNS [bit] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[IsTime]
GO


CREATE FUNCTION [dbo].[ConcatWS](@separator [nvarchar](max), @strOne [nvarchar](max), @strTwo [nvarchar](max))
-- Accepts two strings, if there is something is string one AND soemthing in string two it adds a separator between the two.
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[ConcatWS]
GO


CREATE FUNCTION [dbo].[ToTitleCase](@input [nvarchar](max))
-- Same as ToTitleCasePreserveCaps, except by default converts all words, e.g. is equiv to ToTitleCasePreserveCaps(input, false)
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[ToTitleCase]
GO

CREATE FUNCTION [dbo].[ToTitleCasePreserveCaps](@input [nvarchar](max), @preserveAllCaps [bit])
-- Converts the passed string into a title case verson, preserveAllCaps being true means that if a word is all in capitals then it will remain all in captials. However
-- if preserveAllCaps is false then all words are converted.
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].[ToTitleCasePreserveCaps]
GO

CREATE FUNCTION [dbo].IsNullOrEmpty(@value [nvarchar](max))
-- Checks whether the value passed is either null, or after right trimming its length is zero
RETURNS bit WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].IsNullOrEmpty
GO

CREATE FUNCTION [dbo].IIF(@condition bit, @ifTrue [nvarchar](max), @ifFalse [nvarchar](max))
-- Accepts the condition, it calculates prior to calling the function, and returns the true value if true or the false value if not
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].IIF
GO

CREATE FUNCTION [dbo].IfNullOrEmpty(@value [nvarchar](max), @altValue [nvarchar](max))
-- Works like isnull, in that if the value passed is null then use the alternate value. Except this method also trims the value and then checks if its length is zero.
-- If the length after trim is zero it will also run the alternate value
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].IfNullOrEmpty
GO

/**
CREATE FUNCTION [dbo].MaxDate(@dateOne DateTime, @dateTwo DateTime, @dateThree DateTime, @dateFour DateTime, @dateFive DateTime, @dateSix DateTime, @dateSeven DateTime)
-- Returns the maximum date out of the values passed
RETURNS DateTime WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlServerFunctions].[UserDefinedFunctions].MaxDate
GO


Currently no used and should be changed to a CLR function

IF OBJECT_ID (N'dbo.[ConvertStringToTime]', N'FN') IS NOT NULL
	Drop Function ConvertStringToTime;	
GO

CREATE FUNCTION [dbo].[ConvertStringToTime]
(
	@timeAsString varchar(15)
)
RETURNS time
AS
BEGIN

	declare @returnString varchar(15);
	set @timeAsString = ltrim(@timeAsString);
	
	declare @hh as char(2);
	declare @mm as char(2);
	
	if(len(@timeAsString) = 5)
		begin	
			set @hh = left(@timeAsString, 2);
			set @mm = right(@timeAsString, 2);
		
			if((patindex('%[^0-9]%', @hh) > 0) OR patindex('%[^0-9]%', @mm) > 0 or cast(@hh as tinyint) > 23 or cast(@mm as tinyint) > 59)
				set @returnString = null;
			else 
				set @returnString = @hh + ':' + @mm;
		end
	else if(len(@timeAsString) = 8)
		begin
			declare @ss as char(2);
			set @hh = left(@timeAsString, 2);
			set @mm = substring(@timeAsString, 4, 2);
			set @ss = right(@timeAsString, 2);
		
			if((patindex('%[^0-9]%', @hh) > 0) OR patindex('%[^0-9]%', @mm) > 0 or patindex('%[^0-9]%', @ss) > 0 or cast(@hh as tinyint) > 23 or cast(@mm as tinyint) > 59 or cast(@ss as tinyint) > 59)
				set @returnString = null;
			else 
				set @returnString = @hh + ':' + @mm + ':' + @ss;
		end
	else
		set @returnString = null;

	return cast(@returnString as time);
	
END
*/