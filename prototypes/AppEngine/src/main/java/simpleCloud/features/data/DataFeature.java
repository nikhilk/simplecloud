// DataFeature.java
//

package simpleCloud.features.data;

import java.util.regex.*;
import javax.servlet.http.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class DataFeature extends Feature implements HttpFeature {

    private static final String FeatureName = "data";

    public DataFeature(ServiceProvider services) {
        super(DataFeature.FeatureName, services);
    }

    @Override
    public Pattern getRoute() {
        return Pattern.compile("^/data/([a-z0-9]+)(?:/([a-z0-9]+))?(?:@([^@/]+))?/?$", Pattern.CASE_INSENSITIVE);
    }

    @Override
    public void processRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
        MatchResult match = (MatchResult)request.getAttribute(MatchResult.class.getName());
        String dataType = match.group(1);
        String dataAction = match.group(2);
        String dataEntity = match.group(3);
        String dataMethod = request.getMethod();
    }
}
