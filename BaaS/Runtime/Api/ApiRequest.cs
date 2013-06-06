// ApiRequest.cs
//

using System;
using System.Collections.Generic;
using NodeApi.Network;

namespace SimpleCloud.Api {

    public sealed class ApiRequest {

        private Controller _controller;
        private HttpVerb _verb;
        private string _actionName;
        private Dictionary<string, string> _headers;

        private Dictionary<string, string> _actionArgs;
        private object _data;

        public ApiRequest(Controller controller, HttpVerb verb, string actionName, Dictionary<string, string> headers, Dictionary<string, string> args) {
            _controller = controller;

            _verb = verb;
            _actionName = actionName;
            _headers = headers;

            _actionArgs = args;
        }

        public Dictionary<string, string> ActionArguments {
            get {
                return _actionArgs;
            }
        }

        public string ActionName {
            get {
                return _actionName;
            }
        }

        public Controller Controller {
            get {
                return _controller;
            }
        }

        public object Data {
            get {
                return _data;
            }
            set {
                _data = value;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return _headers;
            }
        }

        public HttpVerb Verb {
            get {
                return _verb;
            }
        }
    }
}
