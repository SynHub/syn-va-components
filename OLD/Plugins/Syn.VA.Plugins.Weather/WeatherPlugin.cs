using System;
using Syn.Bot.Siml;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Weather.Adapter;
using Syn.VA.Plugins.Weather.View;
using Syn.VA.Plugins.Weather.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Weather
{
    public class WeatherPlugin : Plugin
    {
        public WeatherPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new WeatherAdapter());

                var weatherService = new Service("Weather-Report-Service",
                 () =>
                 {
                     WeatherAdapter.GenerateWeatherReport(bot.MainUser, Settings["Location"].Value);
                 });

                weatherService.Description = StringResource.WeatherPlugin_WeatherReportServiceDescription;
                weatherService.Interval = TimeSpan.FromHours(2);
                VA.Services.Add(weatherService);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Weather";

        public override string DisplayName => StringResource.WeatherPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var weatherPanel = new WeatherPanel();
                var weatherContext = weatherPanel.DataContext as WeatherContext;

                if (weatherContext != null)
                {
                    Settings.ApplyToProperties(weatherContext, "Location");
                    weatherContext.SaveAction = () =>
                    {
                        try
                        {
                            Settings["Location"].Value = weatherContext.Location;
                            VA.SettingsManager.Save(Settings, StringResource.WeatherPlugin_SettingsSavedMessage);
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                }

                return weatherPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}