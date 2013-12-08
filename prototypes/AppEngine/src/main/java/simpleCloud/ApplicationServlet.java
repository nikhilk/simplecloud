// ApplicationServlet.java
//

package simpleCloud;

import java.io.*;
import java.util.regex.*;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public final class ApplicationServlet extends HttpServlet {

    private Application getApplication() {
        return (Application)getServletContext().getAttribute(Application.class.getName());
    }

    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        HttpFeature httpFeature = null;

        String uri = request.getRequestURI();
        for (Feature feature : getApplication().getFeatures()) {
            if (feature instanceof HttpFeature) {
                HttpFeature h = (HttpFeature)feature;

                Matcher matcher = h.getRoute().matcher(uri);
                if (matcher.matches()) {
                    request.setAttribute(MatchResult.class.getName(), matcher);
                    httpFeature = h;
                    break;
                }
            }
        }

        if (httpFeature == null) {
            response.setStatus(HttpServletResponse.SC_NOT_FOUND);
            return;
        }

        try {
            httpFeature.processRequest(getServletContext(), request, response);
        }
        catch (Exception e) {
            response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
            response.setContentType("text/plain");
            response.getWriter().println(e.getMessage());
        }
    }
}
