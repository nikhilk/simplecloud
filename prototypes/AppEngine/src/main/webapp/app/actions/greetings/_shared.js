function generateGreeting(g) {
  return '[' + (new Date()).toString() + '] ' + app.name + '(' + request.name + '): ' + g + '!';
}
