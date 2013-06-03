// DataOperation.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public enum DataOperation {

        Lookup = 0,

        Query = 1,

        Insert = 2,

        Update = 3,

        Merge = 4,

        Delete_ = 5,

        Execute = 6
    }
}
