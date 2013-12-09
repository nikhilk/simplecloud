// Application.java
//

package simpleCloud;

import java.lang.reflect.*;
import java.util.*;
import javax.servlet.*;
import simpleCloud.core.*;
import simpleCloud.services.*;

public final class Application implements ServletContextListener, ServiceProvider {

    private HashMap<Class<?>, Object> _services;
    private List<Feature> _features;

    public Application() {
        _services = new HashMap<Class<?>, Object>();
        _services.put(Application.class, this);
        _services.put(StorageService.class, new LocalStorage());
        _services.put(LoggingService.class, new ConsoleLog());

        _features = createFeatures();

        _services.put(ScriptExecutor.class, new ScriptEngine(this));
    }

    @Override
    public void contextDestroyed(ServletContextEvent e) {
    }

    @Override
    public void contextInitialized(ServletContextEvent e) {
        ServletContext context = e.getServletContext();
        context.setAttribute(ServiceProvider.class.getName(), this);
    }

    @SuppressWarnings("unchecked")
    private List<Feature> createFeatures() {
        ArrayList<Feature> features = new ArrayList<Feature>();

        Properties props = System.getProperties();
        for (Enumeration<?> e = props.propertyNames(); e.hasMoreElements();) {
            String name = (String)e.nextElement();
            if (!name.startsWith("simpleCloud.feature")) {
                continue;
            }

            String featureClassName = props.getProperty(name);
            try {
                Class<Feature> featureClass = (Class<Feature>)Class.forName(featureClassName);
                Constructor<Feature> featureCtor = featureClass.getConstructor(ServiceProvider.class);

                features.add(featureCtor.newInstance((ServiceProvider)this));
            }
            catch (Exception ex) {
                // TODO: Logging, rather than silently ignoring
            }
        }

        return features;
    }

    public List<Feature> getFeatures() {
        return _features;
    }

    @Override
    @SuppressWarnings("unchecked")
    public <T> T getService(Class<T> serviceClass) {
        return (T)_services.get(serviceClass);
    }
}
