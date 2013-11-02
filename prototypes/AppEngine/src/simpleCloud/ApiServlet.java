package simpleCloud;

import java.io.*;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public class ApiServlet extends HttpServlet {

    public void doGet(HttpServletRequest req, HttpServletResponse resp) throws IOException {
        resp.setContentType("text/plain");
        resp.getWriter().println("Hello, API Caller!");
    }
}
