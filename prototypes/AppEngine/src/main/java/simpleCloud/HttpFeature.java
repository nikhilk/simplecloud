// HttpHandler.java
//

package simpleCloud;

import java.util.regex.*;
import javax.servlet.*;
import javax.servlet.http.*;

public interface HttpFeature {

    public Pattern getRoute();

    public void processRequest(ServletContext context, HttpServletRequest request, HttpServletResponse response) throws Exception;
}
