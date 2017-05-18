using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace ConfigServer.Core.Tests
{
    public class TestHttpRequest : HttpRequest
    {
        public TestHttpRequest(string path)
        {
            Path = path;
            Method = "GET";
            Body = new MemoryStream();
            Query = new TestQueryCollection();
        }

        public override Stream Body { get; set; }

        public override long? ContentLength { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string ContentType { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override IRequestCookieCollection Cookies { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override IFormCollection Form { get; set; }

        public override bool HasFormContentType { get { throw new NotImplementedException(); } }

        public override IHeaderDictionary Headers { get { throw new NotImplementedException(); } }

        public override HostString Host { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override HttpContext HttpContext { get { throw new NotImplementedException(); } }

        public override bool IsHttps { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string Method { get; set; }

        public override PathString Path { get; set; }

        public override PathString PathBase { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string Protocol { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override IQueryCollection Query { get; set; }

        public override QueryString QueryString { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string Scheme { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }

    public class TestQueryCollection : IQueryCollection
    {
        private Dictionary<string, StringValues> source;

        public TestQueryCollection()
        {
            source = new Dictionary<string, StringValues>();
        }

        public void Add(string key, StringValues value) => source.Add(key, value);

        public StringValues this[string key] => source[key];

        public int Count => source.Count;

        public ICollection<string> Keys => source.Keys;

        public bool ContainsKey(string key) => source.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            return source.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
