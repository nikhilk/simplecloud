// ScriptExecutor.java
//

package simpleCloud.services;

public interface ScriptExecutor {

    public String executeScript(String path, String name) throws ScriptException;
}
