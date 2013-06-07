// ApiHandler.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi.Network;
using SimpleCloud.Api;

namespace SimpleCloud.Server.Handlers {

    public sealed class ApiHandler : IServerHandler {

        private Endpoints _endpoints;

        public ApiHandler(Endpoints endpoints) {
            _endpoints = endpoints;
        }

        public string RoutePattern {
            get {
                return "/services/:controllerName/:actionName?";
            }
        }

        private ServerResponse CreateServerResponse(ApiRequest request, object result) {
            ServerResponse serverResponse = result as ServerResponse;
            if (serverResponse != null) {
                return serverResponse;
            }

            if (Script.IsUndefined(result)) {
                return new ServerResponse(HttpStatusCode.MethodNotAllowed);
            }
            else if (Script.IsNull(result)) {
                if (request.Verb == HttpVerb.GET) {
                    return new ServerResponse(HttpStatusCode.OK).AddObjectContent(new object());
                }
                else {
                    return ServerResponse.NoContent;
                }
            }

            return new ServerResponse(HttpStatusCode.OK).AddObjectContent(result);
        }

        private Task<ServerResponse> ExecuteRequest(Controller controller, ApiRequest request) {
            Deferred<ServerResponse> deferred = Deferred.Create<ServerResponse>();
            Task<object> resultTask = controller.ExecuteRequest(request);

            resultTask.Done(delegate(object result) {
                deferred.Resolve(CreateServerResponse(request, result));
            })
            .Fail(delegate(Exception e) {
                deferred.Resolve(ServerResponse.CreateServerError(e.Message));
            });

            return deferred.Task;
        }

        public Task<ServerResponse> ProcessRequest(ServerRequest request) {
            string controllerName = request.Route.Parameters["controllerName"];
            Controller controller = _endpoints.GetController(controllerName);

            if (controller == null) {
                return Deferred.Create<ServerResponse>(ServerResponse.NotFound).Task;
            }

            ApiRequest apiRequest = new ApiRequest(controller, request.HttpRequest.Method, request.Route.Parameters["actionName"],
                                                   (Dictionary<string, string>)request.HttpRequest.Headers,
                                                   request.UrlData.Query);

            if (controller.SupportsRequest(apiRequest) == false) {
                return Deferred.Create<ServerResponse>(ServerResponse.MethodNotAllowed).Task;
            }

            if ((request.HttpRequest.Method == HttpVerb.POST) || (request.HttpRequest.Method == HttpVerb.PUT)) {
                Deferred<ServerResponse> deferred = Deferred.Create<ServerResponse>();

                request.GetData().ContinueWith(delegate(Task<object> dataTask) {
                    if (dataTask.Status == TaskStatus.Failed) {
                        deferred.Reject(dataTask.Error);
                        return;
                    }

                    Task<ServerResponse> executeTask;
                    try {
                        apiRequest.Data = dataTask.Result;
                        executeTask = ExecuteRequest(controller, apiRequest);
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
                return ExecuteRequest(controller, apiRequest);
            }
        }
    }
}
