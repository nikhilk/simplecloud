// LocalDataSource.cs
//

using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCloud.Data.Sources {

    public sealed class LocalDataSource : DataSource {

        private Dictionary<string, Dictionary<string, Dictionary<string, object>>> _data;

        public LocalDataSource(Application app, string name, Dictionary<string, object> configuration)
            : base(app, name, configuration) {
            _data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
        }

        protected override Task<object> ExecuteNonQuery(DataRequest request, Dictionary<string, object> options) {
            Dictionary<string, Dictionary<string, object>> set = GetDataSet(request.Query);
            string id = request.Query.ID;

            object result = null;
            if (request.Operation == DataOperation.Insert) {
                if (set.ContainsKey(id) == false) {
                    Dictionary<string, object> item = request.Item;
                    item["id"] = id;

                    set[id] = item;
                    result = item;
                }
            }
            else if (request.Operation == DataOperation.Update) {
                if (set.ContainsKey(id)) {
                    Dictionary<string, object> item = request.Item;
                    item["id"] = id;

                    set[id] = item;
                    result = item;
                }
            }
            else if (request.Operation == DataOperation.Merge) {
                if (set.ContainsKey(id)) {
                    Dictionary<string, object> existingItem = set[id];
                    Dictionary<string, object> updatedItem = request.Item;

                    foreach (KeyValuePair<string, object> fieldEntry in updatedItem) {
                        existingItem[fieldEntry.Key] = fieldEntry.Value;
                    }
                    existingItem["id"] = id;

                    result = existingItem;
                }
            }
            else if (request.Operation == DataOperation.Delete_) {
                if (set.ContainsKey(id)) {
                    set.Remove(id);
                    result = true;
                }
            }

            return Deferred.Create<object>(result).Task;
        }

        protected override Task<object> ExecuteQuery(DataRequest request, Dictionary<string, object> options) {
            DataQuery query = request.Query;
            Dictionary<string, Dictionary<string, object>> set = GetDataSet(query);

            object result;
            if (query.IsLookup) {
                result = Script.Or(set[query.ID], null);
            }
            else {
                // TODO: Apply query

                List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
                foreach (KeyValuePair<string, Dictionary<string, object>> itemEntry in set) {
                    items.Add(itemEntry.Value);
                }

                result = items;
            }

            return Deferred.Create<object>(result).Task;
        }

        private Dictionary<string, Dictionary<string, object>> GetDataSet(DataQuery query) {
            string setName = query.Collection.Name;

            Dictionary<string, Dictionary<string, object>> set = _data[setName];
            if (set == null) {
                set = new Dictionary<string, Dictionary<string, object>>();
                _data[setName] = set;
            }

            return set;
        }
    }
}
