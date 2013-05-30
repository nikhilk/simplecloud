// Configuration.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi.IO;

namespace SimpleCloud {

    public static class Configuration {

        public static Dictionary<string, object> Load(string path, bool createEmptyIfNeeded) {
            if (FileSystem.ExistsSync(path)) {
                string data = FileSystem.ReadFileTextSync(path, Encoding.UTF8);
                return Json.ParseData<Dictionary<string, object>>(data);
            }

            return createEmptyIfNeeded ? new Dictionary<string, object>() : null;
        }
    }
}
