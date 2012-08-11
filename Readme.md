Lski
====

A simple C# toolbox repository holding together useful functions. I have split the solutions into different projects as it removes the need to include other libraries unnecessarily e.g. There is a ASP.Net project and a WinForms project which would never be used together.

Most of the projects use the Lski project as its core and expand upon it. The core includes some extension methods for strings and DateTimes and also a couple of simple classes for handling raw connections.

SqlFunctions
------------

This project is not dependant on the core and is designed to add some simple CLR function to SQL Server, including Regular Expressions and some DateTime functions.
