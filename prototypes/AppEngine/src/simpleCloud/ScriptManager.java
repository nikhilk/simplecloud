package simpleCloud;

import org.mozilla.javascript.*;

public final class ScriptManager {

    public final static ScriptManager Instance = new ScriptManager();
    
    private ScriptManager() {
    }
    
    public String executeScript(String name, String script) {
        Context scriptContext = Context.enter();
        
        try {
            scriptContext.setClassShutter(new SuppressAccessClassShutter());
            
            Scriptable scriptScope = scriptContext.initStandardObjects();
            Object result = scriptContext.evaluateString(scriptScope, script, name, 1, null);
            
            return Context.toString(result);
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
