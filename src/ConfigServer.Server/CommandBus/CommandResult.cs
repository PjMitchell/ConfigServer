namespace ConfigServer.Server
{
    internal class CommandResult
    {
        private CommandResult(bool isSuccessful, string errorMessage)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccessful { get; }
        public string ErrorMessage { get; }

        public static CommandResult Success() => new CommandResult(true, string.Empty);
        public static CommandResult Failure(string errorMessage) => new CommandResult(false, errorMessage);
    }
}
