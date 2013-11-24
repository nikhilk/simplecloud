// MozillaScriptExecutor.java
//

package simpleCloud.scripting;

import java.io.*;
import java.util.*;
import org.mozilla.javascript.*;
import simpleCloud.*;
import simpleCloud.scripting.api.*;
import simpleCloud.services.*;

public final class MozillaScriptExecutor implements ScriptExecutor {

    private HashMap<ScriptName, Script> _scripts;

    private ContextFactory _contextFactory;
    private ScriptableObject _sharedGlobal;

    public MozillaScriptExecutor(ScriptLoader loader, ScriptApplication app) {
        _contextFactory = new SandboxContextFactory();
        _sharedGlobal = createGlobalObject(app);

        _scripts = loadScripts(loader);
    }

    private ScriptableObject createGlobalObject(final ScriptApplication app) {
        return (ScriptableObject)_contextFactory.call(new ContextAction() {
            @Override
            public Object run(Context scriptContext) {
                ScriptableObject global = new TopLevel();

                scriptContext.initStandardObjects(global, true);
                ScriptableObject.putProperty(global, "app", Context.javaToJS(app, global));

                global.sealObject();
                return global;
            }
        });
    }

    @Override
    public String executeScript(final ScriptName name, final String objectKey, final Object object) throws ScriptException {
        final Script script = _scripts.get(name);
        if (script == null) {
            throw new ScriptException("The specified script was not found.");
        }

        try {
            return (String)_contextFactory.call(new ContextAction() {
                @Override
                public Object run(Context scriptContext) {
                    Scriptable scope = scriptContext.newObject(_sharedGlobal);
                    scope.setPrototype(_sharedGlobal);
                    scope.setParentScope(null);

                    ScriptableObject.putProperty(scope, objectKey, Context.javaToJS(object, scope));

                    Object result = script.exec(scriptContext, scope);
                    return Context.toString(result);
                }
            });
        }
        catch (RhinoException e) {
            throw new ScriptException("Unable to execute script.", e);
        }
    }

    @SuppressWarnings("unchecked")
    private HashMap<ScriptName, Script> loadScripts(final ScriptLoader loader) {
        return (HashMap<ScriptName, Script>)_contextFactory.call(new ContextAction() {
            @Override
            public Object run(Context scriptContext) {
                HashMap<ScriptName, Script> scripts = new HashMap<ScriptName, Script>();

                for (ScriptName name : loader.getNames()) {
                    try {
                        String scriptSource = loader.loadScript(name);
                        Script script = scriptContext.compileString(scriptSource, name.getName(), 1, null);

                        scripts.put(name, script);
                    }
                    catch (IOException ioe) {
                    }
                }

                return scripts;
            }
        });
    }

    private final class SandboxContextFactory extends ContextFactory {

        @Override
        protected Context makeContext() {
            final Context context = super.makeContext();

            String apiPackage = ScriptApplication.class.getPackage().getName();
            context.setClassShutter(new SandboxClassShutter(apiPackage));

            context.setWrapFactory(new SandboxWrapFactory());

            return context;
        }
    }

    private final class SandboxClassShutter implements ClassShutter {

        private String _allowedPackagePrefix;
        private HashSet<String> _allowedNames;

        public SandboxClassShutter(String allowedPackage) {
            _allowedPackagePrefix = allowedPackage + ".";

            _allowedNames = new HashSet<String>();
            _allowedNames.add(String.class.getName());
        }

        public boolean visibleToScripts(String name) {
            // Only allow access to classes within the allowed package
            return name.startsWith(_allowedPackagePrefix) ||
                   _allowedNames.contains(name);
        }
    }

    public final class SandboxWrapFactory extends WrapFactory {

        @Override
        public Scriptable wrapAsJavaObject(Context cx, Scriptable scope, Object object, Class<?> objectType) {
            return new SandboxNativeJavaObject(scope, object, objectType);
        }
    }

    @SuppressWarnings("serial")
    public final class SandboxNativeJavaObject extends NativeJavaObject {

        public SandboxNativeJavaObject(Scriptable scope, Object object, Class<?> objectType) {
            super(scope, object, objectType);
        }

        @Override
        public Object get(String name, Scriptable scriptable) {
            if (name.equals("getClass")) {
                // Prevent access to reflection, which would allow script to
                // get to the class, then package, and all other classes.
                return Scriptable.NOT_FOUND;
            }

            return super.get(name, scriptable);
        }
    }
}
