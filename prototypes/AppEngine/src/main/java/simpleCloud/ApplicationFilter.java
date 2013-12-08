// ApplicationFilter.java
//

package simpleCloud;

import java.io.*;
import javax.servlet.*;
import com.google.apphosting.api.*;
import simpleCloud.core.*;
import simpleCloud.services.*;

public final class ApplicationFilter implements Filter {

    private ServiceProvider _appServices;

    @Override
    public void destroy() {
    }

    @Override
    public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
            throws IOException, ServletException {
        ServiceContainer services = new ServiceContainer(_appServices);
        request.setAttribute(ServiceProvider.class.getName(), services);

        String requestID = ApiProxy.getCurrentEnvironment()
                .getAttributes()
                .get("com.google.appengine.runtime.request_log_id").toString();
        LoggingService log = new ServletLog(requestID);
        services.registerService(LoggingService.class, log);

        chain.doFilter(request, response);
    }

    @Override
    public void init(FilterConfig config) throws ServletException {
        _appServices = (ServiceProvider)config.getServletContext().getAttribute(ServiceProvider.class.getName());
    }
}
