// FeatureBase.java
//

package simpleCloud.features;

import simpleCloud.*;

public abstract class FeatureBase implements Feature {

    private String _name;
    private Application _app;

    protected FeatureBase(Application app, String name) {
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
