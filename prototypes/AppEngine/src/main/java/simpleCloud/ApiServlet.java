// ApiServlet.java
//

package simpleCloud;

import java.io.*;
import javax.servlet.http.*;
import simpleCloud.services.*;

@SuppressWarnings("serial")
public final class ApiServlet extends HttpServlet {

    private Application getApplication() {
        return (Application)getServletContext().getAttribute(Application.class.getName());
    }

    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String result;
        Boolean success = false;

        try {
            ScriptExecutor scriptExecutor = getApplication().getScriptExecutor();

            result = scriptExecutor.executeScript("app/app.js", "<app>");
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
