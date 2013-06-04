// DataObject.cs
//

using System;
using System.Collections.Generic;
using SimpleCloud.Data;

namespace SimpleCloud.Scripting.Objects {

    [ScriptObject]
    public sealed class DataObject {

        public Func<string, Dictionary<string, object>, object> Sources;

        public DataObject(DataSpace dataSpace) {
            Sources = delegate(string name, Dictionary<string, object> options) {
                DataSource source = dataSpace.GetSource(name);
                if (source != null) {
                    return source.GetService(options);
                }

                return null;
            };
        }
    }
}
