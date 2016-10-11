using System;
using Syn.Bot.Siml;
using Syn.Utility.Extensions;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Translator.Adapter;
using Syn.VA.Plugins.Translator.View;
using Syn.VA.Plugins.Translator.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Translator
{
    public class TranslatorPlugin : Plugin
    {
        public TranslatorPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new TranslateAdapter(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Translator";

        public override string DisplayName => StringResource.TranslatorPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var translatorPanel = new TranslatorPanel();
                var context = translatorPanel.DataContext as TranslatorContext;
                var languageText = Settings["Source-Language"].Value;

                if (context == null) return null;

                var simlBot = VA.Components.Get<SimlBot>();

                if (simlBot.Sets.Contains("Language"))
                {
                    context.LanguageCollection.AddRange(simlBot.Sets["Language"].GetValues());
                    context.SelectedLanguage = languageText;
                }
                else
                {
                    var languagesVariable = Settings["Languages"];
                    context.LanguageCollection.AddRange(languagesVariable.List);
                    context.SelectedLanguage = languageText;
                }

                translatorPanel.LanguageComboBox.SelectionChanged += (sender, args) =>
                {
                    Settings["Source-Language"].Value = translatorPanel.LanguageComboBox.SelectedItem.ToString();
                    VA.SettingsManager.Save(Settings, StringResource.TranslatorPlugin_SettingsSavedMessage);
                };

                return translatorPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
            return null;
        }
    }
}