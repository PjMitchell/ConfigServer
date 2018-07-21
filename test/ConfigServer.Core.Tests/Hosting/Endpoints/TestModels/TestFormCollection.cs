using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO;

namespace ConfigServer.Core.Tests
{
    public class TestFormCollection : IFormCollection
    {
        private readonly Dictionary<string, StringValues> source;
        private readonly TestFormFileCollection fileCollection;
        public TestFormCollection()
        {
            source = new Dictionary<string, StringValues>();
            fileCollection = new TestFormFileCollection();
        }

        public StringValues this[string key] => source[key];

        public int Count => source.Count;

        public ICollection<string> Keys => source.Keys;

        public IFormFileCollection Files => fileCollection;

        public bool ContainsKey(string key) => source.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() => source.GetEnumerator();


        public bool TryGetValue(string key, out StringValues value) => source.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddFile(Stream content, string name)
        {
            var formFile = new TestFormFile
            {
                Name = name,
                FileName = name
            };
            formFile.SetReadStream(content);  
            fileCollection.AddFile(formFile);
        }

        public void AddFile(IFormFile formFile)
        {
            fileCollection.AddFile(formFile);
        }
    }
}
