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
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

[assembly: ScriptAssembly("simpleCloud")]
[assembly: ScriptTemplate(@"
// {name}.js {version}

{dependenciesLookup}
var $global = this;

cmd.CommandLine.parse = function(cmdModel) {
  return cmdModel.parseNode.apply(cmdModel, process.argv);
}

var util = require('util');
var _traceEnabled = true;
function _abort() {
  _traceError.apply(null, arguments);
  process.abort();
}
function _traceAlert() {
  util.print('\x1B[1m\x1B[32mINFO \x1B[22m\x1B[39m');
  util.log(util.format.apply(null, arguments));
}
function _traceError() {
  util.print('\x1B[1m\x1B[31m ERR \x1B[22m\x1B[39m');
  util.log(util.format.apply(null, arguments));
}
function _traceInfo() {
  if (_traceEnabled) {
    util.print('\x1B[1m\x1B[37mINFO \x1B[22m\x1B[39m');
    util.log(util.format.apply(null, arguments));
  }
}
function _traceObject(name, obj) {
  if (_traceEnabled) {
    util.print('---- ');
    util.log(name + ': ' + util.inspect(obj));
  }
}
function _traceWarn() {
  if (_traceEnabled) {
    util.print('\x1B[1m\x1B[33mWARN \x1B[22m\x1B[39m');
    util.log(util.format.apply(null, arguments));
  }
}

{script}
")]
