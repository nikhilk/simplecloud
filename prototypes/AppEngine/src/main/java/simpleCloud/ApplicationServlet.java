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
        Application app = getApplication();
        HttpHandler httpHandler = null;

        String uri = request.getRequestURI();
        for (ApplicationFeature feature : app.getFeatures()) {
            if (feature instanceof HttpHandler) {
                HttpHandler h = (HttpHandler)feature;

                Matcher matcher = h.getRoute().matcher(uri);
                if (matcher.matches()) {
                    request.setAttribute(MatchResult.class.getName(), matcher);
                    httpHandler = h;
                    break;
                }
            }
        }

        if (httpHandler == null) {
            response.setStatus(HttpServletResponse.SC_NOT_FOUND);
            return;
        }

        try {
            httpHandler.processRequest(request, response);
        }
        catch (Exception e) {
            response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
            response.setContentType("text/plain");
            response.getWriter().println(e.getMessage());
        }
    }
}
