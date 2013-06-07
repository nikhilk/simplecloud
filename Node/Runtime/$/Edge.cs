// Edge.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NodeApi.Interop {

    [ScriptImport]
    [ScriptDependency("edge")]
    [ScriptName("edge")]
    [ScriptIgnoreNamespace]
    public static class Edge {

        [ScriptName("func")]
        public static Action<object, AsyncResultCallback<object>> BindToCode(string code) {
            return null;
        }

        [ScriptName("func")]
        public static Action<object, AsyncResultCallback<object>> BindToLibrary(ManagedLibrary library) {
            return null;
        }

        [ScriptName("func")]
        public static Action<object, AsyncResultCallback<object>> BindToFile(string sourceFile) {
            return null;
        }
    }

    [ScriptImport]
    [ScriptObject]
    [ScriptName("Object")]
    public sealed class ManagedLibrary {

        public string AssemblyFile;
        public string TypeName;
        public string MethodName;

        public ManagedLibrary() {
        }

        public ManagedLibrary(params object[] nameValuePairs) {
        }
    }
}
