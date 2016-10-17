using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader.Test
{
    using Xunit;
    public class TestDictionaryDataReader
    {
        [Fact]
        public void TestDictionary()
        {
            var dics = Enumerable.Range(0, 100)
                .Select(i =>
                {
                    return new Dictionary<string, object>()
                    {
                        {"a",i },
                        {"b",$"hogehoge{i}" },
                        {"c",DateTime.MinValue.AddDays(i) },
                        {"d",i * 1.1 },
                        {"e",i * (decimal)1 }
                    };
                }).ToArray();
            using (var dr = dics.AsDataReaderFromDictionary())
            {
                int i = 0;
                while (dr.Read())
                {
                    Assert.Equal(5, dr.FieldCount);
                    Assert.Equal(i, dr.GetInt32(dr.GetOrdinal("a")));
                    Assert.Equal($"hogehoge{i}", dr.GetString(dr.GetOrdinal("b")));
                    Assert.Equal(DateTime.MinValue.AddDays(i), dr.GetDateTime(dr.GetOrdinal("c")));
                    Assert.Equal(i * 1.1, dr.GetDouble(dr.GetOrdinal("d")));
                    Assert.Equal(i * (decimal)1, dr.GetDecimal(dr.GetOrdinal("e")));
                    i += 1;
                }
            }
        }
    }
}
