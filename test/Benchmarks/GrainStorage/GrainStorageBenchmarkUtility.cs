using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks.GrainStorage;

// only a hack - it is not nice

internal class GrainStorageBenchmarkUtility
{
    private static string _DataConnectionString;
    internal static void Prepare(string dataConnectionString)
    {
        _DataConnectionString = dataConnectionString;
        using (var sqlConnection = new SqlConnection(dataConnectionString))
        {
            var database = sqlConnection.Database;
            sqlConnection.Open();
            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "DELETE FROM [dbo].[OrleansStorage]";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "DBCC FREEPROCCACHE";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"BACKUP DATABASE [{database}] TO  DISK = N'NUL:' WITH NOFORMAT, NOINIT,  NAME = N'Orleans', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"BACKUP Log [{database}] TO  DISK = N'NUL:' WITH NOFORMAT, NOINIT,  NAME = N'Orleans', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                cmd.ExecuteNonQuery();
            }
        }


    }

    internal static (string Json, int Size) ReadResults()
    {
        string Json;
        int Size;
        using (var sqlConnection = new SqlConnection(_DataConnectionString))
        {
            var database = sqlConnection.Database;
            sqlConnection.Open();
            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = """
                     SELECT 
                        cp.objtype AS PlanType,
                        cp.refcounts AS ReferenceCounts,
                        cp.usecounts AS UseCounts,
                        cp.size_in_bytes,
                        st. text AS SQLBatch,
                        cp.plan_handle
                      FROM sys.dm_exec_cached_plans AS cp
                      CROSS APPLY sys.dm_exec_query_plan (cp.plan_handle) AS qp
                      CROSS APPLY sys.dm_exec_sql_text (cp.plan_handle) AS st
                      WHERE st.text LIKE '(@GrainIdHash%' 
                      FOR JSON AUTO;
                    """;
                Json = (string)cmd.ExecuteScalar();
            }
            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = """
                      SELECT 
                        size_in_kbytes = SUM(cp.size_in_bytes) / 1024
                      FROM sys.dm_exec_cached_plans AS cp
                      CROSS APPLY sys.dm_exec_query_plan (cp.plan_handle) AS qp
                      CROSS APPLY sys.dm_exec_sql_text (cp.plan_handle) AS st
                      WHERE st.text LIKE '(@GrainIdHash%' 
                    """;
                Size = (int)cmd.ExecuteScalar();
            }
        }
        return (Json, Size);
    }
}
