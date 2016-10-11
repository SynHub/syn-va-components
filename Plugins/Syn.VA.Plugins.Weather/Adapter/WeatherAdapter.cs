using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.Weather;

namespace Syn.VA.Plugins.Weather.Adapter
{
    public class WeatherAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Weather";

        public static void GenerateWeatherReport(BotUser user, string location)
        {
            var va = VirtualAssistant.Instance;
            var settings = va.SettingsManager["Weather"];
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(location))
                {
                    location = settings["location"].Value;
                }

                var document = WeatherApi.GetWeatherDocument(location);
                XElement descriptionElement = null;
                var resultsElement = document.Root?.Element("results");
                if (resultsElement != null && !resultsElement.IsEmpty)
                {
                    descriptionElement = resultsElement.Element("channel").Element("item").Element("description");
                }

                //return desc;
                var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                dispatcher.Invoke(() =>
                {
                    if (descriptionElement != null)
                    {
                        var parsedDescription = XElement.Parse($"<Item>{descriptionElement.Value}</Item>");
                        var cdata = (XCData) parsedDescription.FirstNode;
                        user.Vars["weather-location"].Value = location;
                        user.Vars["weather-value"].Value = cdata.Value;
                        user.Bot.Trigger("Weather-query", user);
                    }
                    else
                    {
                        user.Vars["weather-location"].Value = location;
                        user.Bot.Trigger("Weather-query-no-result", user);
                    }
                });
            });
        }

        public string Evaluate(Context context)
        {
            try
            {
                GenerateWeatherReport(context.User, context.Element.Value);
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}