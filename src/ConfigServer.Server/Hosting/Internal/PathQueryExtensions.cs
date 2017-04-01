using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extention methods for PathString queries 
    /// </summary>
    internal static class PathQueryExtensions
    {
        /// <summary>
        /// Tries to match pathstring to a list of options
        /// </summary>
        /// <param name="options">available options</param>
        /// <param name="pathToQuery">path that is being queried</param>
        /// <returns>PathQueryResult from selected path and available options</returns>
        public static PathQueryResult<string> TryMatchPath(this IEnumerable<string> options, PathString pathToQuery)
        {
            return options.TryMatchPath(s => s, pathToQuery);
        }

        /// <summary>
        /// Tries to match pathstring to a list of options
        /// </summary>
        /// <typeparam name="T">Type of the options available</typeparam>
        /// <param name="options">available options</param>
        /// <param name="selector">path to convert option to path component</param>
        /// <param name="pathToQuery"></param>
        /// <returns>PathQueryResult from selected path and available options</returns>
        public static PathQueryResult<T> TryMatchPath<T>(this IEnumerable<T> options, Func<T, string> selector, PathString pathToQuery)
        {
            foreach (var option in options)
            {
                var queryPath = $"/{selector(option)}";
                PathString remainingPath;
                if (pathToQuery.StartsWithSegments(queryPath,StringComparison.CurrentCultureIgnoreCase, out remainingPath))
                    return PathQueryResult<T>.Success(option, remainingPath);
            }
            return PathQueryResult<T>.Failed<T>();
        }

        public static string[] ToPathParams(this HttpContext context)
        {
            var pathParams = context.Request.Path.ToPathParams();
            return pathParams;
        }

        public static string[] ToPathParams(this PathString path)
        {
            var pathParams = path.HasValue
                ? path.Value.Split('/').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray()
                : new string[0];
            return pathParams;
        }
    }
}
