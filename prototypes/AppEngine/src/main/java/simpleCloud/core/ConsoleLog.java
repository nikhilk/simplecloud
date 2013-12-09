// ConsoleLog.java
//

package simpleCloud.core;

import java.util.logging.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class ConsoleLog implements LoggingService {

    private Logger _logger;
    private String _messagePrefix;

    public ConsoleLog(ServiceProvider services) {
        Logger.getGlobal().setLevel(Level.SEVERE);

        _logger = Logger.getAnonymousLogger();
        _logger.setLevel(Level.SEVERE);
        _messagePrefix = "";
    }

    public ConsoleLog(ServiceProvider services, String prefix) {
        this(services);
        _messagePrefix = prefix + ": ";

        // TODO: Avoid doing this per request

        Application app = services.getService(Application.class);
        String level = (String)app.getConfiguration().get("logLevel");
        if (level != null) {
            _logger.setLevel(Level.parse(level));
        }
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
