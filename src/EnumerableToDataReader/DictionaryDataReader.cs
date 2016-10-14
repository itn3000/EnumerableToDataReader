using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader
{
    using System.Data;
    class DictionaryDataReader : IDataReader
    {
        IEnumerable<IDictionary<string, object>> m_Dictionary;
        IEnumerator<IDictionary<string, object>> m_Iterator;
        IDictionary<string, object> m_Current;
        IDictionary<int, string> m_IndexToNameMapping;
        IDictionary<string, int> m_NameToIndexMapping;
        /// <summary>
        /// creating new dictionary datareader,name list is read from first element.
        /// </summary>
        /// <remarks>
        /// <para>field info(ex. FieldCount,GetName(),etc.) is not enabled until Read() executed</para>
        /// <para>all records are expected to have same keys and types.</para>
        /// </remarks>
        /// <param name="dic">dictionary list.</param>
        public DictionaryDataReader(IEnumerable<IDictionary<string, object>> dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            m_Dictionary = dic;
        }
        public object this[string name]
        {
            get
            {
                return m_Current[name];
            }
        }

        public object this[int i]
        {
            get
            {
                return m_Current[m_IndexToNameMapping[i]];
            }
        }

        public int Depth
        {
            get
            {
                return 1;
            }
        }

        public int FieldCount
        {
            get
            {
                return m_IndexToNameMapping.Count();
            }
        }

        public bool IsClosed
        {
            get
            {
                return m_Iterator == null;
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
            if (m_Iterator != null)
            {
                m_Current = null;
                m_Iterator.Dispose();
                m_Iterator = null;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public bool GetBoolean(int i)
        {
            return (bool)m_Current[m_IndexToNameMapping[i]];
        }

        public byte GetByte(int i)
        {
            return (byte)m_Current[m_IndexToNameMapping[i]];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var data = m_Current[m_IndexToNameMapping[i]] as IEnumerable<byte>;
            long count = 0;
            foreach (var x in data.Skip((int)fieldOffset).Take(length).Select((b, idx) => new { b, idx }))
            {
                buffer[bufferoffset + x.idx] = x.b;
                count++;
            }
            return count;
        }

        public char GetChar(int i)
        {
            return (char)m_Current[m_IndexToNameMapping[i]];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var data = m_Current[m_IndexToNameMapping[i]] as IEnumerable<char>;
            long count = 0;
            foreach (var x in data.Skip((int)fieldoffset).Take(length).Select((c, idx) => new { c, idx }))
            {
                buffer[bufferoffset + x.idx] = x.c;
                count++;
            }
            return count;
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return m_Current[m_IndexToNameMapping[i]].GetType().ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)m_Current[m_IndexToNameMapping[i]];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)m_Current[m_IndexToNameMapping[i]];
        }

        public double GetDouble(int i)
        {
            return (double)m_Current[m_IndexToNameMapping[i]];
        }

        public Type GetFieldType(int i)
        {
            return m_Current[m_IndexToNameMapping[i]].GetType();
        }

        public float GetFloat(int i)
        {
            return (float)m_Current[m_IndexToNameMapping[i]];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)m_Current[m_IndexToNameMapping[i]];
        }

        public short GetInt16(int i)
        {
            return (short)m_Current[m_IndexToNameMapping[i]];
        }

        public int GetInt32(int i)
        {
            return (int)m_Current[m_IndexToNameMapping[i]];
        }

        public long GetInt64(int i)
        {
            return (long)m_Current[m_IndexToNameMapping[i]];
        }

        public string GetName(int i)
        {
            return m_IndexToNameMapping[i];
        }

        public int GetOrdinal(string name)
        {
            return m_NameToIndexMapping[name];
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)m_Current[m_IndexToNameMapping[i]];
        }

        public object GetValue(int i)
        {
            return this[i];
        }

        public int GetValues(object[] values)
        {
            int count = 0;
            for (int i = 0; i < this.FieldCount && i < values.Length; i++)
            {
                values[i] = this[i];
                count++;
            }
            return count;
        }

        public bool IsDBNull(int i)
        {
            return m_Current[m_IndexToNameMapping[i]] == null
                || m_Current[m_IndexToNameMapping[i]] is DBNull;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (m_Iterator == null)
            {
                m_Iterator = m_Dictionary.GetEnumerator();
            }
            var ret = m_Iterator.MoveNext();
            if (ret)
            {
                m_Current = m_Iterator.Current;
                if (m_IndexToNameMapping == null || m_NameToIndexMapping == null)
                {
                    m_IndexToNameMapping = m_Current.Keys.Select((x, i) => new { x, i }).ToDictionary(x => x.i, x => x.x);
                    m_NameToIndexMapping = m_IndexToNameMapping.ToDictionary(kv => kv.Value, kv => kv.Key);
                }
            }
            return ret;
        }
    }
}
