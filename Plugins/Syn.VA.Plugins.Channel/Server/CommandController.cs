using System.Web.Http;

namespace Syn.VA.Plugins.Channel.Server
{
    public class CommandController: ApiController
    {
        [HttpGet]
        public Result Send(string message)
        {
            ApiSendMessage = message;
            return new Result(true, "Message Received.");
        }

        [HttpGet]
        public Result Status()
        {
            return new Result(true, "On");
        }

        public static string ApiSendMessage { get; set; }
    }
}