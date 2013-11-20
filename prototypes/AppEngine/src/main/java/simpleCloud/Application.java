// Application.java
//

package simpleCloud;

import javax.servlet.*;
import simpleCloud.services.*;
import simpleCloud.services.mozilla.*;

public final class Application implements ServletContextListener {
    
    private ScriptExecutor _scriptExecutor;

    @Override
    public void contextDestroyed(ServletContextEvent e) {
    }

    @Override
    public void contextInitialized(ServletContextEvent e) {
        _scriptExecutor = new MozillaScriptExecutor();
        
        ServletContext context = e.getServletContext();
        context.setAttribute(Application.class.getName(), this);
    }
    
    public ScriptExecutor getScriptExecutor() {
        return _scriptExecutor;
    }
}
