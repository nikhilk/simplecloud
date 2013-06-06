// DataSpace.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi;
using NodeApi.IO;
using SimpleCloud.Data.Sources;

namespace SimpleCloud.Data {

    public sealed class DataSpace {

        private Application _app;

        private Dictionary<string, DataSource> _dataSources;
        private Dictionary<string, DataCollection> _dataCollections;

        public DataSpace(Application app) {
            _app = app;

            _dataSources = new Dictionary<string, DataSource>();
            _dataCollections = new Dictionary<string, DataCollection>();

            Load();
        }

        public DataCollection GetCollection(string name) {
            return _dataCollections[name];
        }

        public DataSource GetSource(string name) {
            return _dataSources[name];
        }

        private void Load() {
            Runtime.TraceInfo("Loading DataSpace...");

            Dictionary<string, object> configuration = _app.GetConfigurationObject("data");
            Dictionary<string, Dictionary<string, object>> sources = (Dictionary<string, Dictionary<string, object>>)configuration["sources"];

            if (sources != null) {
                LoadDataSources(sources);
            }
            if (_dataSources.Count != 0) {
                LoadDataCollections();
            }
        }

        private void LoadDataCollections() {
            string dataPath = Path.Join(_app.Options.Path, "data");

            // TODO: Also check if its a directory
            if (FileSystem.ExistsSync(dataPath) == false) {
                Runtime.TraceWarning("Data directory '%s' was not found. No data collections were loaded.", dataPath);
                return;
            }

            foreach (string name in FileSystem.ReadDirectorySync(dataPath)) {
                string collectionPath = Path.Join(dataPath, name);
                string collectionConfigPath = Path.Join(collectionPath, "config.json");

                Dictionary<string, object> collectionConfig = Configuration.Load(collectionConfigPath, /* createEmptyIfNeeded */ false);
                if (collectionConfig != null) {
                    string sourceName = (string)collectionConfig["source"];
                    DataSource source = _dataSources[sourceName];

                    if (source != null) {
                        _dataCollections[name] = new DataCollection(name, collectionPath, collectionConfig, source);

                        Runtime.TraceInfo("Created data collection '%s' associated with data source named '%s'.", name, sourceName);
                    }
                    else {
                        Runtime.Abort("Unable to find a data source named '%s' for '%s' data collection.", sourceName, name);
                    }
                }
                else {
                    Runtime.TraceError("Configuration not found in data collection directory named '%s'. Ignoring.", name);
                }
            }
        }

        private void LoadDataSources(Dictionary<string, Dictionary<string, object>> configuration) {
            foreach (KeyValuePair<string, Dictionary<string, object>> sourceEntry in configuration) {
                string name = sourceEntry.Key;
                string type = (string)sourceEntry.Value["type"];

                if (type == "sql") {
                    _dataSources[name] = new SqlDataSource(_app, name, sourceEntry.Value);
                    Runtime.TraceInfo("Created sql data source named '%s'.", name);
                }
                else if (type == "inproc") {
                    _dataSources[name] = new InprocDataSource(_app, name, sourceEntry.Value);
                    Runtime.TraceInfo("Created local in-memory data source named '%s'.", name);
                }
                else {
                    Runtime.Abort("Invalid data source attribute '%s'.", type);
                }
            }
        }
    }
}
