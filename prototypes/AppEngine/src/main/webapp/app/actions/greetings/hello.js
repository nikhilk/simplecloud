function fn() {
  return '[' + (new Date()).toString() + '] ' + app.name + '(' + request.name + '): ' + 'Hi!';
}
fn();
