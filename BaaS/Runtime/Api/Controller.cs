// Controller.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi.IO;
using NodeApi.Network;
using SimpleCloud.Scripting.Objects;
using SimpleCloud.Server;

namespace SimpleCloud.Api {

    public sealed class Controller {

        private Application _app;
        private string _name;
        private string _path;
        private Dictionary<string, object> _configurtion;

        public Controller(Application app, string name, string path, Dictionary<string, object> configuration) {
            _app = app;
            _name = name;
            _path = path;
            _configurtion = configuration;
        }

        public Task<object> ExecuteRequest(ApiRequest request) {
            ApiRequestObject requestObject = new ApiRequestObject(request);

            object result = _app.Scripts.Execute(_path, GetScriptPath(request), "request", requestObject);
            if (Script.IsNullOrUndefined(result) == false) {
                Task<object> taskResult = result as Task<object>;
                if (taskResult != null) {
                    return taskResult;
                }
            }

            return Deferred.Create<object>(result).Task;
        }

        private string GetScriptPath(ApiRequest request) {
            string scriptName = null;

            switch (request.Verb) {
                case HttpVerb.GET:
                    scriptName = String.IsNullOrEmpty(request.ActionName) ? "get" : "get" + request.ActionName;
                    break;
                case HttpVerb.POST:
                    scriptName = String.IsNullOrEmpty(request.ActionName) ? "post" : request.ActionName;
                    break;
                case HttpVerb.PUT:
                    scriptName = "put";
                    break;
                case HttpVerb.DELETE:
                    scriptName = "delete";
                    break;
            }

            return Path.Join(_path, scriptName + ".js");
        }

        public bool SupportsRequest(ApiRequest request) {
            // TODO: Support actions enabled via config
            return FileSystem.ExistsSync(GetScriptPath(request));
        }
    }
}
