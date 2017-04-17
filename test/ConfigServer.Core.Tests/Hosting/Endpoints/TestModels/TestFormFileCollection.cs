using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ConfigServer.Core.Tests
{
    public class TestFormFileCollection : IFormFileCollection
    {
        private List<IFormFile> source;

        public TestFormFileCollection()
        {
            source = new List<IFormFile>();
        }


        public IFormFile this[string name] => source.Single(s=> s.Name == name);

        public IFormFile this[int index] => source[0];

        public int Count => source.Count;

        public IEnumerator<IFormFile> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        public IFormFile GetFile(string name)
        {
            return this[name];
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return source.Where(w => w.Name == name).ToList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void AddFile(IFormFile formfile)
        {
            source.Add(formfile);
        }
    }
}
