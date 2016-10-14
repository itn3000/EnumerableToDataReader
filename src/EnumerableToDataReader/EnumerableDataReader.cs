using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    using System.Data;
    using System.Collections.Concurrent;
    using System.Collections;
    class EnumerableDataReader : IDataReader
    {
        static readonly ConcurrentDictionary<Type, FunctionMap> m_StaticFunctionMap = new ConcurrentDictionary<Type, FunctionMap>();
        IEnumerable m_DataList;
        IEnumerator m_Current;
        Type m_Type;
        object m_Lock = new object();
        FunctionMap m_FunctionMap;
        public EnumerableDataReader(Type t, IEnumerable dataList)
        {
            lock (m_Lock)
            {
                if (!m_StaticFunctionMap.ContainsKey(t))
                {
                    m_StaticFunctionMap[t] = new FunctionMap(t);
                }
                m_FunctionMap = m_StaticFunctionMap[t];
            }
            m_DataList = dataList;
            m_Type = t;
            m_Current = dataList.GetEnumerator();
        }
        public object this[string name]
        {
            get
            {
                return m_FunctionMap.ObjectGetters[m_FunctionMap.NameIndexMapping[name]](m_Current.Current);
            }
        }

        public object this[int i]
        {
            get
            {
                return m_FunctionMap.ObjectGetters[i](m_Current.Current);
            }
        }

        public int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int FieldCount
        {
            get
            {
                return m_FunctionMap.FieldNum;
            }
        }

        public bool IsClosed
        {
            get
            {
                return m_Current == null;
            }
        }

        public int RecordsAffected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            m_Current = null;
        }

        public void Dispose()
        {
            Close();
        }

        public bool GetBoolean(int i)
        {
            return m_FunctionMap.BoolGetters[i](m_Current.Current);
        }

        public byte GetByte(int i)
        {
            return m_FunctionMap.ByteGetters[i](m_Current.Current);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
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

        public char GetChar(int i)
        {
            return m_FunctionMap.CharGetters[i](m_Current.Current);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
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

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return m_FunctionMap.MemberTypeMapping[m_FunctionMap.IndexNameMapping[i]].ToString();
        }

        public DateTime GetDateTime(int i)
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

        public decimal GetDecimal(int i)
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

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            return m_FunctionMap.MemberTypeMapping[m_FunctionMap.IndexNameMapping[i]];
        }

        public float GetFloat(int i)
        {
            return m_FunctionMap.FloatGetters[i](m_Current.Current);
        }

        public Guid GetGuid(int i)
        {
            return m_FunctionMap.GuidGetters[i](m_Current.Current);
        }

        public short GetInt16(int i)
        {
            return m_FunctionMap.ShortGetters[i](m_Current.Current);
        }

        public int GetInt32(int i)
        {
            return m_FunctionMap.IntGetters[i](m_Current.Current);
        }

        public long GetInt64(int i)
        {
            return m_FunctionMap.LongGetters[i](m_Current.Current);
        }

        public string GetName(int i)
        {
            return m_FunctionMap.IndexNameMapping[i];
        }

        public int GetOrdinal(string name)
        {
            return m_FunctionMap.NameIndexMapping[name];
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return m_FunctionMap.ObjectGetters[i](m_Current.Current).ToString();
        }

        public object GetValue(int i)
        {
            return m_FunctionMap.ObjectGetters[i](m_Current.Current);
        }

        public int GetValues(object[] values)
        {
            for (int i = 0; i < values.Length && i < m_FunctionMap.FieldNum; i++)
            {
                values[i] = m_FunctionMap.ObjectGetters[i];
            }
            return values.Length < m_FunctionMap.FieldNum ? values.Length : m_FunctionMap.FieldNum;
        }

        public bool IsDBNull(int i)
        {
            return m_FunctionMap.ObjectGetters[i](m_Current.Current) == null;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            return m_Current.MoveNext();
        }
    }
}
