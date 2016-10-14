using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    using System.Linq.Expressions;
    using System.Reflection;
    public class FunctionMap
    {
        public FunctionMap(Type t)
        {
            InitializeMapFromType(t);
        }
        public Type ClassType { get; set; }
        public Dictionary<int, Func<object, object>> ObjectGetters { get; private set; } = new Dictionary<int, Func<object, object>>();
        public Dictionary<int, Func<object, char>> CharGetters { get; private set; } = new Dictionary<int, Func<object, char>>();
        public Dictionary<int, Func<object, int>> IntGetters { get; private set; } = new Dictionary<int, Func<object, int>>();
        public Dictionary<int, Func<object, long>> LongGetters { get; private set; } = new Dictionary<int, Func<object, long>>();
        public Dictionary<int, Func<object, bool>> BoolGetters { get; private set; } = new Dictionary<int, Func<object, bool>>();
        public Dictionary<int, Func<object, short>> ShortGetters { get; private set; } = new Dictionary<int, Func<object, short>>();
        public Dictionary<int, Func<object, byte>> ByteGetters { get; private set; } = new Dictionary<int, Func<object, byte>>();
        public Dictionary<int, Func<object, float>> FloatGetters { get; private set; } = new Dictionary<int, Func<object, float>>();
        public Dictionary<int, Func<object, double>> DoubleGetters { get; private set; } = new Dictionary<int, Func<object, double>>();
        public Dictionary<int, Func<object, DateTime>> DateGetters { get; private set; } = new Dictionary<int, Func<object, DateTime>>();
        public Dictionary<int, Func<object, decimal>> DecimalGetters { get; private set; } = new Dictionary<int, Func<object, decimal>>();
        public Dictionary<int, Func<object, Guid>> GuidGetters { get; set; } = new Dictionary<int, Func<object, Guid>>();
        public Dictionary<string, Type> MemberTypeMapping { get; private set; } = new Dictionary<string, Type>();
        public Dictionary<int, string> IndexNameMapping { get; private set; }
        public Dictionary<string, int> NameIndexMapping { get; private set; }
        public int FieldNum { get; private set; }
        Func<object, T1> GetObjectFunction<T1>(Type t, PropertyInfo pi)
        {
            var lambdaParam = Expression.Parameter(typeof(object), "x");
            return Expression.Lambda<Func<object, T1>>(Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(lambdaParam, t), pi), typeof(T1)), lambdaParam).Compile();
        }
        void InitializeMapFromType(Type t)
        {
            var ti = t.GetTypeInfo();
            var properties = ti.GetProperties();
            FieldNum = properties.Length;
            IndexNameMapping = properties.Select((x, i) => new { i, x.Name }).ToDictionary(x => x.i, x => x.Name);
            NameIndexMapping = IndexNameMapping.Select(kv => kv).ToDictionary(kv => kv.Value, kv => kv.Key);
            MemberTypeMapping = new Dictionary<string, Type>();
            ObjectGetters = new Dictionary<int, Func<object, object>>();
            for (int i = 0; i < properties.Length; i++)
            {
                var pi = properties[i];
                MemberTypeMapping[pi.Name] = pi.PropertyType;
                ObjectGetters[i] = GetObjectFunction<object>(t, pi);
                if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                {
                    LongGetters[i] = GetObjectFunction<long>(t, pi);
                    DecimalGetters[i] = GetObjectFunction<decimal>(t, pi);
                }
                else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                {
                    IntGetters[i] = GetObjectFunction<int>(t, pi);
                    LongGetters[i] = GetObjectFunction<long>(t, pi);
                    DecimalGetters[i] = GetObjectFunction<decimal>(t, pi);
                }
                else if (pi.PropertyType == typeof(short) || pi.PropertyType == typeof(short?))
                {
                    ShortGetters[i] = GetObjectFunction<short>(t, pi);
                    IntGetters[i] = GetObjectFunction<int>(t, pi);
                    LongGetters[i] = GetObjectFunction<long>(t, pi);
                    DecimalGetters[i] = GetObjectFunction<decimal>(t, pi);
                }
                else if (pi.PropertyType == typeof(byte) || pi.PropertyType == typeof(byte?))
                {
                    ByteGetters[i] = GetObjectFunction<byte>(t, pi);
                    ShortGetters[i] = GetObjectFunction<short>(t, pi);
                    IntGetters[i] = GetObjectFunction<int>(t, pi);
                    LongGetters[i] = GetObjectFunction<long>(t, pi);
                    DecimalGetters[i] = GetObjectFunction<decimal>(t, pi);
                }
                else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?))
                {
                    ShortGetters[i] = GetObjectFunction<short>(t, pi);
                    IntGetters[i] = GetObjectFunction<int>(t, pi);
                    LongGetters[i] = GetObjectFunction<long>(t, pi);
                    DoubleGetters[i] = GetObjectFunction<double>(t, pi);
                    FloatGetters[i] = GetObjectFunction<float>(t, pi);
                    DecimalGetters[i] = GetObjectFunction<decimal>(t, pi);
                }
                else if (pi.PropertyType == typeof(char) || pi.PropertyType == typeof(char?))
                {
                    CharGetters[i] = GetObjectFunction<char>(t, pi);
                }
                else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?))
                {
                    BoolGetters[i] = GetObjectFunction<bool>(t, pi);
                }
                else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                {
                    DateGetters[i] = GetObjectFunction<DateTime>(t, pi);
                }
                else if (pi.PropertyType == typeof(double) || pi.PropertyType == typeof(double?))
                {
                    DoubleGetters[i] = GetObjectFunction<double>(t, pi);
                }
                else if (pi.PropertyType == typeof(float) || pi.PropertyType == typeof(float?))
                {
                    DoubleGetters[i] = GetObjectFunction<double>(t, pi);
                    FloatGetters[i] = GetObjectFunction<float>(t, pi);
                }
                else if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Guid?))
                {
                    GuidGetters[i] = GetObjectFunction<Guid>(t, pi);
                }
            }
        }
    }
}
