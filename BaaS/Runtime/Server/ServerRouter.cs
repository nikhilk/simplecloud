// ServerrRoute.cs
//

using System;
using System.Runtime.CompilerServices;

namespace SimpleCloud.Server {

    [ScriptImport]
    [ScriptDependency("routes")]
    [ScriptName("Router")]
    public sealed class ServerRouter {

        public void AddRoute(string route, IServerHandler handler) {
        }

        public ServerRoute Match(string path) {
            return null;
        }
    }
}
