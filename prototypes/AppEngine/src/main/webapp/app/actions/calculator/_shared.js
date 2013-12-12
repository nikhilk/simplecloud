function getValue() {
  var value = app.cache.getValue('value');
  if (value === null) {
    value = 0;
  }

  return value;
}

function setValue(value) {
  app.cache.setValue('value', value);
}

function resetValue() {
  app.cache.clearValue('value');
}
