// ServiceContainer.java
//

package simpleCloud.services;

import java.util.*;

public final class ServiceContainer implements ServiceProvider {

    private ServiceProvider _parent;
    private HashMap<Class<?>, Object> _services;

    public ServiceContainer(ServiceProvider parent) {
        _parent = parent;
        _services = new HashMap<Class<?>, Object>();
    }

    @Override
    @SuppressWarnings("unchecked")
    public <T> T getService(Class<T> serviceClass) {
        Object service = _services.get(serviceClass);
        if (service != null) {
            return (T)service;
        }

        if (_parent != null) {
            return _parent.getService(serviceClass);
        }

        return null;
    }

    public <T> void registerService(Class<T> serviceClass, T service) {
        _services.put(serviceClass, service);
    }
}
