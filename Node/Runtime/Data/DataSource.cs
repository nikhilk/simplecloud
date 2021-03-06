// DataSource.cs
//

using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCloud.Data {

    public abstract class DataSource {

        private Application _app;
        private string _name;
        private Dictionary<string, object> _configuration;

        public DataSource(Application app, string name, Dictionary<string, object> configuration) {
            _app = app;
            _name = name;
            _configuration = configuration;
        }

        public Application Application {
            get {
                return _app;
            }
        }

        protected Dictionary<string, object> Configuration {
            get {
                return _configuration;
            }
        }

        protected string Name {
            get {
                return _name;
            }
        }

        public Task<object> Execute(DataRequest request, Dictionary<string, object> options) {
            if (options != null) {
                object result = options["result"];
                if (Script.IsUndefined(result) == false) {
                    return Deferred.Create<object>(result).Task;
                }
            }

            if (request.Operation == DataOperation.Execute) {
                return Deferred.Create<object>(Script.Undefined).Task;
            }
            else if ((request.Operation == DataOperation.Lookup) || (request.Operation == DataOperation.Query)) {
                return ExecuteQuery(request, options);
            }
            else {
                return ExecuteNonQuery(request, options);
            }
        }

        protected virtual Task<object> ExecuteNonQuery(DataRequest request, Dictionary<string, object> options) {
            return Deferred.Create<object>(Script.Undefined).Task;
        }

        protected abstract Task<object> ExecuteQuery(DataRequest request, Dictionary<string, object> options);

        public virtual object GetService(Dictionary<string, object> options) {
            return null;
        }
    }
}
