// LocalScriptLoader.java
//

package simpleCloud.storage;

import java.io.*;
import java.util.*;
import simpleCloud.services.*;

public final class LocalScriptLoader implements ScriptLoader {

    @Override
    public List<ScriptName> getNames() {
        List<ScriptName> names = new ArrayList<ScriptName>();

        File appFolder = new File("app");
        for (File scriptFile : appFolder.listFiles()) {
            String fileName = scriptFile.getName();
            fileName = fileName.substring(0, fileName.indexOf('.'));

            names.add(new ScriptName(fileName));
        }

        return names;
    }

    @Override
    public String loadScript(ScriptName name) throws IOException {
        String path = "app/" + name.getName() + ".js";
        return readFile(path);
    }

    private String readFile(String filePath) throws IOException {
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
}
