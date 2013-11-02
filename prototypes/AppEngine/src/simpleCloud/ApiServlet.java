package simpleCloud;

import java.io.*;
import javax.servlet.http.*;
import org.mozilla.javascript.*;

@SuppressWarnings("serial")
public final class ApiServlet extends HttpServlet {
    
    private static String appScript =
        "function app() {\n" +
        "  return (new Date()).toString();\n" +
        "}\n" +
        "app()";

    public void doGet(HttpServletRequest request, HttpServletResponse response) throws IOException {
        String text = executeScript();
        
        response.setContentType("text/plain");
        response.getWriter().println(text);
    }
    
    private String executeScript() {
        Context scriptContext = Context.enter();
        
        try {
            scriptContext.setClassShutter(new SuppressAccessClassShutter());
            
            Scriptable scriptScope = scriptContext.initStandardObjects();
            Object result = scriptContext.evaluateString(scriptScope, appScript, "<app>", 1, null);
            
            return Context.toString(result);
        }
        catch (RhinoException e) {
            return e.getMessage();
        }
        finally {
            Context.exit();
        }
    }
    
    private final class SuppressAccessClassShutter implements ClassShutter {
        
        public boolean visibleToScripts(String name) {
            return false;
        }
    }
}
