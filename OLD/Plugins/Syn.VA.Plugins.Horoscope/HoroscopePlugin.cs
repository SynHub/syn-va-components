using System;
using Syn.Bot.Siml;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Horoscope.Adapter;
using Syn.VA.Plugins.Horoscope.View;
using Syn.VA.Plugins.Horoscope.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Horoscope
{
    public class HoroscopePlugin : Plugin
    {
        public HoroscopePlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new HoroscopeAdapter(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Horoscope";

        public override string DisplayName => StringResource.HoroscopePlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var panel = new HoroscopePanel();
                var context = panel.DataContext as HoroscopeContext;
                if (context != null)
                {
                    Settings.ApplyToProperties(context, "Zodiac-Sign", "Zodiac-Sign-List");
                }

                panel.ZodiacComboBox.SelectionChanged += (sender, args) =>
                {
                    Settings["Zodiac-Sign"].Value = panel.ZodiacComboBox.SelectedItem.ToString();
                    VA.SettingsManager.Save(Settings);
                };

                return panel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}