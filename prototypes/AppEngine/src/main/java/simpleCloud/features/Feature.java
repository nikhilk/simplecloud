// Feature.java
//

package simpleCloud.features;

import simpleCloud.*;

public abstract class Feature implements ApplicationFeature {

    private String _name;
    private Application _app;

    protected Feature(Application app, String name) {
        _app = app;
        _name = name;
    }

    protected final Application getApplication() {
        return _app;
    }

    @Override
    public final String getName() {
        return _name;
    }
}
