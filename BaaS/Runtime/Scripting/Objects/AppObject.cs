// AppObject.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimpleCloud.Scripting.Objects {

    [ScriptObject]
    public sealed class AppObject {

        public Dictionary<string, object> Settings;
        public Dictionary<string, object> State;

        public AppObject(Application app) {
            Settings = app.Settings;
            State = new Dictionary<string, object>();
        }
    }
}
