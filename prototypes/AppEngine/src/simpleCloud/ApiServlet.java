package simpleCloud;

import java.io.*;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public final class ApiServlet extends HttpServlet {
    
    private static String appScript =
        "function app() {\n" +
        "  return (new Date()).toString();\n" +
        "}\n" +
        "app()";
    
    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String text = ScriptManager.Instance.executeScript("<app>", appScript);
        
        response.setContentType("text/plain");
        response.getWriter().println(text);
    }
}
