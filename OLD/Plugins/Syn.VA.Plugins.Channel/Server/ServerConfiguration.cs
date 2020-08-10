using System;
using System.Web.Http;
using Owin;

namespace Syn.VA.Plugins.Channel.Server
{
    public class ServerConfiguration
    {
        // This method is required by Katana:
        public void Configuration(IAppBuilder app)
        {
            try
            {
                var webApiConfiguration = ConfigureWebApi();
                // Use the extension method provided by the WebApi.Owin library:
                app.UseWebApi(webApiConfiguration);
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "va/{controller}/{id}",
                new { id = RouteParameter.Optional });
            return config;
        }
    }
}