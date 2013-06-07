// SqlDatabase.cs
//

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace SimpleCloud.SqlEdge {

    internal sealed class SqlDatabase {

        private DbProviderFactory _dbProvider;
        private string _connectionString;

        public SqlDatabase(string dbProvider, string connectionString) {
            _dbProvider = DbProviderFactories.GetFactory(dbProvider);
            _connectionString = connectionString;
        }

        private DbCommand CreateCommand(DbConnection connection, string commandText, object[] parameters) {
            DbCommand command = connection.CreateCommand();

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

                    DbParameter parameter = _dbProvider.CreateParameter();
                    parameter.ParameterName = i.ToString(CultureInfo.InvariantCulture);
                    parameter.Value = parameters[i] ?? DBNull.Value;

                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        private DbConnection CreateConnection() {
            DbConnection connection = _dbProvider.CreateConnection();
            connection.ConnectionString = _connectionString;

            return connection;
        }

        private Dictionary<string, object> CreateRow(DbDataReader record, string[] columns) {
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

        private async Task<int> ExecuteAsync(string commandText, object[] parameters) {
            int items = 0;

            using (DbConnection connection = CreateConnection()) {
                using (DbCommand command = CreateCommand(connection, commandText, parameters)) {
                    await connection.OpenAsync();
                    items = await command.ExecuteNonQueryAsync();
                }
            }

            return items;
        }

        public async Task<object> ExecuteSqlAsync(string connectionString, string command, object[] parameters) {
            if (command.StartsWith("select ", StringComparison.OrdinalIgnoreCase)) {
                return await QueryAsync(command, parameters);
            }
            else {
                return await ExecuteAsync(command, parameters);
            }
        }

        private async Task<List<Dictionary<string, object>>> QueryAsync(string commandText, object[] parameters) {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (DbConnection connection = CreateConnection()) {
                using (DbCommand command = CreateCommand(connection, commandText, parameters)) {
                    await connection.OpenAsync();

                    using (DbDataReader reader = await command.ExecuteReaderAsync()) {
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
