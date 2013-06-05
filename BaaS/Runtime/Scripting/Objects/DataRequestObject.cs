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
        public Dictionary<string, string> Arguments;

        public string ID;

        public string Partition;
        public DataQuery Query;
        public Dictionary<string, object> Item;

        public Func<Dictionary<string, object>, Task<object>> Execute;

        public DataRequestObject(DataRequest request) {
            Operation = request.Operation;
            OperationName = request.OperationName;
            Arguments = request.OperationArguments;

            ID = request.Query.ID;

            Partition = request.Partition;
            Query = request.Query;
            Item = request.Item;

            Execute = delegate(Dictionary<string, object> options) {
                request.Partition = Partition;
                request.Query = Query;
                request.Item = Item;

                return request.Query.Collection.Source.Execute(request, options);
            };
        }
    }
}
