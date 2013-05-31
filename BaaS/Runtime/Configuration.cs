// Configuration.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi;
using NodeApi.IO;

namespace SimpleCloud {

    public static class Configuration {

        private static object EvaluateExternalValue(object name, object value) {
            string s = value as string;

            if ((String.IsNullOrEmpty(s) == false) && s.StartsWith("%") && s.EndsWith("%")) {
                value = Node.Process.Environment[s.Substr(1, s.Length - 2)];
            }

            return value;
        }

        public static Dictionary<string, object> Load(string path, bool createEmptyIfNeeded) {
            try {
                if (FileSystem.ExistsSync(path)) {
                    string data = FileSystem.ReadFileTextSync(path, Encoding.UTF8);
                    return Json.ParseData<Dictionary<string, object>>(data, EvaluateExternalValue);
                }

                return createEmptyIfNeeded ? new Dictionary<string, object>() : null;
            }
            catch (Exception e) {
                Runtime.Abort("Error loading configuration from '%s' - %s", path, e.Message);
                return null;
            }
        }
    }
}
