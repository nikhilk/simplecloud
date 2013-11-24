// ScriptLoader.java
//

package simpleCloud.services;

import java.io.*;
import java.util.*;

public interface ScriptLoader {

    public List<ScriptName> getNames();

    public String loadScript(ScriptName name) throws IOException;
}
