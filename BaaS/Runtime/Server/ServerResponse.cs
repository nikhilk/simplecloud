// ServerResponse.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi.IO;
using NodeApi.Network;

namespace SimpleCloud.Server {

    public sealed class ServerResponse {

        public static readonly ServerResponse NoContent = new ServerResponse(HttpStatusCode.NoContent);
        public static readonly ServerResponse NotFound = new ServerResponse(HttpStatusCode.NotFound);
        public static readonly ServerResponse MethodNotAllowed = new ServerResponse(HttpStatusCode.MethodNotAllowed);
        public static readonly ServerResponse Conflict = new ServerResponse(HttpStatusCode.Conflict);

        private HttpStatusCode _statusCode;
        private Dictionary<string, string> _headers;

        private string _content;

        public ServerResponse(HttpStatusCode statusCode) {
            _statusCode = statusCode;
            _headers = new Dictionary<string, string>();
        }

        public HttpStatusCode StatusCode {
            get {
                return _statusCode;
            }
        }

        public static ServerResponse CreateRequestError(string message) {
            ServerResponse response = new ServerResponse(HttpStatusCode.BadRequest);

            Dictionary<string, object> error = new Dictionary<string, object>();
            error["message"] = message;

            return response.AddObjectContent(error);
        }

        public static ServerResponse CreateServerError(string message) {
            ServerResponse response = new ServerResponse(HttpStatusCode.InternalServerError);

            if (String.IsNullOrEmpty(message) == false) {
                Dictionary<string, object> error = new Dictionary<string, object>();
                error["message"] = message;

                response.AddObjectContent(error);
            }

            return response;
        }

        public ServerResponse AddObjectContent(object o) {
            _headers["Content-Type"] = "application/json";
            if (o == null) {
                _content = String.Empty;
            }
            else {
                _content = Json.Stringify(o);
            }

            return this;
        }

        public ServerResponse AddTextContent(string text, string contentType) {
            _content = text;
            _headers["Content-Type"] = contentType;

            return this;
        }

        internal void Write(HttpServerResponse httpResponse) {
            httpResponse.WriteHead(_statusCode, _headers);

            if (_content != null) {
                httpResponse.Write(_content, Encoding.UTF8);
            }

            httpResponse.End();
        }
    }
}
