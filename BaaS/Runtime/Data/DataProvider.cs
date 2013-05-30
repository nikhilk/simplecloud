// DataProvider.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public abstract class DataProvider {

        private Dictionary<string, object> _configuration;

        protected Dictionary<string, object> Configuration {
            get {
                return _configuration;
            }
        }

        public virtual void Initialize(Dictionary<string, object> configuration) {
            _configuration = configuration;
        }
    }
}
