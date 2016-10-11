using System;
using Syn.Bot.Siml;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Dictionary.Adapter;
using Syn.VA.Plugins.Dictionary.View;
using Syn.VA.Plugins.Dictionary.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Dictionary
{
    public class DictionaryPlugin : Plugin
    {
        public DictionaryPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new DictionaryAdapter(this));
                bot.Adapters.Add(new ThesaurusAdapter());
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Dictionary";

        public override string DisplayName => StringResource.DictionaryPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var toReturn = new DictionaryPanel();
                var context = toReturn.DataContext as DictionaryContext;
                Settings.ApplyToProperties(context, "Use-Common");
                if (context != null)
                {
                    context.ToggleValueAction = () =>
                    {
                        //Whenever the toggle button is used change value of IsEnabled
                        Settings["Use-Common"].Value = context.UseCommon.ToString();
                        VA.SettingsManager.Save(Settings);
                    };
                }

                return toReturn as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
            return null;
        }
    }
}
