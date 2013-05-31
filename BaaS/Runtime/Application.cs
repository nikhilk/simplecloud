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

        private ApplicationOptions _options;

        private Dictionary<string, object> _settings;
        private DataSpace _data;
        private ScriptManager _scripts;
        private ServerRuntime _runtime;

        public Application(ApplicationOptions options) {
            _options = options;

            _settings = GetConfigurationObject("settings");
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

        public Dictionary<string, object> Settings {
            get {
                return _settings;
            }
        }

        public Dictionary<string, object> GetConfigurationObject(string name) {
            string configPath = Path.Join(_options.Path, "config", name + ".json");
            return Configuration.Load(configPath, /* createEmptyIfNeeded */ true);
        }

        public static extern void ReportError(string error);

        public static void ReportError(string message, bool fatal) {
            fatal = Script.Value(fatal, true);

            if (fatal) {
                Console.Error(message);
                Node.Process.Abort();
            }
            else {
                Console.Warn(message);
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
