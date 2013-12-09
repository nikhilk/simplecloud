// ScriptRequest.java
//

package simpleCloud.api;

import javax.servlet.http.*;
import simpleCloud.services.*;

public final class ScriptRequest {

    private HttpServletRequest _request;

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
}
