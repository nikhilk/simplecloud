// Startup.cs
//

using System;
using NodeApi;
using NodeApi.IO;
using SimpleCloud;

[ScriptModule]
internal static class Startup {

    static Startup() {
        CommandModel commandModel =
            new CommandModel("sc").AddArgument("port", new CommandArgument("type", "number"))
                                  .AddArgument("path", new CommandArgument("type", "string"))
                                  .AddArgument("logs", new CommandArgument("type", "boolean"));

        ApplicationOptions options;
        try {
            options = (ApplicationOptions)CommandLine.Parse(commandModel);
            options.Port = Script.Or(options.Port, Number.ParseInt(Node.Process.Environment["PORT"]), 1337);
            options.Path = Script.Or(options.Path, Node.Process.GetCurrentDirectory());

            Runtime.EnableTrace = options.Logs;
        }
        catch (Exception e) {
            Console.Log(e.Message);
            Console.Log(commandModel.ToString());

            return;
        }

        Application app = new Application(options);
        app.Run();
    }
}
