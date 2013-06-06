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
        
        private static Action<object, AsyncResultCallback<object>> _executor;
        
        private string _connectionString;
        private bool _localFile;

        internal SqlService(string connectionString, bool localFile) {
            _connectionString = connectionString;
            _localFile = localFile;
        }

        private static void EnsureExecutor() {
            if (_executor == null) {
                ManagedLibrary implementation = new ManagedLibrary();
                implementation.AssemblyFile = Path.Join((string)Script.Literal("__dirname"), "SqlEdge.dll");
                implementation.TypeName = "SimpleCloud.SqlEdge.SqlExecutor";
                implementation.MethodName = "ExecuteSql";

                _executor = Edge.BindToLibrary(implementation);
            }
        }

        public Task<object> Sql(string command, object[] parameters) {
            EnsureExecutor();

            Dictionary<string, object> executionArgs = new Dictionary<string, object>();
            executionArgs["connectionString"] = _connectionString;
            executionArgs["command"] = command;
            executionArgs["parameters"] = Script.Or(parameters, null);
            if (_localFile) {
                executionArgs["localFile"] = true;
            }

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
