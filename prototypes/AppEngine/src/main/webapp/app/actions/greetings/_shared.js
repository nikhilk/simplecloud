function generateGreeting(g) {
  return '[' + (new Date()).toString() + '] ' + app.name + '(' + request.method + ' ' + request.path + '): ' + g + '!';
}
