using System;
using System.Collections.Generic;
using Syn.Bot.Siml;
using Syn.VA.Controls;
using Syn.VA.Extensions;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Skype.Adapter;
using Syn.VA.Plugins.Skype.Model;
using Syn.VA.Plugins.Skype.Set;
using Syn.VA.Plugins.Skype.View;
using Syn.VA.Plugins.Skype.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Skype
{
    public class SkypePlugin : Plugin
    {
        public SkypePlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new SkypeAdapter(this));
                bot.Sets.Add(new SkypeNameSet(this));
                bot.Sets.Add(new SkypeIdSet(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Skype";

        public override string DisplayName => StringResource.SkypePlugin_DisplayName;

        public Dictionary<string, string> GetUsers()
        {
            var toReturn = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in Settings["Skype-Users"].GetTuples<string, string>())
            {
                if (!toReturn.ContainsKey(item.Item1)) toReturn.Add(item.Item1, item.Item2);
            }
            return toReturn;
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var skypePanel = new SkypePanel();
                var skypeContext = skypePanel.DataContext as SkypeContext;
                if (skypeContext != null)
                {
                    var settingsManager = VA.SettingsManager;
                    foreach (var item in Settings["Skype-Users"].GetTuples<string, string>())
                    {
                        var skypeUser = new SkypeUser(item.Item1, item.Item2);
                        skypeContext.Users.Add(skypeUser);
                    }
                    var settingsWindow = VA.Components.Get<SettingsWindow>();
                    skypeContext.AddAction = async () =>
                    {
                        try
                        {
                            var selectedName = await settingsWindow.GetInput(StringResource.SkypePlugin_AddNameTitle, StringResource.SkypePlugin_AddNameDescription);
                            if (!string.IsNullOrEmpty(selectedName))
                            {
                                var selectedId = await settingsWindow.GetInput(StringResource.SkypePlugin_AddIdTitle, StringResource.SkypePlugin_AddIdDescription);
                                if (!string.IsNullOrEmpty(selectedId))
                                {
                                    Settings["Skype-users"].Add(new Tuple<string, string>(selectedName, selectedId));
                                    skypeContext.Users.Add(new SkypeUser(selectedName, selectedId));
                                    settingsManager.Save(Settings);
                                    settingsWindow.DisplayMessage(Name, StringResource.SkypePlugin_UserAdded);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                    skypeContext.RemoveAction = () =>
                    {
                        try
                        {
                            var skypeUser = skypePanel.UserGrid.SelectedItem as SkypeUser;
                            if (skypeUser != null)
                            {
                                Settings["Skype-Users"].Remove(new Tuple<string, string>(skypeUser.Name, skypeUser.Id));
                                skypeContext.Users.Remove(skypeUser);
                                settingsManager.Save(Settings);
                                settingsWindow.DisplayMessage(Name, StringResource.SkypePlugin_UserRemoved);
                            }
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };

                }
                return skypePanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
            return null;
        }
    }
}