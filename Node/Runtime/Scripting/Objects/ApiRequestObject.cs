// ApiRequestObject.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using NodeApi.Network;
using SimpleCloud.Api;
using SimpleCloud.Server;

namespace SimpleCloud.Scripting.Objects {

    [ScriptObject]
    public sealed class ApiRequestObject {

        public HttpVerb Verb;
        public string ActionName;
        public Dictionary<string, string> Arguments;
        public Dictionary<string, string> Headers;
        public object Data;

        public Func<object, Dictionary<string, object>, Task<ServerResponse>> Respond;

        public ApiRequestObject(ApiRequest request) {
            Verb = request.Verb;
            ActionName = request.ActionName;
            Arguments = request.ActionArguments;
            Headers = request.Headers;
            Data = request.Data;

            Respond = delegate(object data, Dictionary<string, object> options) {
                options = Script.Or(options, new Dictionary<string, object>());

                HttpStatusCode statusCode = Script.Or((HttpStatusCode)options["statusCode"],
                                                      (data == null) ? HttpStatusCode.NoContent : HttpStatusCode.OK);

                ServerResponse response = new ServerResponse(statusCode);

                if (data != null) {
                    if (data is string) {
                        response.AddTextContent((string)data, Script.Or((string)options["contentType"], "text/plain"));
                    }
                    else {
                        response.AddObjectContent(data);
                    }
                }

                return Deferred.Create<ServerResponse>(response).Task;
            };
        }
    }
}
