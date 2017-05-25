using ConfigServer.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    internal class FileConfigArchive : IConfigArchive
    {
        readonly string folderPath;

        public FileConfigArchive(FileConfigRespositoryBuilderOptions options)
        {
            folderPath = $"{options.ConfigStorePath}/Archive";
        }

        public Task DeleteArchiveConfig(string name, ConfigurationIdentity identity)
        {
            string path = Path.Combine(GetArchiveResourceSetFolder(identity).FullName, name);
            File.Delete(path);
            return Task.FromResult(true);
        }

        public Task DeleteOldArchiveConfigs(DateTime deletionDate, ConfigurationIdentity identity)
        {
            string path = GetArchiveResourceSetFolder(identity).FullName;
            var filesToDelete = Directory.EnumerateFiles(path)
                .Select(s => new FileInfo(s))
                .Where(f => f.LastWriteTimeUtc <= deletionDate);

            foreach (var fileToDelete in filesToDelete)
                fileToDelete.Delete();

            return Task.FromResult(true);
        }

        public Task<ConfigArchiveEntry> GetArchiveConfig(string name, ConfigurationIdentity identity)
        {
            string path = Path.Combine(GetArchiveResourceSetFolder(identity).FullName, name);
            if (!File.Exists(path))
                return Task.FromResult(new ConfigArchiveEntry { Name = name, Content = string.Empty });
            var storageObject = JObject.Parse(File.ReadAllText(path));
            var json = storageObject.GetValue("Config").ToString();
            var result = new ConfigArchiveEntry
            {
                Name = name,
                Content = json,
                HasEntry = true
            };
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ConfigArchiveEntryInfo>> GetArchiveConfigCatalogue(ConfigurationIdentity identity)
        {
            string path = GetArchiveResourceSetFolder(identity).FullName;
            var result = Directory.EnumerateFiles(path)
                .Select(MapEntryInfo);

            return Task.FromResult(result);
        }

        private ConfigArchiveEntryInfo MapEntryInfo(string path)
        {
            var config = JsonConvert.DeserializeObject<ConfigStorageInfo>(File.ReadAllText(path));
            return new ConfigArchiveEntryInfo
            {
                Name = Path.GetFileName(path),
                Configuration = config.ConfigName,
                TimeStamp = config.TimeStamp
            };
        }

        private DirectoryInfo GetArchiveResourceSetFolder(ConfigurationIdentity identity)
        {
            var filestore = Directory.CreateDirectory($"{folderPath}/{identity.Client.ClientId}");
            return filestore;
        }

        private class ConfigStorageInfo
        {
            public string ServerVersion { get; set; }
            public string ClientId { get; set; }
            public string ConfigName { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
