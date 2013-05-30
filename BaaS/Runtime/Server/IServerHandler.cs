// IServerHandler.cs
//

using System;
using System.Threading;

namespace SimpleCloud.Server {

    public interface IServerHandler {

        string RoutePattern {
            get;
        }

        Task<ServerResponse> ProcessRequest(ServerRequest request);
    }
}
