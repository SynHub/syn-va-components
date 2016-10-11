using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.Wikipedia;
using Syn.VA.Libraries.Wikipedia.Enums;
using Syn.VA.Libraries.Wikipedia.Misc;

namespace Syn.VA.Plugins.Wikipedia
{
    public class WikipediaAdapter : IAdapter
    {
        private readonly WikipediaApi _wikipediaApi;

        public WikipediaAdapter()
        {
            _wikipediaApi = new WikipediaApi { UseTLS = true, Limit = 5, What = What.Text };
        }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Wikipedia";

        public string Evaluate(Context context)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var results = _wikipediaApi.Search(context.Element.Value);
                    var extract = string.Empty;
                    foreach (var search in results.Search)
                    {
                        extract = search.GetExtract();
                        break;
                    }
                    var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                    if (dispatcher != null && !string.IsNullOrEmpty(extract))
                    {
                        dispatcher.Invoke(() =>
                        {
                            context.User.Vars["wikipedia-extract"].Value = extract;
                            context.Bot.Trigger("wikipedia-query", context.User);
                        });
                    }
                    else
                    {
                        context.Bot.Trigger("wikipedia-query-error");
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