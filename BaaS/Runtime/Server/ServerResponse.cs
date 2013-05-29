// ServerResponse.cs
//

using System;
using NodeApi.Network;

namespace SimpleCloud.Server {

    public sealed class ServerResponse {

        private HttpStatusCode _statusCode;

        public ServerResponse(HttpStatusCode statusCode) {
            _statusCode = statusCode;
        }

        public HttpStatusCode StatusCode {
            get {
                return _statusCode;
            }
        }

        internal void Write(HttpServerResponse httpResponse) {
            httpResponse.WriteHead(_statusCode);
            httpResponse.End();
        }
    }
}
