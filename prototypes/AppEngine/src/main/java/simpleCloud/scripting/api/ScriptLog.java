// ScriptLog.java
//

package simpleCloud.scripting.api;

import simpleCloud.services.*;

public final class ScriptLog {

    private LoggingService _log;

    public ScriptLog(LoggingService log) {
        _log = log;
    }

    public void info(String message) {
        _log.info(message);
    }
}
