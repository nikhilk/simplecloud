// Application.java
//

package simpleCloud;

import javax.servlet.*;
import simpleCloud.scripting.*;
import simpleCloud.scripting.api.*;
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
        ScriptApplication appObject = new ScriptApplication(this);

        _scriptExecutor = new MozillaScriptExecutor(loader, appObject);

        ServletContext context = e.getServletContext();
        context.setAttribute(Application.class.getName(), this);
    }

    public ScriptExecutor getScriptExecutor() {
        return _scriptExecutor;
    }
}
