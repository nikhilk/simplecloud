// ScriptEngine.java
//

package simpleCloud.core;

import java.io.*;
import java.util.*;
import org.mozilla.javascript.*;
import simpleCloud.*;
import simpleCloud.scripting.api.*;
import simpleCloud.services.*;

public final class ScriptEngine implements ScriptExecutor {

    private static final String modulesDirectoryName = "code";
    private static final Object moduleLoadLock = new Object();

    private HashMap<ScriptName, Script> _scripts;

    private ContextFactory _contextFactory;
    private ScriptableObject _sharedGlobal;

    public ScriptEngine(ServiceProvider services) {
        _contextFactory = new SandboxContextFactory();

        Application app = services.getService(Application.class);
        StorageService storage = services.getService(StorageService.class);

        _scripts = loadScripts(app, storage);
        _sharedGlobal = createGlobalObject(app);
    }

    private ScriptableObject createGlobalObject(final Application app) {
        return (ScriptableObject)_contextFactory.call(new ContextAction() {
            @Override
            public Object run(Context scriptContext) {
                ScriptableObject global = new TopLevel();
                ScriptApplication appObject = new ScriptApplication(app);

                scriptContext.initStandardObjects(global, true);
                ScriptableObject.putProperty(global, "app", Context.javaToJS(appObject, global));
                ScriptableObject.putProperty(global, "require", new RequireFunction(global, _scripts));

                global.sealObject();
                return global;
            }
        });
    }

    @Override
    public String executeScript(final ScriptName name, final boolean useSharedScript,
                                final String objectKey, final Object object) throws ScriptException {
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

                    if (useSharedScript) {
                        ScriptName sharedScriptName = new ScriptName(name.getFeature(), name.getGroup(), "_shared");
                        Script sharedScript = _scripts.get(sharedScriptName);

                        if (sharedScript != null) {
                            sharedScript.exec(scriptContext, scope);
                        }
                    }

                    Object result = script.exec(scriptContext, scope);
                    return Context.toString(result);
                }
            });
        }
        catch (RhinoException e) {
            throw new ScriptException("Unable to execute script.", e);
        }
    }

    private List<ScriptName> findScripts(Application app, StorageService storage) {
        StorageFile appDirectory = storage.getRoot();

        List<ScriptName> allNames;
        StorageFile codeDirectory = appDirectory.getFile(ScriptEngine.modulesDirectoryName);
        if (codeDirectory.isDirectory()) {
            allNames = getScriptsFromDirectory(ScriptEngine.modulesDirectoryName, null, codeDirectory);
        }
        else {
            allNames = new ArrayList<ScriptName>();
        }

        for (Feature feature : app.getFeatures()) {
            if (!(feature instanceof ScriptFeature)) {
                continue;
            }

            String featureName = feature.getName();

            StorageFile featureDirectory = appDirectory.getFile(featureName);
            if (!featureDirectory.isDirectory()) {
                continue;
            }

            if (((ScriptFeature)feature).usesGroupedScriptFiles()) {
                for (StorageFile groupDirectory : featureDirectory.getFiles()) {
                    if (!(groupDirectory.isDirectory())) {
                        continue;
                    }

                    List<ScriptName> names = getScriptsFromDirectory(featureName, groupDirectory.getName(), groupDirectory);
                    allNames.addAll(names);
                }
            }
            else {
                List<ScriptName> names = getScriptsFromDirectory(featureName, null, featureDirectory);
                allNames.addAll(names);
            }
        }

        return allNames;
    }

    private List<ScriptName> getScriptsFromDirectory(String featureName, String groupName, StorageFile directory) {
        List<ScriptName> names = new ArrayList<ScriptName>();

        for (StorageFile file : directory.getFiles()) {
            if (file.isDirectory()) {
                continue;
            }

            try {
                ScriptName name = new ScriptName(featureName, groupName, file.getName(),
                                                 file.getContent());
                names.add(name);
            }
            catch (IOException e) {
                // TODO: Error logging
            }
        }

        return names;
    }

    @SuppressWarnings("unchecked")
    private HashMap<ScriptName, Script> loadScripts(final Application app, final StorageService storage) {
        return (HashMap<ScriptName, Script>)_contextFactory.call(new ContextAction() {
            @Override
            public Object run(Context scriptContext) {
                HashMap<ScriptName, Script> scripts = new HashMap<ScriptName, Script>();

                for (ScriptName name : findScripts(app, storage)) {
                    try {
                        String scriptSource = (String)name.getObject();
                        Script script = scriptContext.compileString(scriptSource, name.getName(), 1, null);

                        scripts.put(name, script);
                    }
                    catch (Exception e) {
                        // TODO: Error message
                    }
                }

                return scripts;
            }
        });
    }

    @Override
    public ScriptName resolveScript(List<ScriptName> names) {
        for (ScriptName n : names) {
            Script script = _scripts.get(n);
            if (script != null) {
                return n;
            }
        }

        return null;
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

    @SuppressWarnings("serial")
    final class RequireFunction extends BaseFunction {

        private HashMap<ScriptName, Script> _scripts;
        private HashMap<ScriptName, Object> _modules;

        public RequireFunction(Scriptable scope, HashMap<ScriptName, Script> scripts) {
            super(scope, ScriptableObject.getFunctionPrototype(scope));

            _scripts = scripts;
            _modules = new HashMap<ScriptName, Object>();
        }

        @Override
        public Object call(Context context, Scriptable scope, Scriptable thisObj, Object[] args) {
            if ((args.length != 1) || !(args[0] instanceof String)) {
                throw Context.reportRuntimeError("require must be called with a single string argument");
            }

            String moduleName = (String)args[0];
            ScriptName name = new ScriptName("code", null, moduleName);

            Script script = _scripts.get(name);
            if (script == null) {
                throw Context.reportRuntimeError("Unknown code module named '" + moduleName + "'");
            }

            Object module = _modules.get(name);
            if (module != null) {
                return module;
            }

            synchronized (ScriptEngine.moduleLoadLock) {
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
}
