namespace ConfigServer.AzureBlobStorageProvider
{
    internal static class Helpers
    {
        public static string TrimFolderPath(string path)
        {
            var result = path;
            var index = result.IndexOf('/');
            while (index >= 0)
            {
                result = result.Substring(index + 1);
                index = result.IndexOf('/');
            }
            return result;
        }
    }
}
