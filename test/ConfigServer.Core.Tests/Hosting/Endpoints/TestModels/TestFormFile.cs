using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ConfigServer.Core.Tests
{
    public class TestFormFile : IFormFile
    {
        private Stream stream;

        public string ContentType { get; set; }

        public string ContentDisposition { get; set; }

        public IHeaderDictionary Headers { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            return stream;
        }

        public void SetReadStream(Stream stream)
        {
            this.stream = stream;
        }
    }
}
