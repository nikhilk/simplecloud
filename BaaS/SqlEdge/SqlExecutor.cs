// SqlExecutor.cs
//

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCloud.SqlEdge {

    public sealed class SqlExecutor {

        public async Task<object> ExecuteSQL(IDictionary<string, object> data) {
            string connectionString = GetParameter<string>(data, "connectionString");
            string command = GetParameter<string>(data, "command");
            object[] parameters = GetParameter<object[]>(data, "parameters", required: false, defaultValue: new object[0]);

            SqlDatabase db = new SqlDatabase(connectionString);
            if (command.StartsWith("select ", StringComparison.OrdinalIgnoreCase)) {
                return await db.QueryAsync(command, parameters);
            }
            else if (command.StartsWith("insert ", StringComparison.OrdinalIgnoreCase)) {
                Tuple<int, object> result = await db.ExecuteAsync(command, parameters);
                return new Dictionary<string, object>() {
                    { "items", result.Item1 },
                    { "id", result.Item2 }
                };
            }
            else {
                Tuple<int, object> result = await db.ExecuteAsync(command, parameters);
                return result.Item1;
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
