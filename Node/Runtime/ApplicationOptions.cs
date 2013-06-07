// ApplicationOptions.cs
//

using System;
using System.Runtime.CompilerServices;

namespace SimpleCloud {

    [ScriptImport]
    [ScriptObject]
    [ScriptName("Object")]
    public sealed class ApplicationOptions {

        public bool Logs;
        public string Path;
        public int Port;
    }
}
