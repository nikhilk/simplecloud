function fn() {
  return '[' + (new Date()).toString() + '] ' + app.name + '(' + request + '): ' + 'Hi!';
}
fn();
