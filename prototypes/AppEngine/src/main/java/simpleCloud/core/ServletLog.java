// ServletLog.java
//

package simpleCloud.core;

import java.util.logging.*;
import simpleCloud.services.*;

public final class ServletLog implements LoggingService {

    private Logger _logger;
    private String _messagePrefix;

    public ServletLog(String requestID) {
        _logger = Logger.getAnonymousLogger();

        _messagePrefix = requestID + ": ";
    }

    @Override
    public void error(String message) {
        _logger.log(Level.SEVERE, qualifyMessage(message));
    }

    @Override
    public void error(String message, Throwable thrown) {
        _logger.log(Level.SEVERE, qualifyMessage(message), thrown);
    }

    @Override
    public void info(String message) {
        _logger.log(Level.INFO, qualifyMessage(message));
    }

    private String qualifyMessage(String message) {
        return _messagePrefix + message;
    }
}
