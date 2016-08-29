using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace ConfigServer.Server
{
    internal class PartialForm : IFormCollection
    {
        private readonly Dictionary<string, StringValues> items;

        public PartialForm() : this(new Dictionary<string, StringValues>()) { }

        public PartialForm(Dictionary<string, StringValues> items)
        {
            this.items = items;
        }

        public StringValues this[string key] { get { return items[key]; } set { items[key] = value; } }
        
        public int Count => items.Count;

        public IFormFileCollection Files => null;

        public ICollection<string> Keys => items.Keys;

        public bool ContainsKey(string key) => items.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() => items.GetEnumerator();

        public bool TryGetValue(string key, out StringValues value) => items.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
