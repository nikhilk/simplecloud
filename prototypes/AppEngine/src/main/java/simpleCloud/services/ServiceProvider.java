// ServiceProvider.java

package simpleCloud.services;

public interface ServiceProvider {

    public <T> T getService(Class<T> serviceClass);
}
