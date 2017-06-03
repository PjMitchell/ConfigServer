using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConfigServer.Core.Tests
{
    public class TestHttpContext : HttpContext
    {
        public TestHttpContext(string path)
        {
            Request = new TestHttpRequest(path);
        }

 

        public override AuthenticationManager Authentication { get { throw new NotImplementedException(); } }

        public override ConnectionInfo Connection { get { throw new NotImplementedException(); } }

        public override IFeatureCollection Features { get { throw new NotImplementedException(); } }

        public override IDictionary<object, object> Items { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override HttpRequest Request { get; }

        public override CancellationToken RequestAborted { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override IServiceProvider RequestServices { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override HttpResponse Response { get { throw new NotImplementedException(); } }

        public override ISession Session { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override string TraceIdentifier { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override ClaimsPrincipal User { get; set; }

        public override WebSocketManager WebSockets { get { throw new NotImplementedException(); } }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }

    
    
    }
