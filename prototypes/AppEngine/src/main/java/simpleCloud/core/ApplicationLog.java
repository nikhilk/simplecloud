// ApplicationLog.java
//

package simpleCloud.core;

import java.util.logging.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class ApplicationLog implements LoggingService {

    private Logger _logger;

    public ApplicationLog() {
        _logger = Logger.getLogger(Application.class.getPackage().getName());
    }

    @Override
    public void error(String message) {
        _logger.log(Level.SEVERE, message);
    }

    @Override
    public void error(String message, Throwable thrown) {
        _logger.log(Level.SEVERE, message, thrown);
    }

    @Override
    public void info(String message) {
        _logger.log(Level.INFO, message);
    }
}
