// ScriptExecutor.java
//

package simpleCloud.services;

import java.util.*;

public interface ScriptExecutor {

    public String executeScript(ScriptName name, boolean useSharedScript, String objectKey,
                                Object object) throws ScriptException;

    public ScriptName resolveScript(List<ScriptName> names);
}
