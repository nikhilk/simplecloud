// LoggingService.java
//

package simpleCloud.services;

public interface LoggingService {

    public void error(String message);

    public void error(String message, Throwable thrown);

    public void info(String message);
}
