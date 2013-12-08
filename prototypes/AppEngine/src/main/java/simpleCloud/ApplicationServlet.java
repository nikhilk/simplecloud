// ApplicationServlet.java
//

package simpleCloud;

import java.io.*;
import java.util.*;
import java.util.regex.*;
import javax.servlet.*;
import javax.servlet.http.*;
import simpleCloud.services.*;

@SuppressWarnings("serial")
public final class ApplicationServlet extends HttpServlet {

    private List<HttpFeature> _features;

    @Override
    public void init(ServletConfig config) {
        ServletContext context = config.getServletContext();
        ServiceProvider services = (ServiceProvider)context.getAttribute(ServiceProvider.class.getName());
        Application app = services.getService(Application.class);

        _features = new ArrayList<HttpFeature>();
        for (Feature feature : app.getFeatures()) {
            if (feature instanceof HttpFeature) {
                _features.add((HttpFeature)feature);
            }
        }
    }

    @Override
    public void service(HttpServletRequest request, HttpServletResponse response) throws IOException {
        HttpFeature feature = null;

        String uri = request.getRequestURI();
        for (HttpFeature f : _features) {
            Matcher matcher = f.getRoute().matcher(uri);
            if (matcher.matches()) {
                request.setAttribute(MatchResult.class.getName(), matcher);
                feature = f;
                break;
            }
        }

        if (feature == null) {
            response.setStatus(HttpServletResponse.SC_NOT_FOUND);
            return;
        }

        try {
            feature.processRequest(request, response);
        }
        catch (Exception e) {
            response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
            response.setContentType("text/plain");
            response.getWriter().println(e.getMessage());
        }
    }
}
