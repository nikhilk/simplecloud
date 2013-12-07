// ScriptRequest.java
//

package simpleCloud.scripting.api;

import javax.servlet.http.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class ScriptRequest {

    private Application _app;
    private HttpServletRequest _request;

    private ScriptLog _log;

    public ScriptRequest(Application app, HttpServletRequest request) {
        _app = app;
        _request = request;
    }

    public ScriptLog getLog() {
        if (_log == null) {
            _log = new ScriptLog(_app.getServletLog());
        }

        return _log;
    }

    public String getName() {
        return _request.getRequestURI();
    }
}
