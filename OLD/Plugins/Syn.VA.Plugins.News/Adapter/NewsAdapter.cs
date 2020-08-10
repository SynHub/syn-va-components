using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.News.Adapter
{
    public class NewsAdapter : IAdapter
    {
        private readonly Plugin _plugin;

        public NewsAdapter(Plugin plugin)
        {
            _plugin = plugin;
        }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "News";

        public string Evaluate(Context context)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var map = context.Bot.Maps["news-name-link"];
                    var feed = _plugin.Settings["feed"].Value;
                    var mapItem = map[feed];
                    var document = XDocument.Load(mapItem.Value);
                    var transformed = string.Empty;
                    if (document.Root != null)
                    {
                        var xmlString = document.Root.ToString();
                        var generalSettings = VirtualAssistant.Instance.SettingsManager["VA"];
                        var xslPath = Path.Combine(generalSettings["resources-directory"].Value, "Web", "News.xsl");
                        var xslString = File.ReadAllText(xslPath);
                        transformed = SynUtility.Xml.TransformToHtml(xmlString, xslString);
                    }

                    var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                    dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(transformed))
                        {
                            context.User.Vars["News-Headline-Value"].Value = transformed;
                            context.Bot.Trigger("News-Headline-query");
                        }
                    });
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