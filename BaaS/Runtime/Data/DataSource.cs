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

        public Task<object> Execute(DataRequest request, Dictionary<string, object> options) {
            if (String.IsNullOrEmpty(request.OperationName) == false) {
                return Deferred.Create<object>(Script.Undefined).Task;
            }

            if ((request.Operation == DataOperation.Lookup) || (request.Operation == DataOperation.Query)) {
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

        public virtual void Initialize(Application app, Dictionary<string, object> configuration) {
            _configuration = configuration;
        }
    }
}
