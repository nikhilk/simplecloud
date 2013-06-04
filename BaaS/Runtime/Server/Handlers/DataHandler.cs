// DataHandler.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi;
using NodeApi.Network;
using SimpleCloud.Data;

namespace SimpleCloud.Server.Handlers {

    /// <summary>
    /// Handles server requests associated with data access. Implements the HTTP interface
    /// over a set of data resources and fulfills those by performing data requests against
    /// a data collection.
    /// </summary>
    public sealed class DataHandler : IServerHandler {

        private DataSpace _dataSpace;

        public DataHandler(DataSpace dataSpace) {
            _dataSpace = dataSpace;
        }

        public string RoutePattern {
            get {
                return "/services/data/:collectionName/:operationName?";
            }
        }

        private DataRequest CreateDataRequest(ServerRequest serverRequest, DataCollection collection) {
            DataOperation operation;

            string id = serverRequest.UrlData.Query["id"];
            string partition = serverRequest.UrlData.Query["partition"];
            string operationName = serverRequest.Route.Parameters["operationName"];

            switch (serverRequest.HttpRequest.Method) {
                case HttpVerb.GET:
                    if (String.IsNullOrEmpty(id) == false) {
                        operation = DataOperation.Lookup;
                    }
                    else {
                        operation = DataOperation.Query;
                    }
                    break;
                case HttpVerb.POST:
                    if (String.IsNullOrEmpty(operationName)) {
                        operation = DataOperation.Update;
                    }
                    else {
                        operation = DataOperation.Execute;
                    }
                    break;
                case HttpVerb.PUT:
                    operation = DataOperation.Insert;
                    break;
                case HttpVerb.DELETE:
                    operation = DataOperation.Delete_;
                    break;
                default:
                    return null;
            }

            return new DataRequest(new DataQuery(collection, id, partition), operation, operationName);
        }

        private ServerResponse CreateServerResponse(DataRequest request, object result) {
            if (Script.IsUndefined(result)) {
                return new ServerResponse(HttpStatusCode.MethodNotAllowed);
            }
            else if (Script.IsNull(result)) {
                if (request.Operation == DataOperation.Query) {
                    return new ServerResponse(HttpStatusCode.OK).AddObjectContent(new object[0]);
                }
                else if (request.Operation == DataOperation.Insert) {
                    return ServerResponse.Conflict;
                }
                else {
                    return ServerResponse.NotFound;
                }
            }
            else {
                return new ServerResponse(HttpStatusCode.OK).AddObjectContent(result);
            }
        }

        public Task<ServerResponse> ProcessRequest(ServerRequest request) {
            string collectionName = request.Route.Parameters["collectionName"];
            DataCollection collection = _dataSpace.GetCollection(collectionName);

            if (collection == null) {
                return Deferred.Create<ServerResponse>(ServerResponse.NotFound).Task;
            }

            DataRequest dataRequest = CreateDataRequest(request, collection);
            if ((dataRequest == null) || (collection.SupportsRequest(dataRequest) == false)) {
                return Deferred.Create<ServerResponse>(ServerResponse.MethodNotAllowed).Task;
            }

            if ((dataRequest.Operation != DataOperation.Query) &&
                String.IsNullOrEmpty(dataRequest.Query.ID)) {
                ServerResponse response = ServerResponse.CreateRequestError("Missing id parameter.");
                return Deferred.Create<ServerResponse>(response).Task;
            }

            if ((dataRequest.Operation == DataOperation.Insert) ||
                (dataRequest.Operation == DataOperation.Update) ||
                (dataRequest.Operation == DataOperation.Merge)) {
                Deferred<ServerResponse> deferred = Deferred.Create<ServerResponse>();

                request.GetData().ContinueWith(delegate(Task<object> dataTask) {
                    if (dataTask.Status == TaskStatus.Failed) {
                        deferred.Reject(dataTask.Error);
                        return;
                    }

                    Task<ServerResponse> executeTask = null;
                    try {
                        dataRequest.SetItem((Dictionary<string, object>)dataTask.Result);
                        executeTask = ExecuteRequest(collection, dataRequest);
                    }
                    catch (Exception e) {
                        deferred.Resolve(ServerResponse.CreateServerError(e.Message));
                        return;
                    }

                    executeTask.ContinueWith(delegate(Task<ServerResponse> t) {
                        if (t.Status == TaskStatus.Done) {
                            deferred.Resolve(t.Result);
                        }
                        else {
                            deferred.Resolve(ServerResponse.CreateServerError(t.Error.Message));
                        }
                    });
                });

                return deferred.Task;
            }
            else {
                return ExecuteRequest(collection, dataRequest);
            }
        }

        private Task<ServerResponse> ExecuteRequest(DataCollection collection, DataRequest request) {
            Deferred<ServerResponse> deferred = Deferred.Create<ServerResponse>();
            Task<object> resultTask = collection.ExecuteRequest(request);

            resultTask.Done(delegate(object result) {
                deferred.Resolve(CreateServerResponse(request, result));
            })
            .Fail(delegate(Exception e) {
                deferred.Resolve(ServerResponse.CreateServerError(e.Message));
            });

            return deferred.Task;
        }
    }
}
