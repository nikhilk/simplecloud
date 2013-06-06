// DataQuery.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataQuery {

        private readonly DataCollection _collection;
        private readonly string _id;

        private List<Function> _filters;
        private List<Function> _selectors;
        private List<Tuple<string, bool>> _sort;
        private int? _skip;
        private int? _take;

        internal DataQuery(DataCollection collection, string id) {
            _collection = collection;
            _id = id;

            _filters = null;
            _selectors = null;
            _sort = null;
            _skip = null;
            _take = null;
        }

        public DataCollection Collection {
            get {
                return _collection;
            }
        }

        public string ID {
            get {
                return _id;
            }
        }

        public bool IsLookup {
            get {
                return String.IsNullOrEmpty(_id) == false;
            }
        }

        private static DataQuery Clone(DataQuery original) {
            DataQuery clone = new DataQuery(original._collection, original._id);

            if (original._filters != null) {
                clone._filters = original._filters.Concat();
            }
            if (original._selectors != null) {
                clone._selectors = original._selectors.Concat();
            }
            if (original._sort != null) {
                clone._sort = original._sort.Concat();
            }
            if (original._skip.HasValue) {
                clone._skip = original._skip.Value;
            }
            if (original._take.HasValue) {
                clone._take = original._take.Value;
            }

            return clone;
        }

        public object[] Evaluate(object[] items) {
            // TODO: This is a very temporary implementation...

            if (_filters == null) {
                return items;
            }

            List<object> filteredItems = new List<object>();

            int filterCount = _filters.Count;

            for (int i = 0, itemCount = items.Length; i < itemCount; i++) {
                object item = items[i];
                bool match = true;
                for (int f = 0; f < filterCount; f++) {
                    if ((bool)_filters[f].Call(item) == false) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    filteredItems.Add(item);
                }
            }

            return filteredItems;
        }

        public DataQuery OrderBy(string field) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);
            if (query._sort == null) {
                query._sort = new List<Tuple<string, bool>>();
            }

            query._sort.Add(new Tuple<string, bool>(field, false));
            return query;
        }

        public DataQuery OrderByDescending(string field) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);
            if (query._sort == null) {
                query._sort = new List<Tuple<string, bool>>();
            }

            query._sort.Add(new Tuple<string, bool>(field, true));
            return query;
        }

        public DataQuery Select(Function selector) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            if (query._selectors == null) {
                query._selectors = new List<Function>();
            }

            query._selectors.Add(selector);
            return query;
        }

        public DataQuery Skip(int items) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            query._skip = items;
            return query;
        }

        public DataQuery Take(int items) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            query._take = items;
            return query;
        }

        public DataQuery Where(Function predicate) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            if (query._filters == null) {
                query._filters = new List<Function>();
            }

            query._filters.Add(predicate);
            return query;
        }
    }
}
