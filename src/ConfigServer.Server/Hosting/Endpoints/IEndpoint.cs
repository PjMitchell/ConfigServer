using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IOldEndpoint
    {
        bool IsAuthorizated(HttpContext context, ConfigServerOptions options);
        Task<bool> TryHandle(HttpContext context);
    }

    internal interface IEndpoint
    {
        Task Handle(HttpContext context, ConfigServerOptions options);
    }
}
