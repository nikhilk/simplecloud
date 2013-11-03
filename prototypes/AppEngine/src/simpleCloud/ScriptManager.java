package simpleCloud;

import java.io.*;
import org.mozilla.javascript.*;

public final class ScriptManager {

    public final static ScriptManager Instance = new ScriptManager();
    
    private ScriptLoader _loader;
    
    private ScriptManager() {
        _loader = new ScriptLoader();
    }
    
    public String executeScript(String path, String name) {
        Context scriptContext = Context.enter();
        
        try {
            scriptContext.setClassShutter(new SuppressAccessClassShutter());
            
            String script = _loader.loadScript(path);
            
            Scriptable scriptScope = scriptContext.initStandardObjects();
            Object result = scriptContext.evaluateString(scriptScope, script, name, 1, null);
            
            return Context.toString(result);
        }
        catch (IOException e) {
            return e.getMessage();
        }
        catch (RhinoException e) {
            return e.getMessage();
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
