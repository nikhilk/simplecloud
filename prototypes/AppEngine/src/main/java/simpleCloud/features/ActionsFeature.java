// ActionsFeature.java
//

package simpleCloud.features;

import java.util.regex.*;
import javax.servlet.http.*;
import simpleCloud.*;
import simpleCloud.scripting.api.*;
import simpleCloud.services.*;

public final class ActionsFeature extends ApplicationFeature implements HttpFeature, ScriptFeature {

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

    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
        MatchResult match = (MatchResult)request.getAttribute(MatchResult.class.getName());

        String actionGroup = match.group(1);
        String actionName = match.group(2);

        ScriptName scriptName = new ScriptName(ActionsFeature.FeatureName, actionGroup, actionName);
        ScriptRequest scriptRequest = new ScriptRequest(actionGroup + "." + actionName);

        ScriptExecutor scriptExecutor = getApplication().getScriptExecutor();
        String result = scriptExecutor.executeScript(scriptName, "request", scriptRequest);

        response.setStatus(HttpServletResponse.SC_OK);
        response.setContentType("text/plain");
        response.getWriter().println(result);
    }
}
