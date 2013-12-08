// ConsoleLog.java
//

package simpleCloud.core;

import java.util.logging.*;
import simpleCloud.services.*;

public final class ConsoleLog implements LoggingService {

    private Logger _logger;
    private String _messagePrefix;

    public ConsoleLog() {
        _logger = Logger.getAnonymousLogger();
        _messagePrefix = "";
    }

    public ConsoleLog(String prefix) {
        this();
        _messagePrefix = prefix + ": ";
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
