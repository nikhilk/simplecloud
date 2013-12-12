var value = getValue();

var operand = request.getParameter('value');
if (operand !== null) {
  value += parseFloat(operand);
  setValue(value);
}

request.result = value;
