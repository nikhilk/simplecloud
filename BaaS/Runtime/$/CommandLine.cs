// CommandLine.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NodeApi {

    [ScriptImport]
    [ScriptDependency("node-commandline", Identifier = "cmd")]
    public static class CommandLine {

        public static object Parse(CommandModel commandModel) {
            return null;
        }
    }

    [ScriptImport]
    [ScriptDependency("node-commandline", Identifier = "cmd")]
    [ScriptName("CommandLine")]
    public sealed class CommandModel {

        public CommandModel(string name) {
        }

        public CommandModel AddArgument(string name, CommandArgument options) {
            return null;
        }

        public CommandArgument GetArgument(string name) {
            return null;
        }

        public Dictionary<string, object> Parse(params string[] args) {
            return null;
        }

        public Dictionary<string, object> ParseNode(params string[] args) {
            return null;
        }
    }

    [ScriptImport]
    [ScriptIgnoreNamespace]
    [ScriptName("Object")]
    public sealed class CommandArgument {

        public CommandArgument() {
        }

        public CommandArgument(params object[] nameValuePairs) {
        }

        public object[] AllowedValues {
            get {
                return null;
            }
            set {
            }
        }

        public int Order {
            get {
                return 0;
            }
            set {
            }
        }

        public bool Required {
            get {
                return false;
            }
            set {
            }
        }

        public bool Sequenced {
            get {
                return false;
            }
            set {
            }
        }

        public string Type {
            get {
                return null;
            }
            set {
            }
        }
    }
}
