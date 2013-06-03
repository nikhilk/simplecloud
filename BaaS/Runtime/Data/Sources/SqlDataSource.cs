// SqlDataProvider.cs
//

using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCloud.Data.Sources {

    public sealed class SqlDataSource : DataSource {

        public SqlDataSource(Application app, string name, Dictionary<string, object> configuration)
            : base(app, name, configuration) {
        }

        protected override Task<object> ExecuteNonQuery(DataRequest request, Dictionary<string, object> options) {
            return Deferred.Create<object>(Script.Undefined).Task;
        }

        protected override Task<object> ExecuteQuery(DataRequest request, Dictionary<string, object> options) {
            return Deferred.Create<object>(Script.Undefined).Task;
        }
    }
}
