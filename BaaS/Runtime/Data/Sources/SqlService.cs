// SqlService.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi;
using NodeApi.Interop;
using NodeApi.IO;

namespace SimpleCloud.Data.Sources {

    public sealed class SqlService {

        private string _connectionString;
        private Action<object, AsyncResultCallback<object>> _executor;

        internal SqlService(string connectionString) {
            _connectionString = connectionString;
        }

        private void EnsureExecutor() {
            if (_executor == null) {
                ManagedLibrary implementation = new ManagedLibrary();
                implementation.AssemblyFile = Path.Join((string)Script.Literal("__dirname"), "SqlEdge.dll");
                implementation.TypeName = "SimpleCloud.SqlEdge.SqlExecutor";
                implementation.MethodName = "ExecuteSQL";

                _executor = Edge.BindToLibrary(implementation);
            }
        }

        public Task<object> Sql(string command, object[] parameters) {
            EnsureExecutor();

            Dictionary<string, object> executionArgs = new Dictionary<string, object>();
            executionArgs["connectionString"] = _connectionString;
            executionArgs["command"] = command;
            executionArgs["parameters"] = Script.Or(parameters, null);

            Deferred<object> deferred = Deferred.Create<object>();
            _executor(executionArgs, delegate(Exception e, object o) {
                if (e != null) {
                    deferred.Reject(e);
                }
                else {
                    deferred.Resolve(o);
                }
            });
            return deferred.Task;
        }
    }
}
