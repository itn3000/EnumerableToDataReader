using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Data;
using System.Collections.Concurrent;

namespace EnumerableToDataReader
{
    using System.Collections;
    using System.Data.Common;
    class TypedEnumerableDataReader<T> : DbDataReader
    {

        static readonly Lazy<TypedFunctionMap<T>> m_StaticFunctionMap = new Lazy<TypedFunctionMap<T>>();
        IEnumerable<T> m_DataList;
        IEnumerator<T> m_Current;
        TypedFunctionMap<T> m_FunctionMap
        {
            get
            {
                return m_StaticFunctionMap.Value;
            }
        }
        public TypedEnumerableDataReader(IEnumerable<T> dataList)
        {
            m_DataList = dataList;
            m_Current = dataList.GetEnumerator();
        }
        public new void Dispose()
        {
            Close();
            base.Dispose();
        }
        public override object this[string name]
        {
            get
            {
                return m_FunctionMap.ObjectGetters[m_FunctionMap.NameIndexMapping[name]](m_Current.Current);
            }
        }

        public override object this[int i]
        {
            get
            {
                return m_FunctionMap.ObjectGetters[i](m_Current.Current);
            }
        }

        public override int Depth
        {
            get
            {
                return 0;
            }
        }

        public override int FieldCount
        {
            get
            {
                return m_FunctionMap.FieldNum;
            }
        }

        public override bool IsClosed
        {
            get
            {
                return m_Current == null;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                return 0;
            }
        }

        public override bool HasRows
        {
            get
            {
                return m_Current != null || m_DataList.Any();
            }
        }

        public override bool GetBoolean(int i)
        {
            return m_FunctionMap.BoolGetters[i](m_Current.Current);
        }

        public override byte GetByte(int i)
        {
            return m_FunctionMap.ByteGetters[i](m_Current.Current);
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var data = ((IEnumerable<byte>)m_FunctionMap.ObjectGetters[i](m_Current.Current)).Skip((int)fieldOffset).Take(length).ToArray();
            long readLength = 0;
            for (int idx = 0; idx < data.Length && idx < length; idx++)
            {
                buffer[bufferoffset + idx] = data[idx];
                readLength += 1;
            }
            return readLength;
        }

        public override char GetChar(int i)
        {
            return m_FunctionMap.CharGetters[i](m_Current.Current);
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var data = ((IEnumerable<char>)m_FunctionMap.ObjectGetters[i](m_Current.Current)).Skip((int)fieldoffset).Take(length).ToArray();
            var readlength = 0;
            for (int idx = 0; idx < data.Length; idx++)
            {
                buffer[bufferoffset + idx] = data[idx];
                readlength += 1;
            }
            return readlength;
        }

        public override string GetDataTypeName(int i)
        {
            return m_FunctionMap.MemberTypeMapping[m_FunctionMap.IndexNameMapping[i]].ToString();
        }

        public override DateTime GetDateTime(int i)
        {
            if (m_FunctionMap.DateGetters.ContainsKey(i))
            {
                return m_FunctionMap.DateGetters[i](m_Current.Current);
            }
            else
            {
                return DateTime.Parse(m_FunctionMap.ObjectGetters[i](m_Current.Current).ToString());
            }
        }

        public override decimal GetDecimal(int i)
        {
            if (m_FunctionMap.DecimalGetters.ContainsKey(i))
            {
                return m_FunctionMap.DecimalGetters[i](m_Current.Current);
            }
            else if (m_FunctionMap.IntGetters.ContainsKey(i))
            {
                return m_FunctionMap.IntGetters[i](m_Current.Current);
            }
            else if (m_FunctionMap.LongGetters.ContainsKey(i))
            {
                return m_FunctionMap.LongGetters[i](m_Current.Current);
            }
            else if (m_FunctionMap.ShortGetters.ContainsKey(i))
            {
                return m_FunctionMap.ShortGetters[i](m_Current.Current);
            }
            else if (m_FunctionMap.ByteGetters.ContainsKey(i))
            {
                return m_FunctionMap.ByteGetters[i](m_Current.Current);
            }
            else
            {
                return (decimal)m_FunctionMap.ObjectGetters[i](m_Current.Current);
            }
        }

        public override double GetDouble(int i)
        {
            return m_FunctionMap.DoubleGetters[i](m_Current.Current);
        }

        public override Type GetFieldType(int i)
        {
            return m_FunctionMap.MemberTypeMapping[m_FunctionMap.IndexNameMapping[i]];
        }

        public override float GetFloat(int i)
        {
            return m_FunctionMap.FloatGetters[i](m_Current.Current);
        }

        public override Guid GetGuid(int i)
        {
            return m_FunctionMap.GuidGetters[i](m_Current.Current);
        }

        public override short GetInt16(int i)
        {
            return m_FunctionMap.ShortGetters[i](m_Current.Current);
        }

        public override int GetInt32(int i)
        {
            return m_FunctionMap.IntGetters[i](m_Current.Current);
        }

        public override long GetInt64(int i)
        {
            return m_FunctionMap.LongGetters[i](m_Current.Current);
        }

        public override string GetName(int i)
        {
            return m_FunctionMap.IndexNameMapping[i];
        }

        public override int GetOrdinal(string name)
        {
            return m_FunctionMap.NameIndexMapping[name];
        }

        public override string GetString(int i)
        {
            return m_FunctionMap.ObjectGetters[i](m_Current.Current).ToString();
        }

        public override object GetValue(int i)
        {
            return m_FunctionMap.ObjectGetters[i](m_Current.Current);
        }

        public override int GetValues(object[] values)
        {
            for (int i = 0; i < values.Length && i < m_FunctionMap.FieldNum; i++)
            {
                values[i] = m_FunctionMap.ObjectGetters[i];
            }
            return values.Length < m_FunctionMap.FieldNum ? values.Length : m_FunctionMap.FieldNum;
        }

        public override bool IsDBNull(int i)
        {
            var obj = m_FunctionMap.ObjectGetters[i](m_Current.Current);
            if (obj == null || obj is DBNull)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (!m_Current.MoveNext())
            {
                m_Current = null;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override IEnumerator GetEnumerator()
        {
            return m_Current;
        }
#if NET45
        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }
        public override void Close()
        {
            m_Current?.Dispose();
            m_Current = null;
        }
#else
        public void Close()
        {
            m_Current?.Dispose();
            m_Current = null;
        }
#endif
    }
}
