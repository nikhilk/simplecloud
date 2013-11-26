// ActionsFeature.java
//

package simpleCloud.features;

import java.util.regex.*;
import javax.servlet.http.*;
import simpleCloud.*;
import simpleCloud.scripting.api.*;
import simpleCloud.services.*;

public final class ActionsFeature extends ApplicationFeature implements HttpHandler {

    public ActionsFeature(Application app) {
        super(app, "actions");
    }

    public Pattern getRoute() {
        return Pattern.compile("^/actions/([a-z0-9]+)(?:/([a-z0-9]+))?/?$", Pattern.CASE_INSENSITIVE);
    }

    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
        MatchResult match = (MatchResult)request.getAttribute(MatchResult.class.getName());

        String actionGroup = match.group(1);
        // String actionName = match.group(2);

        ScriptName scriptName = new ScriptName(actionGroup);
        ScriptRequest scriptRequest = new ScriptRequest(actionGroup);

        ScriptExecutor scriptExecutor = getApplication().getScriptExecutor();
        String result = scriptExecutor.executeScript(scriptName, "request", scriptRequest);

        response.setStatus(HttpServletResponse.SC_OK);
        response.setContentType("text/plain");
        response.getWriter().println(result);
    }
}
