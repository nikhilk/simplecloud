// DataSource.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataSource {

        private string _name;
        private DataProvider _provider;

        public DataSource(string name, DataProvider provider) {
            _name = name;
            _provider = provider;
        }

        public string Name {
            get {
                return _name;
            }
        }
    }
}
