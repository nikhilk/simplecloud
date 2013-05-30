// Application.cs
//

using System;
using System.Collections.Generic;
using SimpleCloud.Data;
using SimpleCloud.Server;

namespace SimpleCloud {

    public sealed class Application {

        private ApplicationOptions _options;

        private DataSpace _data;
        private ScriptManager _scripts;

        private ServerRuntime _runtime;

        public Application(ApplicationOptions options) {
            _options = options;

            _data = new DataSpace(this);
            _scripts = new ScriptManager(this);
        }

        public DataSpace Data {
            get {
                return _data;
            }
        }

        public ApplicationOptions Options {
            get {
                return _options;
            }
        }

        public ScriptManager Scripts {
            get {
                return _scripts;
            }
        }

        public void Run() {
            List<IServerModule> modules = new List<IServerModule>();
            List<IServerHandler> handlers = new List<IServerHandler>();

            _runtime = new ServerRuntime(_options.Path, modules, handlers, _options.Log);
            _runtime.Run(_options.Port);
        }
    }
}
