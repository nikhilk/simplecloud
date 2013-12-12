// ScriptRequest.java
//

package simpleCloud.api;

import javax.servlet.http.*;
import simpleCloud.services.*;

public final class ScriptRequest {

    private final HttpServletRequest _request;
    private Object _result;
    private boolean _validResult;

    private ScriptLog _log;

    public ScriptRequest(HttpServletRequest request) {
        _request = request;
    }

    public ScriptLog getLog() {
        if (_log == null) {
            ServiceProvider services = (ServiceProvider)_request.getAttribute(ServiceProvider.class.getName());
            _log = new ScriptLog(services.getService(LoggingService.class));
        }

        return _log;
    }

    public String getName() {
        return _request.getRequestURI();
    }

    public Object getResult() {
        return _result;
    }

    public boolean hasResult() {
        return _validResult;
    }

    public void setResult(Object value) {
        _result = value;
        _validResult = true;
    }
}
