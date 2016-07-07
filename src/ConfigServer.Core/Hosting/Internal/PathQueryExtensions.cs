using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ConfigServer.Core.Internal
{
    public static class PathQueryExtensions
    {
        public static PathQueryResult<string> TryMatchPath(this IEnumerable<string> options, PathString pathToQuery)
        {
            return options.TryMatchPath(s => s, pathToQuery);
        }

        public static PathQueryResult<T> TryMatchPath<T>(this IEnumerable<T> options, Func<T, string> selector, PathString pathToQuery)
        {
            foreach (var option in options)
            {
                var queryPath = $"/{selector(option)}";
                PathString remainingPath;
                if (pathToQuery.StartsWithSegments(queryPath, out remainingPath))
                    return PathQueryResult<T>.Success(option, remainingPath);
            }
            return PathQueryResult<T>.Failed<T>();
        }
    }
}
