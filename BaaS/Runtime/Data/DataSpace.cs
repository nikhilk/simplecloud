// DataSpace.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using NodeApi.IO;
using SimpleCloud.Data.Providers;

namespace SimpleCloud.Data {

    public sealed class DataSpace {

        private Dictionary<string, DataSource> _dataSources;
        private Dictionary<string, DataCollection> _dataCollections;

        public DataSpace() {
            _dataSources = new Dictionary<string, DataSource>();
            _dataCollections = new Dictionary<string, DataCollection>();

            Load();
        }

        private void Load() {
            Dictionary<string, object> configuration = Application.Current.GetConfigurationObject("data");
            Dictionary<string, Dictionary<string, object>> sources = (Dictionary<string, Dictionary<string, object>>)configuration["sources"];

            if (sources != null) {
                LoadDataSources(sources);
            }
            if (_dataSources.Count != 0) {
                LoadDataCollections();
            }
        }

        private void LoadDataCollections() {
            string dataPath = Path.Join(Application.Current.Options.Path, "data");

            // TODO: Check if its a directory
            if (FileSystem.ExistsSync(dataPath)) {
                string[] names = FileSystem.ReadDirectorySync(dataPath);

                foreach (string name in names) {
                    string collectionPath = Path.Join(dataPath, name);
                    string collectionConfigPath = Path.Join(collectionPath, "config.json");

                    Dictionary<string, object> collectionConfig = Application.LoadConfiguration(collectionConfigPath, /* createEmptyIfNeeded */ false);
                    if (collectionConfig != null) {
                        string sourceName = (string)collectionConfig["source"];
                        DataSource source = _dataSources[sourceName];

                        if (source != null) {
                            _dataCollections[sourceName] = new DataCollection(sourceName, source, collectionConfig);
                        }
                        else {
                            Application.Current.ReportError("Unable to find a data source named '" + sourceName + "' for '" + name + "' data collection.");
                        }
                    }
                    else {
                        Application.Current.ReportError("Found a data collection directory named '" + name + "' without any configuration. Ignoring.", /* fatal */ false);
                    }
                }
            }
        }

        private void LoadDataSources(Dictionary<string, Dictionary<string, object>> configuration) {
            foreach (KeyValuePair<string, Dictionary<string, object>> sourceEntry in configuration) {
                string providerType = (string)sourceEntry.Value["provider"];
                DataProvider provider = null;

                if (providerType == "sql") {
                    provider = new SqlDataProvider();
                }

                if (provider != null) {
                    provider.Initialize(sourceEntry.Value);
                    _dataSources[sourceEntry.Key] = new DataSource(sourceEntry.Key, provider);
                }
                else {
                    Application.Current.ReportError("Invalid data provider attribute '" + providerType + "'.");
                }
            }
        }
    }
}
