// SqlExecutor.cs
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCloud.SqlEdge {

    public sealed class SqlExecutor {

        private const string SqlServerDbProvider = "System.Data.SqlClient";
        private const string SqlFileDbProvider = "System.Data.SqlServerCe.4.0";

        public Task<object> ExecuteSql(IDictionary<string, object> data) {
            string dbProvider = SqlServerDbProvider;
            bool useThreadPool = false;

            string connectionString = GetParameter<string>(data, "connectionString");
            if (data.ContainsKey("localFile")) {
                dbProvider = SqlFileDbProvider;
                useThreadPool = true;

                connectionString = String.Format("Data Source={0};File Access Retry Timeout=10", connectionString);
            }

            string command = GetParameter<string>(data, "command");
            object[] parameters = GetParameter<object[]>(data, "parameters", required: false, defaultValue: new object[0]);

            SqlDatabase db = new SqlDatabase(dbProvider, connectionString);

            if (useThreadPool) {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                ThreadPool.QueueUserWorkItem(async (o) => {
                    try {
                        object result = await db.ExecuteSqlAsync(connectionString, command, parameters);
                        tcs.SetResult(result);
                    }
                    catch (Exception e) {
                        tcs.SetException(e);
                    }
                });

                return tcs.Task;
            }
            else {
                return db.ExecuteSqlAsync(connectionString, command, parameters);
            }
        }

        private static T GetParameter<T>(IDictionary<string, object> data, string name, bool required = true, T defaultValue = default(T)) {
            object value;
            if (data.TryGetValue(name, out value)) {
                if ((required == false) && (value == null)) {
                    return defaultValue;
                }

                if (typeof(T).IsAssignableFrom(value.GetType())) {
                    return (T)value;
                }
            }

            if (required) {
                throw new ArgumentException("Missing or invalid value for parameter '" + name + "'");
            }

            return defaultValue;
        }
    }
}
