// HttpHandler.java
//

package simpleCloud;

import java.util.regex.*;
import javax.servlet.http.*;

public interface HttpFeature {

    public Pattern getRoute();

    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception;

    public static final class HttpMethods {

        public static final String DELETE = "DELETE";
        public static final String GET = "GET";
        public static final String HEAD = "HEAD";
        public static final String OPTIONS = "OPTIONS";
        public static final String POST = "POST";
        public static final String PUT = "PUT";

        private HttpMethods() {
        }
    }
}
