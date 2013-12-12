// ScriptApplication.java
//

package simpleCloud.api;

import java.util.*;
import org.mozilla.javascript.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class ScriptApplication {

    private final Application _app;
    private final ScriptableObject _settings;
    private ScriptCache _cache;

    public ScriptApplication(Application app, Context context, Scriptable global) {
        _app = app;

        ConfigurationService configService = app.getService(ConfigurationService.class);
        Map<Object, Object> configSettings = configService.getConfiguration();

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

    public ScriptCache getCache() {
        if (_cache == null) {
            _cache = new ScriptCache((ServiceProvider)_app);
        }

        return _cache;
    }

    public String getName() {
        return "Application";
    }

    public Object getSettings() {
        return _settings;
    }
}
