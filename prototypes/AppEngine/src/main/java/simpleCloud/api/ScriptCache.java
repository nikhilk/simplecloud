// ScriptCache.java
//

package simpleCloud.api;

import java.util.*;
import org.mozilla.javascript.Context;
import com.google.appengine.api.memcache.*;
import simpleCloud.services.*;

public final class ScriptCache {

    private MemcacheService _memcache;

    public ScriptCache(ServiceProvider services) {
        _memcache = MemcacheServiceFactory.getMemcacheService();
    }

    public boolean exists(String key) {
        return _memcache.contains(key);
    }

    public Object getValue(String key) {
        Object value = _memcache.get(key);
        return value;
    }

    public void setValue(String key, Object value) {
        _memcache.put(key, value);
    }

    public void setValueWithExpiration(String key, Object value, Date expirationTime) {
        _memcache.put(key, value, Expiration.onDate(expirationTime));
    }

    public void setValueWithTTL(String key, Object value, int seconds) {
        _memcache.put(key, value, Expiration.byDeltaSeconds(seconds));
    }

    public void reset() {
        _memcache.clearAll();
    }

    public void resetValue(String key) {
        _memcache.delete(key);
    }
}
