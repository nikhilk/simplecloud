// Endpoints.cs
//

using System;
using System.Collections.Generic;
using NodeApi.IO;

namespace SimpleCloud.Api {

    public sealed class Endpoints {

        private Application _app;

        private Dictionary<string, Controller> _controllers;

        public Endpoints(Application app) {
            _app = app;

            _controllers = new Dictionary<string, Controller>();
            Load();
        }

        public Controller GetController(string name) {
            return _controllers[name];
        }

        private void Load() {
            Runtime.TraceInfo("Loading API Endpoints...");

            string apiPath = Path.Join(_app.Options.Path, "api");

            // TODO: Also check if its a directory
            if (FileSystem.ExistsSync(apiPath) == false) {
                Runtime.TraceWarning("API directory '%s' was not found. No actions were loaded.", apiPath);
                return;
            }

            foreach (string name in FileSystem.ReadDirectorySync(apiPath)) {
                string controllerPath = Path.Join(apiPath, name);
                string controllerConfigPath = Path.Join(controllerPath, "config.json");

                Dictionary<string, object> controllerConfig = Configuration.Load(controllerConfigPath, /* createEmptyIfNeeded */ false);
                if (controllerConfig != null) {
                    _controllers[name] = new Controller(_app, name, controllerPath, controllerConfig);

                    Runtime.TraceInfo("Created controller '%s'.", name);
                }
                else {
                    Runtime.TraceError("Configuration not found in action directory named '%s'. Ignoring.", name);
                }
            }
        }
    }
}
