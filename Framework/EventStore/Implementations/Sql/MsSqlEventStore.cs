using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Framework.EventStore;

namespace TheBookThing.Framework.EventStore.Implementations.MsSql
{
    public class MsSqlEventStore : IEventStore
    {
        readonly string _connectionString;

        public MsSqlEventStore(string connectionString)
        {
            _connectionString = connectionString;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                const string txt =
                    @"IF NOT EXISTS 
                        (SELECT * FROM sys.objects 
                            WHERE object_id = OBJECT_ID(N'[dbo].[Events]') 
                            AND type in (N'U'))

                        CREATE TABLE [dbo].[Events](
                            [Id] [int] PRIMARY KEY IDENTITY,
	                        [Name] [nvarchar](50) NOT NULL,
	                        [GlobalVersion] [int] NOT NULL,
	                        [Version] [int] NOT NULL,
	                        [Data] [varbinary](max) NOT NULL
                        ) ON [PRIMARY]";
                using (var cmd = new SqlCommand(txt, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

        }
        
        public IEnumerable<Record> EnumerateHistory()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string sql =
                    @"SELECT Data, Name, GlobalVersion,Version FROM Events
                        ORDER BY Id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = (byte[])reader["Data"];
                            var name = Guid.Parse((string)reader["Name"]);
                            var globalVersion = (int)reader["GlobalVersion"];
                            var version = (int)reader["Version"];
                            yield return new Record(name, version,globalVersion,data);
                        }
                    }
                }
            }
        }

        public void Append(IEnumerable<Record> newRecords)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    foreach (var newRecord in newRecords)
                    {
                        const string txt =
                            @"INSERT INTO Events (Name,GlobalVersion,Version,Data) 
                                VALUES(@name,@globalversion,@version,@data)";

                        using (var cmd = new SqlCommand(txt, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@name", newRecord.AggregateIdentifier.ToString());
                            cmd.Parameters.AddWithValue("@globalversion", newRecord.GlobalRevision);
                            cmd.Parameters.AddWithValue("@version", newRecord.Revision);
                            cmd.Parameters.AddWithValue("@data", newRecord.Data);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
            }
        }

    }
}