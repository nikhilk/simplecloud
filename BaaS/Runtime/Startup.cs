// Startup.cs
//

using System;
using NodeApi;
using NodeApi.IO;
using SimpleCloud;

[ScriptModule]
internal static class Startup {

    static Startup() {
        ApplicationOptions options;

        CommandModel commandModel =
            new CommandModel("sc").AddArgument("port", new CommandArgument("type", "number", "required", true))
                                  .AddArgument("path", new CommandArgument("type", "string"))
                                  .AddArgument("logs", new CommandArgument("type", "boolean"));

        try {
            options = (ApplicationOptions)CommandLine.Parse(commandModel);

            options.Port = Script.Or<int>(options.Port, Int32.Parse(Node.Process.Environment["PORT"]), 1337);
            options.Path = Script.Or(options.Path, Node.Process.GetCurrentDirectory());
        }
        catch (Exception e) {
            Console.Log(e.Message);
            Console.Log(commandModel.ToString());

            return;
        }

        Application.Current.Run(options);
    }
}
