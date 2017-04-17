using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ConfigServer.Core.Tests
{
    public class TestHttpRequest : HttpRequest
    {
        public TestHttpRequest(string path)
        {
            Path = path;
            Method = "GET";
            Body = new MemoryStream();
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

        public override IQueryCollection Query { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override QueryString QueryString { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string Scheme { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
