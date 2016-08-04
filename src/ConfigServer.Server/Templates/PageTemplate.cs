using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConfigServer.Server.Templates
{
    internal class PageBuilder
    {
        private readonly HttpContext context;

        public PageBuilder(HttpContext context)
        {
            this.context = context;
        }

        public Task WriteContent(string title, string content)
        {
            return context.Response.WriteAsync(GetTemplate(title, content));
        }

        public void Redirect(string location)
        {
            context.Response.Redirect(location);
        }

        private static string GetTemplate(string title, string content)
        {
            return $@"
            <!DOCTYPE html>
            <html>
                <head>
                    <meta charset=""utf - 8"" />
                    <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" />
                    <title>{title}</title>
                </head>
                <body>
                    <header>{title}</header>       
                    {content}
                    <footer>       
                        <p></p>
                    </footer>
                </body>
            </html>";
        } 
    }
}
