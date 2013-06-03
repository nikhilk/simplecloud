// ScriptManager.cs
//

using System;
using System.Collections.Generic;
using NodeApi.Compute;
using NodeApi.IO;
using SimpleCloud.Scripting.Objects;

namespace SimpleCloud.Scripting {

    public sealed class ScriptManager {

        // TODO: Add caching of script instances along with file-watcher-based invalidation

        private Application _app;

        private AppObject _appObject;
        private DataObject _dataObject;

        public ScriptManager(Application app) {
            _app = app;

            _appObject = new AppObject(_app);
            _dataObject = new DataObject(_app.Data);
        }

        public object Execute(string scopePath, string scriptPath, string contextMember, object contextObject) {
            ScriptInstance script = null;
            string scriptCode = String.Empty;

            if (FileSystem.ExistsSync(scriptPath)) {
                scriptCode = FileSystem.ReadFileTextSync(scriptPath, Encoding.UTF8);

                string sharedScriptPath = Path.Join(scopePath, "_shared.js");
                if (FileSystem.ExistsSync(sharedScriptPath)) {
                    scriptCode = FileSystem.ReadFileTextSync(sharedScriptPath, Encoding.UTF8) + "\r\n" + scriptCode;
                }

                script = ScriptEngine.CreateScript(scriptCode);
            }

            if (script != null) {
                Dictionary<string, object> context = new Dictionary<string, object>();
                context["app"] = _appObject;
                context["data"] = _dataObject;

                if (String.IsNullOrEmpty(contextMember) == false) {
                    context[contextMember] = contextObject;
                }

                return script.RunInNewContext(context);
            }

            return Script.Undefined;
        }
    }
}
