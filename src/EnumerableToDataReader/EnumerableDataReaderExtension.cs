using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    using System.Data;
    using System.Collections;
    public static class EnumerableDataReaderExtension
    {
        public static IDataReader ToDataReader<T>(this IEnumerable<T> list)
        {
            return new TypedEnumerableDataReader<T>(list);
        }
        public static IDataReader ToDataReader(this IEnumerable list, Type t)
        {
            return new EnumerableDataReader(t, list);
        }
        public static IDataReader ToDictionaryDataReader(this IEnumerable<IDictionary<string, object>> list)
        {
            return new DictionaryDataReader(list);
        }
    }
}
