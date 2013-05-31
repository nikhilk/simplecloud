// Runtime.cs
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NodeApi {

    [ScriptImport]
    [ScriptIgnoreNamespace]
    public static class Runtime {

        [ScriptAlias("_traceEnabled")]
        [ScriptField]
        public static bool EnableTrace {
            get {
                return false;
            }
            set {
            }
        }

        [ScriptAlias("_abort")]
        public static void Abort(string message, params object[] values) {
        }

        [ScriptAlias("_traceAlert")]
        public static void TraceAlert(string message, params object[] values) {
        }

        [ScriptAlias("_traceError")]
        public static void TraceError(string message, params object[] values) {
        }

        [ScriptAlias("_traceInfo")]
        public static void TraceInfo(string message, params object[] values) {
        }

        [ScriptAlias("_traceObject")]
        public static void TraceObject(string name, object o) {
        }

        [ScriptAlias("_traceWarn")]
        public static void TraceWarning(string message, params object[] values) {
        }
    }
}
