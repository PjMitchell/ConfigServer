﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Security.Claims;

namespace ConfigServer.Core.Tests
{
    public class TestHttpContextBuilder
    {
        private readonly TestHttpContext source;

        private TestHttpContextBuilder(string path)
        {
            source = new TestHttpContext(path);
            source.User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public HttpContext TestContext => source;

        public static TestHttpContextBuilder CreateForPath(string path) => new TestHttpContextBuilder(path);

        public TestHttpContextBuilder WithMethod(string method)
        {
            source.Request.Method = method;
            return this;
        }
        public TestHttpContextBuilder WithPost() => WithMethod("POST");
        public TestHttpContextBuilder WithDelete() => WithMethod("DELETE");

        public TestHttpContextBuilder WithClaims(params Claim[] claims)
        {
            source.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
            return this;
        }
        public TestHttpContextBuilder WithJsonBody<TData>(TData data)
        {
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return WithStringBody(json);
        }

        public TestHttpContextBuilder WithFile(Stream stream, string name)
        {
            var form = new TestFormCollection();
            form.AddFile(stream, name);
            source.Request.Form = form;
            return this;
        }

        public TestHttpContextBuilder WithStringBody(string data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            source.Request.Body = stream;
            return this;
        }

        public TestHttpContextBuilder WithQueryParam(string key, string param)
        {
            ((TestQueryCollection)source.Request.Query).Add(key, param);
            return this;
        }
    }

}
