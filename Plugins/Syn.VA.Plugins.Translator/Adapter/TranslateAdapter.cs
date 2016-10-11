using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.Translation;

namespace Syn.VA.Plugins.Translator.Adapter
{
    public class TranslateAdapter : IAdapter
    {
        readonly TranslatorPlugin _plugin;

        public TranslateAdapter(TranslatorPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Translate";

        public string Evaluate(Context context)
        {
            try
            {
                var targetAttribute = context.Element.Attribute("Target");
                if (targetAttribute != null)
                {
                    var translator = new TranslatorApi
                    {
                        SourceLanguage = _plugin.Settings["Source-Language"].Value,
                        SourceText = context.Element.Value,
                        TargetLanguage = targetAttribute.Value
                    };

                    Task.Factory.StartNew(() =>
                    {
                        var result = translator.Translate();

                        var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                        dispatcher.Invoke(() =>
                        {
                            context.User.Vars["translation-query"].Value = context.Element.Value;
                            if (!string.IsNullOrEmpty(result))
                            {
                                context.User.Vars["translation-text"].Value = result;
                                context.Bot.Trigger("translation-found");
                            }
                            else
                            {
                                context.Bot.Trigger("translation-not-found");
                            }
                        });
                    });
                }
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
            return string.Empty;
        }
    }
}