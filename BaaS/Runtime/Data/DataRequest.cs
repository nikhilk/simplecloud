// DataRequest.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataRequest {

        private DataQuery _query;

        private DataOperation _operation;
        private string _operationName;

        private Dictionary<string, object> _item;

        public DataRequest(DataQuery query, DataOperation operation, string operationName) {
            _query = query;

            _operation = operation;
            _operationName = operationName;
        }

        public Dictionary<string, object> Item {
            get {
                return _item;
            }
        }

        public DataOperation Operation {
            get {
                return _operation;
            }
        }

        public string OperationName {
            get {
                return _operationName;
            }
        }

        public DataQuery Query {
            get {
                return _query;
            }
        }

        public void SetItem(Dictionary<string, object> item) {
            _item = item;
        }
    }
}
