// ScriptApplication.java
//

package simpleCloud.scripting.api;

import simpleCloud.*;

public final class ScriptApplication {

    private Application _app;

    public ScriptApplication(Application app) {
        _app = app;
    }

    public String getName() {
        return "Application";
    }
}
