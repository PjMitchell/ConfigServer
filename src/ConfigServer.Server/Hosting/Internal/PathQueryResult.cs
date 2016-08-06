using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the result of a path query returning whether or not the query was successful and the resulting object from the query 
    /// </summary>
    /// <typeparam name="T">Type of the object returned by the query</typeparam>
    internal class PathQueryResult<T>
    {
        private PathQueryResult() {}
        private PathQueryResult(T queryResult, PathString remainingPath)
        {
            HasResult = true;
            QueryResult = queryResult;
            RemainingPath = remainingPath;
        }


        /// <summary>
        /// Initializes a Successful path query result with object found by query and remaining path 
        /// </summary>
        /// <typeparam name="TResult">Type of object found by query</typeparam>
        /// <param name="queryResult">object found by query</param>
        /// <param name="remainingPath">remaining path from the original query</param>
        /// <returns>The initialized successful PathQueryResult</returns>
        public static PathQueryResult<TResult> Success<TResult>(TResult queryResult, PathString remainingPath) => new PathQueryResult<TResult>(queryResult, remainingPath);

        /// <summary>
        ///  Initializes a failed path query result
        /// </summary>
        /// <typeparam name="TResult">Type of object expected by quer</typeparam>
        /// <returns>The initialized failed PathQueryResult</returns>
        public static PathQueryResult<TResult> Failed<TResult>() => new PathQueryResult<TResult>();

        /// <summary>
        /// Did the query return an object?
        /// </summary>
        public bool HasResult { get; }

        /// <summary>
        /// Object returned by the query
        /// </summary>
        public T QueryResult { get; }

        /// <summary>
        /// Path remaining from the original query once the queried object is removed from the path
        /// </summary>
        public PathString RemainingPath { get; }
    }

}
