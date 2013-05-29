// IServerHandler.cs
//

using System;
using System.Threading;

namespace SimpleCloud.Server {

    public interface IServerHandler {

        Task<ServerResponse> ProcessRequest(ServerRequest request);
    }
}
