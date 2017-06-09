using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal static class HttpContextExtensions
    {
        public static async Task<JObject> GetJObjectFromJsonBodyAsync(this HttpContext source)
        {
            var json = await source.ReadBodyTextAsync();
            var result = JObject.Parse(json);
            return result;
        }

        public static async Task<T> GetObjectFromJsonBodyAsync<T>(this HttpContext source)
        {
            var json = await source.ReadBodyTextAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public static async Task<object> GetObjectFromJsonBodyOrDefaultAsync(this HttpContext source, Type type)
        {
            var json = await source.ReadBodyTextAsync();
            bool failed = false;
            var result = JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
            {
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    failed = true;
                    args.ErrorContext.Handled = true;
                }
            });
            return failed? null: result;
        }

        public static Task<string> ReadBodyTextAsync(this HttpContext source)
        {
            var body = source.Request.Body;
            return new StreamReader(body).ReadToEndAsync();
        }

        public static bool ChallengeUser(this HttpContext source, string claimType, ICollection<string> acceptableValues,bool allowAnomynous, IHttpResponseFactory responseFactory)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                return source.ChallengeAuthentication(allowAnomynous, responseFactory);

            //If we have an expected claim then we do not want to allow anomynous
            if(!source.ChallengeAuthentication(false, responseFactory))
                return false;

            if (!source.User.HasClaim(c=> claimType.Equals(c.Type, StringComparison.OrdinalIgnoreCase) && acceptableValues.Contains(c.Value)))
            {
                responseFactory.BuildStatusResponse(source, 403);
                return false;
            }

            return true;
        }

        public static bool ChallengeAuthentication(this HttpContext source, bool allowAnomynous, IHttpResponseFactory responseFactory)
        {
            if (!allowAnomynous && !source.User.Identity.IsAuthenticated)
            {
                responseFactory.BuildStatusResponse(source, 401);
                return false;
            }
            return true;
        }


        public static bool ChallengeClientRead(this HttpContext source, ConfigServerOptions option, ConfigurationClient client, IHttpResponseFactory responseFactory)
        {
            if(client == null)
            {
                responseFactory.BuildNotFoundStatusResponse(source);
                return false;
            }

            if (string.IsNullOrWhiteSpace(option.ClientReadClaimType) || string.IsNullOrWhiteSpace(client.ReadClaim))
                return source.ChallengeAuthentication(option.AllowAnomynousAccess, responseFactory);

            //If we have an expected claim then we do not want to allow anomynous
            if (!source.ChallengeAuthentication(false, responseFactory))
                return false;

            if (!source.User.HasClaim(c => option.ClientReadClaimType.Equals(c.Type, StringComparison.OrdinalIgnoreCase) && client.ReadClaim.Equals(c.Value, StringComparison.OrdinalIgnoreCase)))
            {
                responseFactory.BuildStatusResponse(source, 403);
                return false;
            }

            return true;
        }

        public static bool ChallengeClientWrite(this HttpContext source, ConfigServerOptions option, ConfigurationClient client, IHttpResponseFactory responseFactory)
        {
            if (client == null)
            {
                responseFactory.BuildNotFoundStatusResponse(source);
                return false;
            }

            if (string.IsNullOrWhiteSpace(option.ClientWriteClaimType) || string.IsNullOrWhiteSpace(client.WriteClaim))
                return source.ChallengeAuthentication(option.AllowAnomynousAccess, responseFactory);

            //If we have an expected claim then we do not want to allow anomynous
            if (!source.ChallengeAuthentication(false, responseFactory))
                return false;

            if (!source.HasClaim(option.ClientWriteClaimType, client.WriteClaim))
            {
                responseFactory.BuildStatusResponse(source, 403);
                return false;
            }

            return true;
        }

        public static bool CheckClientWrite(this HttpContext source, ConfigServerOptions option, ConfigurationClient client)
        {
            if (string.IsNullOrWhiteSpace(option.ClientReadClaimType) || string.IsNullOrWhiteSpace(client.WriteClaim))
                return true;
            return source.HasClaim(option.ClientWriteClaimType, client.WriteClaim);
        }

        public static bool HasClaim(this HttpContext source, string claimType, string claim)
        {
            return source.User.HasClaim(c => claimType.Equals(c.Type, StringComparison.OrdinalIgnoreCase) && claim.Equals(c.Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
