// MozillaScriptExecutor.java
//

package simpleCloud.services.mozilla;

import java.io.*;
import org.mozilla.javascript.*;
import simpleCloud.services.*;

public final class MozillaScriptExecutor implements ScriptExecutor {
    
    private ScriptLoader _loader;
    
    public MozillaScriptExecutor() {
        _loader = new ScriptLoader();
    }

    @Override
    public String executeScript(String path, String name) throws ScriptException {
        Context scriptContext = Context.enter();
        
        try {
            scriptContext.setClassShutter(new SuppressAccessClassShutter());
            
            String script = _loader.loadScript(path);
            
            Scriptable scriptScope = scriptContext.initStandardObjects();
            Object result = scriptContext.evaluateString(scriptScope, script, name, 1, null);
            
            return Context.toString(result);
        }
        catch (IOException e) {
            throw new ScriptException("Unable to load script.", e);
        }
        catch (RhinoException e) {
            throw new ScriptException("Unable to execute script.", e);
        }
        finally {
            Context.exit();
        }
    }
    
    private final class SuppressAccessClassShutter implements ClassShutter {
        
        public boolean visibleToScripts(String name) {
            return false;
        }
    }
}
