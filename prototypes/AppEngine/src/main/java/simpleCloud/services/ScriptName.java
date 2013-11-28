// ScriptName.java
//

package simpleCloud.services;

import java.util.*;

public final class ScriptName {

    private String _feature;
    private String _group;
    private String _name;

    private Object _object;

    private String _qualifiedName;
    private int _hashCode;

    public ScriptName(String feature, String group, String name) {
        this(feature, group, name, null);
    }

    public ScriptName(String feature, String group, String name, Object object) {
        _feature = feature;
        _group = group;
        _name = name;
        _object = object;

        if (group != null) {
            _qualifiedName = feature + "." + group + "." + name;
        }
        else {
            _qualifiedName = feature + "." + name;
        }
        _qualifiedName = _qualifiedName.toLowerCase(Locale.ROOT);

        _hashCode = _qualifiedName.hashCode();
    }

    @Override
    public boolean equals(Object obj) {
        return (obj instanceof ScriptName) &&
               ((ScriptName)obj).getQualifiedName().equals(getQualifiedName());
    }

    public String getFeature() {
        return _feature;
    }

    public String getGroup() {
        return _group;
    }

    public String getName() {
        return _name;
    }

    public Object getObject() {
        return _object;
    }

    public String getQualifiedName() {
        return _qualifiedName;
    }

    @Override
    public int hashCode() {
        return _hashCode;
    }
}
