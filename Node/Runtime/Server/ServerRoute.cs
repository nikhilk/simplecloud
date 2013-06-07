// ServerRoute.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimpleCloud.Server {

    [ScriptImport]
    public sealed class ServerRoute {

        private ServerRoute() {
        }

        [ScriptField]
        [ScriptName("fn")]
        public IServerHandler Handler {
            get {
                return null;
            }
        }

        [ScriptField]
        [ScriptName("params")]
        public Dictionary<string, string> Parameters {
            get {
                return null;
            }
        }

        [ScriptField]
        [ScriptName("route")]
        public string Pattern {
            get {
                return null;
            }
        }

        [ScriptField]
        public string[] Splats {
            get {
                return null;
            }
        }
    }
}
