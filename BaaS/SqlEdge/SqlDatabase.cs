// SqlDatabase.cs
//

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;

namespace SimpleCloud.SqlEdge {

    internal sealed class SqlDatabase {

        private string _connectionString;

        public SqlDatabase(string connectionString) {
            _connectionString = connectionString;
        }

        private SqlCommand CreateCommand(SqlConnection connection, string commandText, object[] parameters) {
            SqlCommand command = connection.CreateCommand();

            command.CommandText = commandText;
            if (parameters != null) {
                for (int i = 0; i < parameters.Length; i++) {
                    object value = parameters[i];

                    if (value is string) {
                        DateTime dt;
                        if (DateTime.TryParse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt)) {
                            value = dt;
                        }
                    }

                    SqlParameter parameter = command.CreateParameter();
                    parameter.ParameterName = i.ToString(CultureInfo.InvariantCulture);
                    parameter.Value = parameters[i] ?? DBNull.Value;

                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        private Dictionary<string, object> CreateRow(SqlDataReader record, string[] columns) {
            Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.Ordinal);
            for (int i = 0; i < columns.Length; i++) {
                object value = record.GetValue(i);

                if (value == DBNull.Value) {
                    value = null;
                }
                else {
                    Type valueType = value.GetType();
                    if (valueType == typeof(Guid)) {
                        value = value.ToString();
                    }
                    else if (valueType == typeof(DateTime)) {
                        value = ((DateTime)value).ToString("R", CultureInfo.InvariantCulture);
                    }
                }

                row[columns[i]] = value;
            }

            return row;
        }

        public async Task<Tuple<int, object>> ExecuteAsync(string commandText, object[] parameters) {
            int items = 0;
            object id = null;

            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand command = CreateCommand(connection, commandText, parameters)) {
                    await connection.OpenAsync();
                    items = await command.ExecuteNonQueryAsync();
                }

                if (commandText.StartsWith("insert ", StringComparison.OrdinalIgnoreCase)) {
                    using (SqlCommand idCommand = CreateCommand(connection, "SELECT @@Identity", null)) {
                        id = await idCommand.ExecuteScalarAsync();
                    }
                }
            }

            return new Tuple<int, object>(items, id);
        }

        public async Task<List<Dictionary<string, object>>> QueryAsync(string commandText, object[] parameters) {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand command = CreateCommand(connection, commandText, parameters)) {
                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        string[] columns = null;

                        while (reader.Read()) {
                            if (columns == null) {
                                columns = new string[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i++) {
                                    columns[i] = reader.GetName(i);
                                }
                            }

                            Dictionary<string, object> row = CreateRow(reader, columns);
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }
    }
}
