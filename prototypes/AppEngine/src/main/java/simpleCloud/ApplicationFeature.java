// ApplicationFeature.java
//

package simpleCloud;

public abstract class ApplicationFeature {

    private String _name;
    private Application _app;

    protected ApplicationFeature(Application app, String name) {
        _app = app;
        _name = name;
    }

    protected final Application getApplication() {
        return _app;
    }

    public final String getName() {
        return _name;
    }
}
