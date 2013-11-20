// ApiServlet.java
//

package simpleCloud;

import java.io.*;
import javax.servlet.http.*;
import simpleCloud.services.*;
import simpleCloud.services.mozilla.*;

@SuppressWarnings("serial")
public final class ApiServlet extends HttpServlet {
    
    private ScriptExecutor _scriptExecutor;

    public ApiServlet() {
        this(new MozillaScriptExecutor());
    }

    public ApiServlet(ScriptExecutor scriptExecutor) {
        _scriptExecutor = scriptExecutor;
    }

    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String result;
        Boolean success = false;
        
        try {
            result = _scriptExecutor.executeScript("app/app.js", "<app>");
            success = true;
        }
        catch (ScriptException e) {
            result = e.getMessage();
        }

        response.setStatus(success ? HttpServletResponse.SC_OK : HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
        response.setContentType("text/plain");
        response.getWriter().println(result);
    }
}
