// ScriptApplication.java
//

package simpleCloud.api;

import java.util.*;
import org.mozilla.javascript.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class ScriptApplication {

    private final ScriptableObject _settings;

    public ScriptApplication(Application app, Context context, Scriptable global) {
        ConfigurationService configService = app.getService(ConfigurationService.class);
        Map<Object, Object> configSettings = configService.getConfiguration("settings");

        if (configSettings != null) {
            _settings = (ScriptableObject)context.newObject(global);

            Iterator<Map.Entry<Object, Object>> iterator = configSettings.entrySet().iterator();
            while (iterator.hasNext()) {
                Map.Entry<Object, Object> pair = iterator.next();
                ScriptableObject.putProperty(_settings, pair.getKey().toString(), pair.getValue());
            }
        }
        else {
            _settings = null;
        }
    }

    public String getName() {
        return "Application";
    }

    public Object getSettings() {
        return _settings;
    }
}
