// RequireFunction.java
//

package simpleCloud.scripting;

import java.util.*;
import org.mozilla.javascript.*;
import simpleCloud.services.*;

@SuppressWarnings("serial")
final class RequireFunction extends BaseFunction {

    private static final Object loadLock = new Object();

    private HashMap<ScriptName, Script> _scripts;
    private HashMap<ScriptName, Object> _modules;
    private String _codeFeature;

    public RequireFunction(Scriptable scope, HashMap<ScriptName, Script> scripts, String codeFeature) {
        super(scope, ScriptableObject.getFunctionPrototype(scope));

        _scripts = scripts;
        _codeFeature = codeFeature;

        _modules = new HashMap<ScriptName, Object>();
    }

    @Override
    public Object call(Context context, Scriptable scope, Scriptable thisObj, Object[] args) {
        if ((args.length != 1) || !(args[0] instanceof String)) {
            throw Context.reportRuntimeError("require must be called with a single string argument");
        }

        String moduleName = (String)args[0];
        ScriptName name = new ScriptName(_codeFeature, null, moduleName);

        Script script = _scripts.get(name);
        if (script == null) {
            throw Context.reportRuntimeError("Unknown code module named '" + moduleName + "'");
        }

        Object module = _modules.get(name);
        if (module != null) {
            return module;
        }

        synchronized (loadLock) {
            module = _modules.get(name);
            if (module != null) {
                return module;
            }

            return loadModule(context, name, script);
        }
    }

    @Override
    public int getArity() {
        return 1;
    }

    private Object loadModule(Context context, ScriptName name, Script script) {
        Scriptable globalScope = getParentScope();

        Scriptable scope = context.newObject(globalScope);
        scope.setPrototype(globalScope);
        scope.setParentScope(null);

        Scriptable exports = context.newObject(globalScope);
        _modules.put(name, exports);

        ScriptableObject.putProperty(scope, "exports", exports);
        ScriptableObject.putProperty(scope, "require", this);

        script.exec(context, scope);

        Object module = ScriptableObject.getProperty(scope, "exports");
        _modules.put(name, module);

        return module;
    }
}
