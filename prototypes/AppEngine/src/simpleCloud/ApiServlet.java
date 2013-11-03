package simpleCloud;

import java.io.*;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public final class ApiServlet extends HttpServlet {

    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String result = ScriptManager.Instance.executeScript("app/app.js", "<app>");
        
        response.setContentType("text/plain");
        response.getWriter().println(result);
    }
}
