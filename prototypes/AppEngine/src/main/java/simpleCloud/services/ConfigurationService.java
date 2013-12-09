// ConfigurationService.java
//

package simpleCloud.services;

import java.util.*;

public interface ConfigurationService {

    public Map<Object, Object> getConfiguration();

    public Map<Object, Object> getConfiguration(String name);
}
