// ScriptName.java
//

package simpleCloud.services;

import java.util.*;

public final class ScriptName {

    private String _name;
    private int _hashCode;

    public ScriptName(String name) {
        _name = name;
        _hashCode = name.toLowerCase(Locale.ROOT).hashCode();
    }

    @Override
    public boolean equals(Object obj) {
        return (obj instanceof ScriptName) &&
               ((ScriptName)obj).getName().equalsIgnoreCase(_name);
    }

    public String getName() {
        return _name;
    }

    @Override
    public int hashCode() {
        return _hashCode;
    }
}
