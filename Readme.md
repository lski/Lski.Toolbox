Lski.Toolbox
============

A simple C# toolbox repository containing a few basic useful functions. Highlights include:

- Basic string helpers
  - ConcatWs - Linking non-empty list items with a separator, discarding the rest
  - SubStringSafe - A safe sub string, where it handles being out of bounds gracefully
  - FixedLength - Ensures a string is a fixed length, eiter truncating or padding as needed
  - Truncate - A safe truncate return null if null passed in
  - Trim - A safe trim, returning null if null passed in
- StringBuilder AsEnumerable() extension method for enabling linq on chars in StringBuilder
- Generic Observable class used for subscribing to changes on variables
- Functions for handling Generics
- Functions for handling Enum types, including:
  - Description attribute, allowing a human readable label for Enums accessible by a cached extension method.
  - Has and Is functions for flag based enumerations
- DateTime extensions:
  - Functions for working with Unix times
  - Returning getting date of first day of week
- MinOrDefault and MaxOrDefault linq extensions
- IList extensions for moving items

Plus others...

## Build

```
cd ./src
dotnet build
```

## Test

```
cd ./src
dotnet test
```

## Publish

```
PACKAGE_VERSION=""
NUGET_KEY=""

cd ./src
dotnet test
dotnet pack -c Release -o ../nuget
dotnet nuget push -s https://api.nuget.org/v3/index.json "../nuget/Lski.Toolbox.$PACKAGE_VERSION.nupkg" -k "$NUGET_KEY"
```

_NB: Remember to add variables above._

## Upgrading 4 to 5

- Encryption functions have been extracted to [Lski.Encryption](https://github.com/lski/Lski.Encryption), although not like for like as a couple of less secure items have been removed, the main functionality is there.
- RandomString has been extracted to [Lski.RandomString](https://github.com/lski/Lski.RandomString).

### Roadmap

To ensure this project will work in an ASP.Net Core environment whilst trying to maintain the current API.
