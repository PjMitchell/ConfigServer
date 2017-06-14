using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IEndpoint
    {
        Task Handle(HttpContext context, ConfigServerOptions options);
    }
}
