// LocalScriptLoader.java
//

package simpleCloud.core;

import java.io.*;
import java.util.*;
import simpleCloud.*;
import simpleCloud.services.*;

public final class LocalScriptLoader implements ScriptLoader {

    private Application _app;

    public LocalScriptLoader(Application app) {
        _app = app;
    }

    @Override
    public List<ScriptName> getNames() {
        List<ScriptName> allNames = new ArrayList<ScriptName>();

        File appFolder = new File("app");
        for (ApplicationFeature feature : _app.getFeatures()) {
            if (feature instanceof ScriptFeature) {
                String featureName = feature.getName();

                File featureFolder = new File(appFolder, featureName);
                if (!featureFolder.isDirectory()) {
                    continue;
                }

                if (((ScriptFeature)feature).usesGroupedScriptFiles()) {
                    for (File groupFolder : featureFolder.listFiles()) {
                        if (groupFolder.isDirectory()) {
                            List<ScriptName> names = getNamesFromFolder(featureName, groupFolder.getName(), groupFolder);
                            allNames.addAll(names);
                        }
                    }
                }
                else {
                    List<ScriptName> names = getNamesFromFolder(featureName, null, featureFolder);
                    allNames.addAll(names);
                }
            }
        }

        return allNames;
    }

    private List<ScriptName> getNamesFromFolder(String featureName, String groupName, File folder) {
        List<ScriptName> names = new ArrayList<ScriptName>();

        for (File file : folder.listFiles()) {
            if (file.isFile()) {
                String fileName = file.getName();
                fileName = fileName.substring(0, fileName.lastIndexOf('.'));

                String script = null;
                try {
                    script = readFile(file.getPath());

                    ScriptName name = new ScriptName(featureName, groupName, fileName, script);
                    names.add(name);
                }
                catch (IOException ioe) {
                    // TODO: Error reporting
                }
            }
        }

        return names;
    }

    @Override
    public String loadScript(ScriptName name) throws IOException {
        return (String)name.getObject();
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
