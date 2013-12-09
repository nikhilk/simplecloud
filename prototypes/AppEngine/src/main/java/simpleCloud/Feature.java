// Feature.java
//

package simpleCloud;

import simpleCloud.services.*;

public abstract class Feature {

    private String _name;
    private ServiceProvider _services;

    protected Feature(String name, ServiceProvider services) {
        _name = name;
        _services = services;
    }

    protected final Application getApplication() {
        return _services.getService(Application.class);
    }

    public final String getName() {
        return _name;
    }
}
