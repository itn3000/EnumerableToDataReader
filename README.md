# EnumerableToDataReader

C# library for IEnumerable to DbDataReader converter

# Requirements

* NuGet 2.12 or later
* .NET Framework 4.5 or later(.NET core is also ok)

# Usage

write ```using EnumerableToDataReader``` in your .cs,you can use the following extension functions

* `AsDataReader<T>(this IEnumerable<T> list)`
* `AsDataReader(this IEnumerable list, Type t)`
* `AsDataReaderFromDictionary(this IEnumerable<IDictionary<string,object>> list)`

# Samples

see [unit tests](src/EnumerableToDataReader.Test)
