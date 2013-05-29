// AssemblyInfo.cs
//

using System;
using System.Reflection;

[assembly: AssemblyTitle("SimpleCloud")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SimpleCloud")]
[assembly: AssemblyCopyright("Copyright © 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ScriptAssembly("Runtime")]
[assembly: ScriptTemplate(@"
// {name}.js {version}

{dependenciesLookup}
var $global = this;

cmd.CommandLine.parse = function(cmdModel) {
  return cmdModel.parseNode.apply(cmdModel, process.argv);
}

{script}
")]
