// DataCollection.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi.IO;
using SimpleCloud.Scripting.Objects;

namespace SimpleCloud.Data {

    public sealed class DataCollection {

        private string _name;
        private DataSource _source;
        private string _path;
        private Dictionary<string, object> _configuration;

        public DataCollection(string name, string path, Dictionary<string, object> configuration, DataSource source) {
            _name = name;
            _path = path;
            _configuration = configuration;
            _source = source;
        }

        public string Name {
            get {
                return _name;
            }
        }

        public DataSource Source {
            get {
                return _source;
            }
        }

        public Task<object> ExecuteRequest(DataRequest request) {
            DataRequestObject requestObject = new DataRequestObject(request);

            object result = _source.Application.Scripts.Execute(_path, GetScriptPath(request), "request", requestObject);
            if (Script.IsNullOrUndefined(result) == false) {
                Task<object> taskResult = result as Task<object>;
                if (taskResult != null) {
                    return taskResult;
                }
            }

            return Deferred.Create<object>(result).Task;
        }

        private string GetScriptPath(DataRequest request) {
            string scriptName = null;

            switch (request.Operation) {
                case DataOperation.Lookup:
                    scriptName = "lookup";
                    break;
                case DataOperation.Query:
                    scriptName = String.IsNullOrEmpty(request.OperationName) ? "query" : "query." + request.OperationName;
                    break;
                case DataOperation.Insert:
                    scriptName = "insert";
                    break;
                case DataOperation.Update:
                    scriptName = "update";
                    break;
                case DataOperation.Merge:
                    // TODO: If update behaves as upsert, then this should just be update
                    scriptName = "merge";
                    break;
                case DataOperation.Delete_:
                    scriptName = "delete";
                    break;
                case DataOperation.Execute:
                    scriptName = request.OperationName;
                    break;
            }

            return Path.Join(_path, scriptName + ".js");
        }

        public bool SupportsRequest(DataRequest request) {
            // TODO: Support operations enabled via config
            return FileSystem.ExistsSync(GetScriptPath(request));
        }
    }
}
