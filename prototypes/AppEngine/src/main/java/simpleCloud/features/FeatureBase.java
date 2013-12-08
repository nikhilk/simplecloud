// FeatureBase.java
//

package simpleCloud.features;

import simpleCloud.*;
import simpleCloud.services.*;

public abstract class FeatureBase implements Feature {

    private String _name;
    private ServiceProvider _services;

    protected FeatureBase(String name, ServiceProvider services) {
        _name = name;
        _services = services;
    }

    protected final Application getApplication() {
        return _services.getService(Application.class);
    }

    @Override
    public final String getName() {
        return _name;
    }
}
