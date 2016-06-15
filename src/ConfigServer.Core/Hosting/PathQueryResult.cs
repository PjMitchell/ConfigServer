using Microsoft.AspNetCore.Http;

namespace ConfigServer.Core.Hosting
{
    public class PathQueryResult<T>
    {
        private PathQueryResult()
        {

        }

        public static PathQueryResult<TResult> Success<TResult>(TResult queryResult, PathString remainingPath)
        {
            return new PathQueryResult<TResult>
            {
                HasResult = true,
                QueryResult = queryResult,
                RemainingPath = remainingPath
            };
        }

        public static PathQueryResult<TResult> Failed<TResult>()
        {
            return new PathQueryResult<TResult>();
        }

        public bool HasResult { get; private set; }
        public T QueryResult { get; private set; }
        public PathString RemainingPath { get; private set; }
    }

}
