using System;

namespace ConfigServer.Server.Templates
{
    internal static class CreateClientContent
    {
        public static string GetContent()
        {
            var guid = Guid.NewGuid();
            return $@"
            <h3>Create Client</h3>
            <form method=""post"">
            <h4>{guid}</h4>
            <input type=""hidden"" name=""ClientId"" value=""{guid}"">
            <h4>Name:</h4>
            <input type =""text"" name=""Name"" value=""{guid}"">
            <h4>Description:</h4>
            <input type =""text"" name=""Description"" value="""">
            <br/>
            <input type=""submit"" value=""Submit"">
            </form>";
        }
    }
}
