Lski.Toolbox v2
============

A simple C# toolbox repository holding together useful functions. Highlights include:

- Basic string helpers
  - ConcatWs - Linking non-empty list items with a separator, discarding the rest
  - SubStringSafe - A safe sub string, where it handles being out of bounds gracefully
  - FixedLength - Ensures a string is a fixed length, eiter truncating or padding as needed
  - Truncate - A safe truncate return null if null passed in
  - Trim - A safe trim, returning null if null passed in
- String encryption/decryption methods
- Random string generator designed to work with loops without collisions
- Generic Observable class used for subscribing to changes on variables
- Functions for handling Generics
- Functions for handling Enum types, including:
  - Description attribute, allowing a human readable label for Enums accessible by a cached extension method.
  - Has and Is functions for flag based enumerations
- DateTime extensions:
  - Functions for working with Unix times
  - Returning getting date of first day of week
  - Combining a date and a time from two DateTime objects
- MinOrDefault and MaxOrDefault linq extensions
- IList extensions for moving items

Plus others...

### Upgrading from v1 to v2

There are many break changes between version 1 and 2, mostly stripping old legacy functions, but I have also removed out two sections:

- [Lski.Toolbox.Data](https://github.com/lski/Lski.Toolbox.Data) and [Lski.Toolbox.ImageResizer](https://github.com/lski/Lski.Toolbox.ImageResizer) are now a separate library and nuget packages
- System.Linq.Dynamic has been removed as the same code is available as a separate nuget package by someone else

### Roadmap

To ensure this project will work in an ASP.Net Core environment whilst trying to maintain the current API.
