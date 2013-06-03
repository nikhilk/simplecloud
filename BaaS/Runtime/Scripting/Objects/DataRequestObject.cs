// DataRequestObject.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SimpleCloud.Data;

namespace SimpleCloud.Scripting.Objects {

    [ScriptObject]
    public sealed class DataRequestObject {

        public DataOperation Operation;
        public string OperationName;

        public string ID;
        public string Partition;
        public DataQuery Query;
        public object Item;

        public Func<Dictionary<string, object>, Task<object>> Execute;

        public DataRequestObject(DataRequest request) {
            Operation = request.Operation;
            OperationName = request.OperationName;

            ID = request.Query.ID;
            Partition = request.Query.Partition;
            Query = request.Query;
            Item = request.Item;

            Execute = delegate(Dictionary<string, object> options) {
                return request.Query.Collection.Source.Execute(request, options);
            };
        }
    }
}
