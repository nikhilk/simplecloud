// HttpHandler.java
//

package simpleCloud;

import java.util.regex.*;
import javax.servlet.http.*;

public interface HttpHandler {

    public Pattern getRoute();

    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception;
}
