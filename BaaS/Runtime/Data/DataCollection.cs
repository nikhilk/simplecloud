// DataCollection.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataCollection {

        private string _name;
        private DataSource _source;
        private Dictionary<string, object> _configuration;

        public DataCollection(string name, DataSource source, Dictionary<string, object> configuration) {
            _name = name;
            _source = source;
            _configuration = configuration;
        }

        public string Name {
            get {
                return _name;
            }
        }
    }
}
