using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader.Test
{
    using Xunit;
    using Xunit.Abstractions;
    using System.Reflection;
    public class TestEnumerableDataReader
    {
        ITestOutputHelper m_Outputter;
        public TestEnumerableDataReader(ITestOutputHelper outputter)
        {
            m_Outputter = outputter;
        }
        [Fact]
        public void TestRead()
        {
            var data = Enumerable.Range(0, 100).Select(i =>
             {
                 return new SampleClass()
                 {
                     a = i.ToString()
                     ,
                     b = (short)i
                     ,
                     c = i
                     ,
                     d = (byte)i
                     ,
                     e = i
                     ,
                     f = i
                     ,
                     g = (char)i
                     ,
                     h = DateTime.Now
                     ,
                     i = Guid.Empty
                     ,
                     j = i
                     ,
                     k = Enumerable.Range(0,16).Select(j => (byte)i).ToArray()
                 };
             }).ToArray();
            var ti = typeof(SampleClass).GetTypeInfo();
            var properties = ti.GetProperties();
            using (var reader = data.AsDataReader(typeof(SampleClass)))
            {
                int i = 0;
                while (reader.Read())
                {
                    foreach (var pi in properties)
                    {
                        var ord = reader.GetOrdinal(pi.Name);
                        Assert.Equal(pi.Name, reader.GetName(ord));
                        Assert.Equal(pi.PropertyType, reader.GetFieldType(ord));
                    }
                    Assert.Equal(data[i].a, reader.GetString(reader.GetOrdinal("a")));
                    Assert.Equal(data[i].b, reader.GetInt16(reader.GetOrdinal("b")));
                    Assert.Equal(data[i].c, reader.GetInt32(reader.GetOrdinal("c")));
                    Assert.Equal(data[i].d, reader.GetByte(reader.GetOrdinal("d")));
                    Assert.Equal(data[i].e, reader.GetInt64(reader.GetOrdinal("e")));
                    Assert.Equal(data[i].f, reader.GetDecimal(reader.GetOrdinal("f")));
                    Assert.Equal(data[i].g, reader.GetChar(reader.GetOrdinal("g")));
                    Assert.Equal(data[i].h, reader.GetDateTime(reader.GetOrdinal("h")));
                    Assert.Equal(data[i].i, reader.GetGuid(reader.GetOrdinal("i")));
                    Assert.False(reader.IsDBNull(reader.GetOrdinal("j")));
                    Assert.Equal(data[i].j, reader.GetInt32(reader.GetOrdinal("j")));
                    var values = new object[properties.Length];
                    Assert.Equal(properties.Length, reader.GetValues(values));
                    var buf = new byte[128];
                    var bytesread = reader.GetBytes(reader.GetOrdinal("k"), 0, buf, 0, buf.Length);
                    Assert.Equal(data[i].k.Count, bytesread);
                    i++;
                }
            }
        }
    }
}
