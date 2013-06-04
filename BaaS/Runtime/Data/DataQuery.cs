// DataQuery.cs
//

using System;
using System.Collections.Generic;

namespace SimpleCloud.Data {

    public sealed class DataQuery {

        private readonly DataCollection _collection;
        private readonly string _id;

        private List<Func<object, bool>> _filters;
        private List<Func<object, object>> _selectors;
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

        public DataQuery Select(Func<object, object> selector) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            if (_selectors == null) {
                _selectors = new List<Func<object, object>>();
            }

            _selectors.Add(selector);
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

        public DataQuery Where(Func<object, bool> predicate) {
            if (IsLookup) {
                return this;
            }

            DataQuery query = DataQuery.Clone(this);

            if (_filters == null) {
                _filters = new List<Func<object, bool>>();
            }

            _filters.Add(predicate);
            return query;
        }
    }
}
