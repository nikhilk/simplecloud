// ActionsFeature.java
//

package simpleCloud.features;

import java.util.*;
import java.util.regex.*;
import javax.servlet.*;
import javax.servlet.http.*;
import simpleCloud.*;
import simpleCloud.scripting.api.*;
import simpleCloud.services.*;

public final class ActionsFeature extends Feature implements HttpFeature, ScriptFeature {

    private static final String FeatureName = "actions";

    public ActionsFeature(Application app) {
        super(app, ActionsFeature.FeatureName);
    }

    public Pattern getRoute() {
        return Pattern.compile("^/actions/([a-z0-9]+)(?:/([a-z0-9]+))?/?$", Pattern.CASE_INSENSITIVE);
    }

    public boolean usesGroupedScriptFiles() {
        return true;
    }

    public void processRequest(ServletContext context, HttpServletRequest request, HttpServletResponse response)
            throws Exception {
        ScriptExecutor scriptExecutor = getApplication().getScriptExecutor();

        MatchResult match = (MatchResult)request.getAttribute(MatchResult.class.getName());
        String actionGroup = match.group(1);
        String actionName = match.group(2);
        String actionMethod = request.getMethod();

        ArrayList<ScriptName> candidateNames = new ArrayList<ScriptName>();
        if (actionName == null) {
            ScriptName name = new ScriptName(ActionsFeature.FeatureName, actionGroup, actionMethod);
            candidateNames.add(name);
        }
        else {
            if (actionMethod == "POST") {
                ScriptName simpleName = new ScriptName(ActionsFeature.FeatureName, actionGroup, actionName);
                candidateNames.add(simpleName);
            }

            ScriptName qualifiedName = new ScriptName(ActionsFeature.FeatureName, actionGroup,
                                                      actionMethod + "." + actionName);
            candidateNames.add(qualifiedName);
        }

        ScriptName resolvedName = scriptExecutor.resolveScript(candidateNames);
        if (resolvedName == null) {
            response.setStatus(HttpServletResponse.SC_NOT_FOUND);
            return;
        }

        ScriptRequest scriptRequest = new ScriptRequest(getApplication(), request);
        String result = scriptExecutor.executeScript(resolvedName, /* sharedScript */ true, "request", scriptRequest);

        response.setStatus(HttpServletResponse.SC_OK);
        response.setContentType("text/plain");
        response.getWriter().println(result);
    }
}
