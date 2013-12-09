// YamlConfiguration.java
//

package simpleCloud.core;

import java.io.*;
import java.util.*;
import org.yaml.snakeyaml.*;
import org.yaml.snakeyaml.constructor.*;
import simpleCloud.services.*;

public final class YamlConfiguration implements ConfigurationService {

    private static final Object configLoadingLock = new Object();

    private StorageService _storage;
    private HashMap<String, Map<Object, Object>> _configData;

    public YamlConfiguration(ServiceProvider services) {
        _storage = services.getService(StorageService.class);
        _configData = new HashMap<String, Map<Object, Object>>();
    }

    @Override
    public Map<Object, Object> getConfiguration() {
        return getConfiguration("settings");
    }

    @Override
    public Map<Object, Object> getConfiguration(String name) {
        if (!_configData.containsKey(name)) {
            synchronized (YamlConfiguration.configLoadingLock) {
                if (!_configData.containsKey(name)) {
                    Map<Object, Object> config = loadConfiguration(name);

                    _configData.put(name, config);
                    return config;
                }
            }
        }

        return _configData.get(name);
    }

    @SuppressWarnings("unchecked")
    private Map<Object, Object> loadConfiguration(String name) {
        StorageFile configDirectory = _storage.getRoot().getFile("config");
        if (!configDirectory.isDirectory()) {
            return null;
        }

        String configFileName = name + ".yaml";
        StorageFile configFile = configDirectory.getFile(configFileName);

        if (configFile.exists()) {
            try {
                String yamlContent = configFile.getContent();

                Yaml yamlParser = new Yaml(new ConfigConstructor());
                return (Map<Object, Object>)yamlParser.load(yamlContent);
            }
            catch (IOException e) {
                // TODO: Log error
            }
        }

        return null;
    }

    private final class ConfigConstructor extends Constructor {

        public ConfigConstructor() {
            super(Map.class);
        }

        @Override
        protected List<Object> createDefaultList(int initSize) {
            return new ArrayList<Object>(initSize);
        }

        @Override
        protected Map<Object, Object> createDefaultMap() {
            return new HashMap<Object, Object>();
        }
    }
}
