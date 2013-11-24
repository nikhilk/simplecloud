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

    private ContextFactory _contextFactory;

    private ScriptApplication _appObject;

    public MozillaScriptExecutor(Application app) {
        _contextFactory = new SandboxContextFactory();
        _appObject = new ScriptApplication(app);
    }

    @Override
    public String executeScript(final String path, final String name) throws ScriptException {
        try {
            final String script = loadScript(path);

            return (String)_contextFactory.call(new ContextAction() {
                @Override
                public Object run(Context scriptContext) {
                    ScriptableObject global = new TopLevel();
                    scriptContext.initStandardObjects(global);
                    ScriptableObject.putProperty(global, "app", Context.javaToJS(_appObject, global));

                    Object result = scriptContext.evaluateString(global, script, name, 1, null);
                    return Context.toString(result);
                }
            });
        }
        catch (IOException e) {
            throw new ScriptException("Unable to load script.", e);
        }
        catch (RhinoException e) {
            throw new ScriptException("Unable to execute script.", e);
        }
    }

    private String loadScript(String filePath) throws IOException {
        BufferedReader reader = new BufferedReader(new FileReader(filePath));
        try {
            StringBuffer buffer = new StringBuffer();
            char[] data = new char[1024];

            int readCount = 0;
            while((readCount = reader.read(data)) != -1){
                String s = String.valueOf(data, 0, readCount);
                buffer.append(s);
            }

            return buffer.toString();
        }
        finally {
            reader.close();
        }
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

    public static class SandboxWrapFactory extends WrapFactory {

        @Override
        public Scriptable wrapAsJavaObject(Context cx, Scriptable scope, Object object, Class<?> objectType) {
            return new SandboxNativeJavaObject(scope, object, objectType);
        }
    }

    @SuppressWarnings("serial")
    public static class SandboxNativeJavaObject extends NativeJavaObject {

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
