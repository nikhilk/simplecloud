// HttpHandler.java
//

package simpleCloud;

import java.util.regex.*;
import javax.servlet.http.*;

public interface HttpFeature {

    public Pattern getRoute();

    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception;
}
