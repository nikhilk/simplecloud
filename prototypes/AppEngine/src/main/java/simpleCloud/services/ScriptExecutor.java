// ScriptExecutor.java
//

package simpleCloud.services;

public interface ScriptExecutor {

    public String executeScript(ScriptName name, String objectKey, Object object) throws ScriptException;
}
