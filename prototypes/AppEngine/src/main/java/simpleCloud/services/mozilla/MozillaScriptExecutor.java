// MozillaScriptExecutor.java
//

package simpleCloud.services.mozilla;

import java.io.*;
import org.mozilla.javascript.*;
import simpleCloud.services.*;

public final class MozillaScriptExecutor implements ScriptExecutor {

    private ContextFactory _contextFactory;
    private ScriptLoader _loader;

    public MozillaScriptExecutor() {
        _contextFactory = new SandboxContextFactory();
        _loader = new ScriptLoader();
    }

    @Override
    public String executeScript(final String path, final String name) throws ScriptException {
        try {
            final String script = _loader.loadScript(path);

            return (String)_contextFactory.call(new ContextAction() {
                @Override
                public Object run(Context scriptContext) {
                    Scriptable scriptScope = scriptContext.initStandardObjects();
                    Object result = scriptContext.evaluateString(scriptScope, script, name, 1, null);

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


    private final class SandboxContextFactory extends ContextFactory {

        @Override
        protected Context makeContext() {
            final Context context = super.makeContext();
            context.setClassShutter(new SandboxClassShutter());

            return context;
        }
    }

    private final class SandboxClassShutter implements ClassShutter {

        public boolean visibleToScripts(String name) {
            // Disable access to arbitrary java types
            return false;
        }
    }
}
