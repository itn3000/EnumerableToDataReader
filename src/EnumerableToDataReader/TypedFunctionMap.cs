using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    using System.Reflection;
    using System.Linq.Expressions;
    class TypedFunctionMap<T>
    {
        public TypedFunctionMap()
        {
            InitializeMapFromType(typeof(T));
        }
        public Type ClassType { get; set; }
        public Dictionary<int, Func<T, object>> ObjectGetters { get; private set; } = new Dictionary<int, Func<T, object>>();
        public Dictionary<int, Func<T, char>> CharGetters { get; private set; } = new Dictionary<int, Func<T, char>>();
        public Dictionary<int, Func<T, int>> IntGetters { get; private set; } = new Dictionary<int, Func<T, int>>();
        public Dictionary<int, Func<T, long>> LongGetters { get; private set; } = new Dictionary<int, Func<T, long>>();
        public Dictionary<int, Func<T, bool>> BoolGetters { get; private set; } = new Dictionary<int, Func<T, bool>>();
        public Dictionary<int, Func<T, short>> ShortGetters { get; private set; } = new Dictionary<int, Func<T, short>>();
        public Dictionary<int, Func<T, byte>> ByteGetters { get; private set; } = new Dictionary<int, Func<T, byte>>();
        public Dictionary<int, Func<T, float>> FloatGetters { get; private set; } = new Dictionary<int, Func<T, float>>();
        public Dictionary<int, Func<T, double>> DoubleGetters { get; private set; } = new Dictionary<int, Func<T, double>>();
        public Dictionary<int, Func<T, DateTime>> DateGetters { get; private set; } = new Dictionary<int, Func<T, DateTime>>();
        public Dictionary<int, Func<T, decimal>> DecimalGetters { get; private set; } = new Dictionary<int, Func<T, decimal>>();
        public Dictionary<int, Func<T, Guid>> GuidGetters { get; set; } = new Dictionary<int, Func<T, Guid>>();
        public Dictionary<int, string> IndexNameMapping { get; private set; }
        public Dictionary<string, int> NameIndexMapping { get; private set; }
        public Dictionary<string, Type> MemberTypeMapping { get; private set; } = new Dictionary<string, Type>();
        public int FieldNum { get; private set; }
        Func<T, T1> GetObjectFunction<T1>(Type t, PropertyInfo pi)
        {
            var lambdaParam = Expression.Parameter(t, "x");
            return Expression.Lambda<Func<T, T1>>(Expression.Convert(Expression.MakeMemberAccess(lambdaParam, pi), typeof(T1)), lambdaParam).Compile();
        }
        void InitializeMapFromType(Type t)
        {
            var ti = t.GetTypeInfo();
            var properties = ti.GetProperties();
            FieldNum = properties.Length;
            IndexNameMapping = properties.Select((x, i) => new { i, x.Name }).ToDictionary(x => x.i, x => x.Name);
            NameIndexMapping = IndexNameMapping.Select(kv => kv).ToDictionary(kv => kv.Value, kv => kv.Key);
            MemberTypeMapping = new Dictionary<string, Type>();
            ObjectGetters = new Dictionary<int, Func<T, object>>();
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
