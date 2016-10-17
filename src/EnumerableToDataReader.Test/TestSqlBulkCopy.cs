using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnumerableToDataReader.Test
{
    using System.Data;
    using System.Data.SqlClient;
    using Xunit;
    using Microsoft.Data.Sqlite;
    using Xunit.Abstractions;
    public class TestSqlBulkCopy
    {
        readonly ITestOutputHelper m_Outputter;
        public TestSqlBulkCopy(ITestOutputHelper outputter)
        {
            m_Outputter = outputter;
        }
        class bulkcopyclass
        {
            public int a;
            public string b;
        }
        void CreateDb(string dataSource, string dbName)
        {
            var cb = new SqlConnectionStringBuilder();
            cb.DataSource = dataSource;
            cb.IntegratedSecurity = true;
            using (var con = new SqlConnection(cb.ConnectionString))
            {
                con.Open();
                cb.AsDataReader(typeof(int));
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"
if not exists (select * from sys.databases where name='{dbName}')
begin
    create database [{dbName}]
end
";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        void CreateTable(IDbConnection con, IDbTransaction trans, string tableName)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = $@"
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='{tableName}')
begin
    drop table {tableName}
end
create table {tableName}(a int,b nvarchar(255))
";
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
            }
        }
        void DropDb(string dataSource, string dbName)
        {
            var cb = new SqlConnectionStringBuilder();
            cb.DataSource = dataSource;
            cb.IntegratedSecurity = true;
            using (var con = new SqlConnection(cb.ConnectionString))
            {
                con.Open();
                cb.AsDataReader(typeof(int));
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"
if exists (select * from sys.databases where name='{dbName}')
begin
    drop database [{dbName}]
end
";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        [Fact]
        public void TestCopy()
        {
            var mastercb = new SqlConnectionStringBuilder();
            mastercb.DataSource = "(localdb)\\EnumerableToDataReader";
            mastercb.IntegratedSecurity = true;
            string dbName = "testbulkcopy";
            string tableName = "testcopy";
            CreateDb(mastercb.DataSource, dbName);
            try
            {
                var cb = new SqlConnectionStringBuilder(mastercb.ConnectionString);
                cb.InitialCatalog = dbName;
                using (var con = new SqlConnection(cb.ConnectionString))
                {
                    con.Open();
                    CreateTable(con, null, tableName);
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    using (var bc = new SqlBulkCopy(con))
                    {
                        bc.DestinationTableName = tableName;
                        using (var dr = Enumerable.Range(0, 10000).Select(i => new bulkcopyclass() { a = i, b = $"hoge{i}" }).AsDataReader())
                        {
                            bc.WriteToServer(dr);
                        }
                    }
                    m_Outputter.WriteLine($"insert elapsed={sw.Elapsed}");
                    sw.Reset();
                    sw.Start();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"select count(*) as cnt from {tableName}";
                        using (var dr = cmd.ExecuteReader())
                        {
                            int cnt = 0;
                            if (dr.Read())
                            {
                                cnt = dr.GetInt32(dr.GetOrdinal("cnt"));
                            }
                            Assert.Equal(10000, cnt);
                        }
                    }
                    m_Outputter.WriteLine($"read elapsed={sw.Elapsed}");
                }
            }
            finally
            {
                try
                {
                    // too slow
                    //DropDb(mastercb.DataSource, dbName);
                }
                catch
                {
                }
            }
        }
    }
}
