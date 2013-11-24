// Application.java
//

package simpleCloud;

import javax.servlet.*;
import simpleCloud.scripting.*;
import simpleCloud.services.*;
import simpleCloud.storage.*;

public final class Application implements ServletContextListener {

    private ScriptExecutor _scriptExecutor;

    @Override
    public void contextDestroyed(ServletContextEvent e) {
    }

    @Override
    public void contextInitialized(ServletContextEvent e) {
        ScriptLoader loader = new LocalScriptLoader();
        _scriptExecutor = new MozillaScriptExecutor(this, loader);

        ServletContext context = e.getServletContext();
        context.setAttribute(Application.class.getName(), this);
    }

    public ScriptExecutor getScriptExecutor() {
        return _scriptExecutor;
    }
}
