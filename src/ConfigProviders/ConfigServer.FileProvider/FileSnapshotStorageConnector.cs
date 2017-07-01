using ConfigServer.TextProvider.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    class FileSnapshotStorageConnector : ISnapshotStorageConnector
    {
        readonly string folderPath;
        const string indexFile = "snapshotIndex.json";

        public FileSnapshotStorageConnector(FileConfigRespositoryBuilderOptions options)
        {
            folderPath = $"{options.ConfigStorePath}/Snapshot";
        }

        public Task<IEnumerable<SnapshotTextEntry>> GetSnapshotEntries(string snapshotId)
        {
            string path = GetSnapShotFolder(snapshotId).FullName;
            var result = Directory.EnumerateFiles(path)
                .Select(MapToTextEntry);

            return Task.FromResult(result);
        }

        public Task SetSnapshotEntries(string snapshotId, IEnumerable<SnapshotTextEntry> entries)
        {
            var directory = GetSnapShotFolder(snapshotId);
            DeleteSnaphot(directory);
            foreach (var entry in entries)
            {
                File.WriteAllText($"{directory.Name}/{entry.ConfigurationName}.json", entry.ConfigurationJson);
            }

            return Task.FromResult(true);
        }


        public Task<string> GetSnapshotRegistryFileAsync()
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            if (!File.Exists(path))
                return Task.FromResult(string.Empty);

            return Task.FromResult(File.ReadAllText(path));
        }

        public Task SetSnapshotRegistryFileAsync(string value)
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            File.WriteAllText(path, value);
            return Task.FromResult(true);
        }

        public Task DeleteSnapshot(string snapshotId)
        {
            var directory = GetSnapShotFolder(snapshotId);
            DeleteSnaphot(directory);
            return Task.FromResult(true);
        }

        private static void DeleteSnaphot(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
        }
        private static SnapshotTextEntry MapToTextEntry(string filePath)
        {
            return new SnapshotTextEntry(Path.GetFileNameWithoutExtension(filePath), File.ReadAllText(filePath));
        }

        private DirectoryInfo GetSnapShotFolder(string snapshotId)
        {
            var filestore = Directory.CreateDirectory($"{folderPath}/{snapshotId}");
            return filestore;
        }

        private DirectoryInfo GetFileStore()
        {
            return Directory.CreateDirectory(folderPath);
        }
    }
}
