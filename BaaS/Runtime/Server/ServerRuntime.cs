// ServerRuntime.cs
//

using System;
using System.Collections.Generic;
using System.Threading;
using NodeApi;
using NodeApi.IO;
using NodeApi.Network;

namespace SimpleCloud.Server {

    public sealed class ServerRuntime : IServerModule {

        private string _path;
        private List<IServerModule> _modules;
        private ServerRouter _router;

        private HttpServer _httpServer;

        public ServerRuntime(string path, List<IServerModule> modules, List<IServerHandler> handlers) {
            _path = path;
            _modules = modules;

            _router = new ServerRouter();
            foreach (IServerHandler handler in handlers) {
                _router.AddRoute(handler.RoutePattern, handler);
            }

            _modules.Add(this);
            for (int i = 0, moduleCount = _modules.Count; i < moduleCount - 2; i++) {
                _modules[i].InitializeModule(_modules[i + 1]);
            }

            _httpServer = Http.CreateServer();
            _httpServer.Request += OnHttpServerRequest;
        }

        public string Path {
            get {
                return _path;
            }
        }

        public void InitializeModule(IServerModule nextModule) {
            // The runtime does not have a next module - it is always at the very end of the
            // module pipeline, so there is nothing to do here.
        }

        private void OnHttpServerRequest(HttpServerRequest httpRequest, HttpServerResponse httpResponse) {
            UrlData urlData = Url.Parse(httpRequest.Url, /* parseQueryString */ true);
            ServerRoute route = _router.Match(urlData.Path);

            ServerRequest request = new ServerRequest(httpRequest, urlData, route);
            Task<ServerResponse> responseTask = _modules[0].ProcessRequest(request);

            responseTask.Done(delegate(ServerResponse response) {
                response.Write(httpResponse);
                
                Runtime.TraceInfo("%d : %s %s", response.StatusCode, httpRequest.Method, httpRequest.Url);
            })
            .Fail(delegate(Exception e) {
                httpResponse.WriteHead(HttpStatusCode.InternalServerError, "Internal Server Error");
                httpResponse.End();

                Runtime.TraceInfo("500 : %s %s", httpRequest.Method, httpRequest.Url);
            });
        }

        public Task<ServerResponse> ProcessRequest(ServerRequest request) {
            // The runtime is a server module, which is logically at the end of the module
            // pipeline (i.e. doesn't wrap another module), and it is responsible for executing
            // the handler associated with the current route, or generating a not found response.

            IServerHandler handler = null;

            ServerRoute route = request.Route;
            if (route != null) {
                handler = route.Handler;
            }

            if (handler != null) {
                return handler.ProcessRequest(request);
            }
            else {
                ServerResponse notFoundResponse = request.CreateResponse(HttpStatusCode.NotFound);
                return Deferred.Create<ServerResponse>(notFoundResponse).Task;
            }
        }

        public void Run(int port) {
            _httpServer.Listen(port);
            Runtime.TraceAlert("Started HTTP server. Listening on http://localhost:%d", port);
        }
    }
}
