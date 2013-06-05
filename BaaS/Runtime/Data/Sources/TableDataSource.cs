// TableDataSource.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi.WindowsAzure;
using NodeApi.WindowsAzure.Storage;

namespace SimpleCloud.Data.Sources {

    public sealed class TableDataSource : DataSource {

        private CloudTableService _tableService;

        public TableDataSource(Application app, string name, Dictionary<string, object> configuration)
            : base(app, name, configuration) {
            string storageAccount = (string)configuration["storageAccount"];
            if (String.IsNullOrEmpty(storageAccount)) {
                Runtime.Abort("No storageAccount was specified in the configuration for the '%s' data source.", name);
            }

            string accessKey = (string)configuration["accessKey"];
            if (String.IsNullOrEmpty(accessKey)) {
                Runtime.Abort("No accessKey was specified in the configuration for the '%s' data source.", name);
            }

            _tableService = Azure.CreateTableService(storageAccount, accessKey);
        }

        private void CleanEntity(CloudTableEntity entity) {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata["etag"] = Script.GetField(Script.GetField(entity, "_"), "etag");
            metadata["timestamp"] = Script.GetField(entity, "Timestamp");
            metadata["partition"] = entity.PartitionKey;

            Script.SetField(entity, "_", metadata);
            Script.SetField(entity, "id", entity.RowKey);

            Script.DeleteField(entity, "Timestamp");
            Script.DeleteField(entity, "PartitionKey");
            Script.DeleteField(entity, "RowKey");
        }

        protected override Task<object> ExecuteNonQuery(DataRequest request, Dictionary<string, object> options) {
            DataQuery query = request.Query;
            string tableName = request.Query.Collection.Name;

            if (String.IsNullOrEmpty(request.Partition)) {
                throw new Exception("Missing partition information to execute request.");
            }

            CloudTableEntity entity;
            if (request.Item != null) {
                entity = (CloudTableEntity)request.Item;
                Script.DeleteField(entity, "id");
                Script.DeleteField(entity, "_");
            }
            else {
                entity = new CloudTableEntity();
            }
            entity.PartitionKey = request.Partition;
            entity.RowKey = query.ID;

            Deferred<object> deferred = Deferred.Create<object>();

            if (request.Operation == DataOperation.Insert) {
                _tableService.InsertEntity(tableName, entity, delegate(Exception e, CloudTableEntity insertedEntity) {
                    if (e != null) {
                        deferred.Reject(e);
                    }
                    else {
                        deferred.Resolve(true);
                    }
                });
            }
            else if (request.Operation == DataOperation.Update) {
                _tableService.MergeEntity(tableName, entity, delegate(Exception e, CloudTableEntity updatedEntity) {
                    if (e != null) {
                        deferred.Reject(e);
                    }
                    else {
                        deferred.Resolve(true);
                    }
                });
            }
            else if (request.Operation == DataOperation.Delete_) {
                _tableService.DeleteEntity(tableName, entity, delegate(Exception e, bool successful) {
                    deferred.Resolve((e == null) && successful);
                });
            }
            else {
                deferred.Resolve(Script.Undefined);
            }

            return deferred.Task;
        }

        protected override Task<object> ExecuteQuery(DataRequest request, Dictionary<string, object> options) {
            DataQuery query = request.Query;
            string tableName = request.Query.Collection.Name;

            Deferred<object> deferred = Deferred.Create<object>();

            if (query.IsLookup) {
                if (String.IsNullOrEmpty(request.Partition)) {
                    throw new Exception("Missing partition information to perform lookup.");
                }

                Runtime.TraceInfo("Querying table service pk = %s, rk = %s", request.Partition, query.ID);

                _tableService.QueryEntity(tableName, request.Partition, query.ID, delegate(Exception e, CloudTableEntity entity) {
                    if (e != null) {
                        deferred.Resolve(null);
                    }
                    else {
                        CleanEntity(entity);
                        deferred.Resolve(entity);
                    }
                });
            }
            else {
                // TODO: Apply actual query
                CloudTableQuery tableQuery = new CloudTableQuery().From(tableName);
                if (String.IsNullOrEmpty(request.Partition) == false) {
                    tableQuery = tableQuery.WhereKeys(request.Partition, null);
                }

                _tableService.QueryEntities(tableQuery, delegate(Exception e, List<CloudTableEntity> entities) {
                    entities.ForEach(CleanEntity);
                    object[] items = query.Evaluate((object[])entities);

                    deferred.Resolve(items);
                });
            }

            return deferred.Task;
        }

        public override object GetService(Dictionary<string, object> options) {
            return _tableService;
        }
    }
}
