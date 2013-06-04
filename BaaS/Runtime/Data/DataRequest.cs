// DataRequest.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataRequest {

        private readonly DataOperation _operation;
        private readonly string _operationName;
        private readonly Dictionary<string, string> _operationArgs;

        private string _partition;
        private DataQuery _query;
        private Dictionary<string, object> _item;

        public DataRequest(DataOperation operation, string operationName, Dictionary<string, string> args) {
            _operation = operation;
            _operationName = operationName;
            _operationArgs = args;
        }

        public Dictionary<string, object> Item {
            get {
                return _item;
            }
            set {
                _item = value;
            }
        }

        public DataOperation Operation {
            get {
                return _operation;
            }
        }

        public Dictionary<string, string> OperationArguments {
            get {
                return _operationArgs;
            }
        }

        public string OperationName {
            get {
                return _operationName;
            }
        }

        public string Partition {
            get {
                return _partition;
            }
            set {
                _partition = value;
            }
        }

        public DataQuery Query {
            get {
                return _query;
            }
            set {
                _query = value;
            }
        }
    }
}
