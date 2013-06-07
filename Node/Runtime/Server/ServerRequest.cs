// ServerRequest.cs
//

using System;
using System.Serialization;
using System.Threading;
using NodeApi.IO;
using NodeApi.Network;

namespace SimpleCloud.Server {

    public sealed class ServerRequest {

        private HttpServerRequest _httpRequest;

        private UrlData _urlData;
        private ServerRoute _route;

        private object _data;
        private Exception _dataException;

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

        public Task<object> GetData() {
            if ((_httpRequest.Method != HttpVerb.POST) && (_httpRequest.Method != HttpVerb.PUT)) {
                throw new Exception("No data for requests without a body.");
            }

            if (Script.IsUndefined(_data) == false) {
                return Deferred.Create<object>(_data).Task;
            }

            Deferred<object> deferred = Deferred.Create<object>();
            if (_dataException != null) {
                deferred.Reject(_dataException);
            }
            else {
                string content = String.Empty;

                _httpRequest.SetEncoding(Encoding.UTF8);
                _httpRequest.Data += delegate(string chunk) {
                    content += chunk;
                };
                _httpRequest.End += delegate() {
                    try {
                        _data = String.IsNullOrEmpty(content) ? null : Json.Parse(content);
                        deferred.Resolve(_data);
                    }
                    catch (Exception e) {
                        _dataException = e;
                        deferred.Reject(new Exception("Invalid JSON in request body. " + e.Message));
                    }
                };
            }

            return deferred.Task;
        }
    }
}
