using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader.Test
{
    using Xunit;
    public class TestAttribute
    {
        class AttributeTestClass
        {
            public int a { get; set; }
            [IgnoreField]
            public string b { get; set; }
            [FieldNameAs("x")]
            public double c { get; set; }
        }
        [Fact]
        public void TestIgnore()
        {
            using (var dr = Enumerable.Range(0, 10).Select(i => new AttributeTestClass() { a = i, b = i.ToString(), c = i }).AsDataReader())
            {
                int cnt = 0;
                while (dr.Read())
                {
                    Assert.Equal(2, dr.FieldCount);
                    Assert.Equal(cnt, dr.GetInt32(dr.GetOrdinal("a")));
                    Assert.Equal((double)cnt, dr.GetDouble(dr.GetOrdinal("x")));
                    cnt++;
                }
            }
        }
    }
}
