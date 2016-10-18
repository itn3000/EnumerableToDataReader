using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader.Test
{
    using Xunit;
    using System.Reflection;
    public class TestDataReaderExtension
    {
        public void TestToDictionary()
        {
            var data = Enumerable.Range(0, 100)
                .Select(i =>
                {
                    return new SampleClass()
                    {
                        a = i.ToString()
                        ,
                        b = (short)(i & 0xffff)
                        ,
                        c = i
                        ,
                        d = (byte)(i & 0xff)
                        ,
                        e = i
                        ,
                        f = i
                        ,
                        g = (char)i
                        ,
                        h = DateTime.MinValue.AddDays(i)
                        ,
                        i = Guid.Empty
                        ,
                        j = i
                        ,
                        k = Enumerable.Range(0, i).Select(j => (byte)(i & 0xff)).ToArray()
                    };
                }).ToArray()
                ;
            var properties = typeof(SampleClass).GetTypeInfo().GetProperties().ToDictionary(x => x.Name, x => x);
            using (var dr = data.AsDataReader())
            {
                int cnt = 0;
                foreach (var x in dr.ToDictionary().Select((dic,i) => new { dic, i }))
                {
                    Assert.Equal(properties.Count, x.dic.Count);
                    foreach(var kv in x.dic)
                    {
                        Assert.Equal(properties[kv.Key].GetValue(data[x.i]), kv.Value);
                    }
                    cnt++;
                }
                Assert.Equal(data.Length, cnt);
            }
        }
    }
}
