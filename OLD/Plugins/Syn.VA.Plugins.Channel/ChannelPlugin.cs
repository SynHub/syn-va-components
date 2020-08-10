using System;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Channel.Server;
using Syn.VA.Plugins.Channel.View;
using Syn.VA.Plugins.Channel.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Channel
{
    public class ChannelPlugin : Plugin
    {
        public ChannelPlugin()
        {
            try
            {
                Server = new ChannelServer();

                VA.Loaded += (sender, args) =>
                {
                    var enabled = Settings["Enabled"].Value;
                    bool found;
                    if (bool.TryParse(enabled, out found) && found)
                    {
                        Server.Start();
                    }
                };
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public ChannelServer Server { get; }

        public override string Name => "Channel";

        public override string DisplayName => StringResource.ChannelPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var panel = new ChannelPanel();
                var context = panel.DataContext as ChannelContext;
                Settings.ApplyToProperties(context, "Port-Number","Enabled");

                panel.ToggleSwitch.IsCheckedChanged += (sender, args) =>
                {
                    Settings["Enabled"].Value = panel.ToggleSwitch.IsChecked.ToString();
                    VA.SettingsManager.Save(Settings);
                    var enabled = Settings["Enabled"].Value;
                    bool parsed;
                    if (bool.TryParse(enabled, out parsed))
                    {
                        if (parsed)
                        {
                            Server.Start();
                        }
                        else
                        {
                            Server.Stop();
                        }
                    }
                };

                panel.PortNumericUpDown.ValueChanged += (sender, args) =>
                {
                    Settings["Port-Number"].Value = Convert.ToInt32(panel.PortNumericUpDown.Value).ToString();
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