using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.Horoscope;

namespace Syn.VA.Plugins.Horoscope.Adapter
{
    public class HoroscopeAdapter : IAdapter
    {
        private readonly HoroscopePlugin _plugin;

        public bool IsRecursive => true;
        public XName TagName => SimlSpecification.Namespace.O + "Horoscope";

        public HoroscopeAdapter(HoroscopePlugin plugin)
        {
            _plugin = plugin;
        }

        public string Evaluate(Context context)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var signAttribute = context.Element.Attribute("Sign");

                    string zodiacSignString;
                    if (signAttribute != null)
                    {
                        zodiacSignString = signAttribute.Value;
                    }
                    else if (!string.IsNullOrEmpty(context.Element.Value))
                    {
                        zodiacSignString = context.Element.Value;
                    }
                    else
                    {
                        zodiacSignString = _plugin.Settings["Zodiac-Sign"].Value;
                    }

                    ZodiacSign zodiacSign;
                    var foundSign = Enum.TryParse(zodiacSignString, true, out zodiacSign);
                    if (foundSign)
                    {
                        var horoscopeValue = new HoroscopeApi().GetHoroscope(zodiacSign);
                        var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                        context.User.Vars["horoscope-sign"].Value = zodiacSignString;
                        if (dispatcher != null && !string.IsNullOrEmpty(horoscopeValue))
                        {
                            dispatcher.Invoke(() =>
                            {
                                context.User.Vars["horoscope-value"].Value = horoscopeValue;
                                context.Bot.Trigger("horoscope-query");
                            });
                        }
                        else
                        {
                            context.Bot.Trigger("horoscope-query-error");
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}
