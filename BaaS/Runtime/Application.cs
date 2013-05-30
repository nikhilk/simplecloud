// Application.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi;
using NodeApi.IO;
using SimpleCloud.Data;
using SimpleCloud.Server;

namespace SimpleCloud {

    public sealed class Application {

        public static readonly Application Current = new Application();

        private ApplicationOptions _options;

        private DataSpace _data;
        private ScriptManager _scripts;
        private ServerRuntime _runtime;

        private Application() {
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

        public Dictionary<string, object> GetConfigurationObject(string name) {
            string configPath = Path.Join(_options.Path, "config", name + ".json");
            return Configuration.Load(configPath, /* createEmptyIfNeeded */ true);
        }

        public extern void ReportError(string error);

        public void ReportError(string message, bool fatal) {
            fatal = Script.Value(fatal, true);

            if (fatal) {
                Console.Error(message);
                Node.Process.Abort();
            }
            else {
                Console.Warn(message);
            }
        }

        public void Run(ApplicationOptions options) {
            _options = options;

            _data = new DataSpace();
            _scripts = new ScriptManager();

            List<IServerModule> modules = new List<IServerModule>();
            List<IServerHandler> handlers = new List<IServerHandler>();

            _runtime = new ServerRuntime(_options.Path, modules, handlers, _options.Log);
            _runtime.Run(_options.Port);
        }
    }
}
