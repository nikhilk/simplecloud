// CaseInsensitiveHashMap.java
//

package simpleCloud.util;

import java.util.HashMap;

@SuppressWarnings("serial")
public final class CaseInsensitiveHashMap<V> extends HashMap<String, V> {

    @Override
    public V put(String key, V value) {
       return super.put(key.toLowerCase(), value);
    }

    public V get(String key) {
       return super.get(key.toLowerCase());
    }
}
