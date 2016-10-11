namespace Syn.VA.Plugins.Channel.Server
{
    public class Result
    {
        public Result()
        {
            Success = false;
            Message = string.Empty;
        }

        public Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}