// Startup.cs
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NodeApi;
using NodeApi.IO;
using NodeApi.Network;
using SimpleCloud.Server;

[ScriptModule]
internal static class Startup {

    static Startup() {
        Dictionary<string, object> arguments;

        CommandModel commandModel =
            new CommandModel("sc").AddArgument("port", new CommandArgument("type", "number", "required", true))
                                  .AddArgument("path", new CommandArgument("type", "string"))
                                  .AddArgument("logs", new CommandArgument("type", "boolean"));

        try {
            arguments = CommandLine.Parse(commandModel);
        }
        catch (Exception e) {
            Console.Log(e.Message);
            Console.Log(commandModel.ToString());

            return;
        }

        int port = Script.Or<int>((int)arguments["port"], Int32.Parse(Node.Process.Environment["PORT"]), 1337);
        string path = Script.Or((string)arguments["path"], Node.Process.GetCurrentDirectory());
        bool log = (bool)arguments["log"];

        List<IServerModule> modules = new List<IServerModule>();
        Dictionary<string, IServerHandler> handlers = new Dictionary<string, IServerHandler>();

        ServerApplication application = new ServerApplication(path, modules, handlers, log);
        application.Run(port);
    }
}
