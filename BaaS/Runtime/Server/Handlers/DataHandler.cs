// DataHandler.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using SimpleCloud.Data;

namespace SimpleCloud.Server.Handlers {

    public sealed class DataHandler : IServerHandler {

        private DataSpace _dataSpace;

        public DataHandler(DataSpace dataSpace) {
            _dataSpace = dataSpace;
        }

        public string RoutePattern {
            get {
                return "/services/data/:dataCollection/:namedOperation?";
            }
        }

        public Task<ServerResponse> ProcessRequest(ServerRequest request) {
            return null;
        }
    }
}
