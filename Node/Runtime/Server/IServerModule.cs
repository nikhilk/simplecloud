// IServerModule.cs
//

using System;
using System.Threading;

namespace SimpleCloud.Server {

    public interface IServerModule {

        void InitializeModule(IServerModule nextModule);

        Task<ServerResponse> ProcessRequest(ServerRequest request);
    }
}
