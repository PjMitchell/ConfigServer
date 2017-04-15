using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IEndpoint
    {
        bool IsAuthorizated(HttpContext context, ConfigServerOptions options);
        Task<bool> TryHandle(HttpContext context);
    }
}
