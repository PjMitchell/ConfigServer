using ConfigServer.Core;

namespace ConfigServer.Server.Templates
{
    internal static class EditClientContent
    {
        public static string GetContent(ConfigurationClient client)
        {
            return $@"
            <h3>Edit Client</h3>
            <form method=""post"">
            <h4>{client.ClientId}</h4>
            <input type=""hidden"" name=""ClientId"" value=""{client.ClientId}"">
            <h4>Name:</h4>
            <input type =""text"" name=""Name"" value=""{client.Name}"">
            <h4>Description:</h4>
            <input type =""text"" name=""Description"" value=""{client.Description}"">
            <br/>
            <input type=""submit"" value=""Submit"">
            </form>";
        }
    }
}
