// ServerRequest.cs
//

using System;
using NodeApi.Network;

namespace SimpleCloud.Server {

    public sealed class ServerRequest {

        private HttpServerRequest _httpRequest;

        private UrlData _urlData;
        private ServerRoute _route;

        internal ServerRequest(HttpServerRequest httpRequest, UrlData urlData, ServerRoute route) {
            _httpRequest = httpRequest;

            _urlData = urlData;
            _route = route;
        }

        public HttpServerRequest HttpRequest {
            get {
                return _httpRequest;
            }
        }

        public ServerRoute Route {
            get {
                return _route;
            }
        }

        public UrlData UrlData {
            get {
                return _urlData;
            }
        }
    }
}
